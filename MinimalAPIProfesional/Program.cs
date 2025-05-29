using System.ComponentModel.DataAnnotations;
using System.Threading;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;                       // IDistributedCache "builder.Services.AddDistributedSqlServerCache(options => { ... });"
using Microsoft.Extensions.Caching.Memory;                            // IMemoryCache "builder.Services.AddMemoryCache();"

using FluentValidation;                                               // Services.AddValidatorsFromAssemblyContaining
using FluentValidation.Results;                                       // ValidationResult

using Serilog;                                                        //  .WriteTo.File / LoggerConfiguration 
using Serilog.Core;

using MinimalAPIProfesional;
using MinimalAPIProfesional.Validation;
using MinimalAPIProfesional.Data.Models;
using MinimalAPIProfesional.Data;                                     // ApiDbContext
using MinimalAPIProfesional.Services;
using MinimalAPIProfesional.DTO___Models;
using MinimalAPIProfesional.Endpoints;


// ====================================================================================================================================================================================================
//                                                                                                                                                                                                    !
//                                                                                         Construction                                                                                               !
//                                                                                                                                                                                                    !
// ====================================================================================================================================================================================================
// Creating the WebApplicationBuilder factory class ---------------------------------------------------------------------------------------------------------------------------------------------------
WebApplicationBuilder? builder  = WebApplication.CreateBuilder(args);
//var builder = WebApplication.CreateBuilder(new WebApplicationOptions
//{
//     EnvironmentName = Environments.Development
//});



// Registering and configuring services ---------------------------------------------------------------------------------------------------------------------------------------------------------------
builder.Logging.ClearProviders();                                                              // Supprime le système de Logging qu'Asp.Net a mis par défaut

LoggerConfiguration? loggerConfiguration = new LoggerConfiguration()                           // Création d'une configuration de log destinée à Serilog
     .WriteTo.Console()
     .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day);

Logger? logger = loggerConfiguration.CreateLogger();

builder.Logging.AddSerilog(logger);



// enregistrement des validators ----------------------------------------------------------------------------------------------------------------------------------------------------------------------
builder.Services.AddValidatorsFromAssemblyContaining<Program>();



// Cache mémoire manuel -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//builder.Services.AddMemoryCache();



// Output Cache (Automatique) -------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//builder.Services.AddOutputCache(opt =>
//{
//     opt.AddBasePolicy(b => b.Expire(TimeSpan.FromMinutes(1)));                                                                                     // Stratégie par défaut

//     opt.AddPolicy("Expire2Min", p => p.Expire(TimeSpan.FromMinutes(2)).Tag("personnes"));                                                          // Stratégie spécifique
//                                                                                                                                                    // avec un Tag associé pour en permettre la RAZ
//                                                                                                                                                    // en utilisant la méthode "EvictByTagAsync" de l'interface "IOutputCacheStore"
//                                                                                                                                                    // dans un PUT ou un DELETE

//     //opt.AddPolicy("Expire10S", p => p.Expire(TimeSpan.FromSeconds(10)));                                                                         // Stratégie spécifique
//     opt.AddPolicy("Expire10S", p =>                                                                                                                // Stratégie spécifique
//     {
//          p.Cache()
//          .Expire(TimeSpan.FromSeconds(10))
//          .Tag("seconds10");
//     });

//     opt.AddPolicy("ById", p => p.SetVaryByRouteValue("id"));                                                                                       // Stratégie précisant qu'il y aura un cache par identifiant fourni dans la route !
//});



// Distributed Cache for Sql Server -------------------------------------------------------------------------------------------------------------------------------------------------------------------
//builder.Services.AddDistributedSqlServerCache(options =>
//{
//     options.ConnectionString      = builder.Configuration.GetConnectionString("ConnectionStringSqlServer_DistributedCache");
//     options.SchemaName            = "dbo";
//     options.TableName             = "TestCache";
//});



// Distributed Cache for Redis ------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//builder.Services.AddStackExchangeRedisCache(options =>
//{
//     options.Configuration         = builder.Configuration.GetConnectionString("ConnectionStringRedis_DistributedCache");
//     options.InstanceName          = "MinimalAPIProfesional_";                                                                                        // Préfixe pour les clefs stockées dans Redis
//});



// Enregistrement du contexte de la base de données ---------------------------------------------------------------------------------------------------------------------------------------------------
//builder.Services.AddDbContext<ApiDbContext>(opt => opt.UseSqlite(builder.Configuration.GetConnectionString("SqLite-DEVELOPMENT")));
builder.Services.AddDbContext<ApiDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionStringSqlServer_DEVELOPMENT")));
//builder.Services.AddDbContext<ApiDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionStringSqlServer_PRODUCTION")));



// Enregistrement des Services ------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//builder.Services.AddScoped<IPersonService, EFCorePersonService>();    // Précise l'interface "IPersonService" et son implémentation à utiliser "EFCorePersonService"
//                                                                      //
//                                                                      // "AddScoped", en ASP.NET Web API, permet de dire que la durée de vie
//                                                                      // du service sera la même que celle de la requête http et l'instance du service
//                                                                      // sera nettoyée de la mémoire (surtout s'il dispose de IDisposable)
//                                                                      // à la fin de vie de la requête http
//                                                                      //
//                                                                      // Pour info "AddTranscient" ne lie pas la durée de vie du service à celle de la requête mais à sa propre fin.
//                                                                      // Ainsi, si une requête a besoin trois fois de ce service alors trois instances différentes de ce service
//                                                                      // seront créées !
//                                                                      //
//                                                                      // Pour résumer :
//                                                                      //     - Singleton     : 1 instance permanente (pour toutes les requêtes)
//                                                                      //     - AddScoped     : 1 instance par requête
//                                                                      //     - AddTranscient : 1 instance à chaque fois que ce service est appelé même si c'est au sein de la même requête
builder.Services.MapPersonServices();                                 // Remplace ce qui est au-dessus et est utilisé avec la classe 'PersonEndpoints.cs' qui prend en charge l'externalisation des Endpoints du 'program.cs'


builder.Services.AddEndpointsApiExplorer();                           // Service Microsoft permettant d'explorer les différents Enpoints pour fournir l'information à Swagger
builder.Services.AddSwaggerGen();                                     // Service utilisant le package Swagger



// Building the WebApplication object -----------------------------------------------------------------------------------------------------------------------------------------------------------------
var app = builder.Build();



// Génération du fichier swagger.json et de l'interface graphique exposant le fichier------------------------------------------------------------------------------------------------------------------
if (app.Environment.IsDevelopment())
{
     app.UseSwagger();                                                // Génère le fichier Swagger.json nécessaire pour les clients voulant consommer l'API
     app.UseSwaggerUI();                                              // Génère l'interface graphique qui va lire le fichier Swagger.json (pour une interface plus conviviale sous forme de page HTML)
}





// ====================================================================================================================================================================================================
//                                                                                                                                                                                                    !
//                                                                              Appel de Services et Middlewares                                                                                      !
//                                                                                                                                                                                                    !
// ====================================================================================================================================================================================================
// Appel des Services ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
// Création de la base de données si elle n'existe pas : !!!!! A ne pas faire en Production car nous ne profiterions plus du système de "Migration" pour faire évoluer la base !!!!!
//app.Services
//     .CreateScope().ServiceProvider
//     .GetRequiredService<ApiDbContext>().Database
//     .EnsureCreated();



// Checks whether there is a migration to be applied and does so if necessary -------------------------------------------------------------------------------------------------------------------------
//await app.Services
//     .CreateScope().ServiceProvider
//     .GetRequiredService<ApiDbContext>().Database
//     //.ensureCreated();
//     .MigrateAsync();



// Appel des Middlewares ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
// Déclaration de l'utilisation du cache automatique
//app.UseOutputCache();



//// Regroupent des Endpoints concernant les "Person" -------------------------------------------------------------------------------------------------------------------------------------------------
//app.MapGroup("/person")
//     .MapPersonEndpoints();





// ====================================================================================================================================================================================================
//                                                                                                                                                                                                    !
//                                                                                          Endpoints GET                                                                                             !
//                                                                                                                                                                                                    !
// ====================================================================================================================================================================================================
#region hello
//app.MapGet("/hello",
//(
//     [FromServices] ILogger<Program> logger
//) =>
//{

//     logger.LogInformation("Log depuis l'Endpoint Hello");
//     return Results.Ok("Hello World!");
//});



//app.MapGet("/hello/{p_nom}",
//(
//     [FromRoute]    string              p_nom,
//     [FromServices] ILogger<Program>    logger
//) =>
//{

//     logger.LogInformation("Nous avons salué {p_nom}", p_nom);                                                                                        // Log avec Templating
//     return Results.Ok($"Bonjour {p_nom}");
//});
#endregion



#region person 1er lot
////app.MapGet("/person", async (
////     [FromServices] ApiDbContext context) =>
////{

////     List<Person> lp = await context.Persons.ToListAsync();

////     return Results.Ok(lp);
////});
#endregion



#region validations en entrée
////app.MapGet("/person/{id:int}", async (
////     [FromRoute]    int            id,

////     [FromServices] ApiDbContext   Dbcontext) =>
////{

////     Person? pers = await Dbcontext.Persons.Where(p => p.Id == id).FirstOrDefaultAsync();

////     if (pers is null) return Results.NotFound();

////     return Results.Ok(pers);
////});
#endregion



#region avec base de données sans filtre
// Avec memory cache manuel ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//app.MapGet("/person", async
//(
//     [FromServices] ApiDbContext        context,

//                    CancellationToken   token
//) =>
//{

//     List<Person> person = await context.PersonTable.ToListAsync(token);
//     return Results.Ok(person);
//});
#endregion



#region avec base de données avec un filtre
// Avec memory cache manuel ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//app.MapGet("/person/{id:int}", async
//(
//     [FromRoute]         int                 id,

//     [FromServices]      ApiDbContext        context,

//                         CancellationToken   token
//) =>
//{

//     //Person p = await context.Persons.Where(w => w.Id == id).ToList();
//     Person p = await context.PersonTable.Where(w => w.Id == id).FirstOrDefaultAsync(token);

//          if (p is null) return Results.NotFound();

//          return Results.Ok(p);
//});
#endregion



#region avec Memory cache (manuel)
// Avec Memory cache (manuel) -------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//app.MapGet("/person/{id:int}", async
//(
//     [FromRoute]    int                 id,

//     [FromServices] ApiDbContext        context,
//     [FromServices] IMemoryCache        cache,

//                    CancellationToken   token
//) =>
//{

//     if (cache.TryGetValue<Person>($"personne_{id}", out Person? p))                // J'essaie en premier de récupérer la Person à partir du cache
//     {

//          return Results.Ok(p);
//     }
//     else                                                                            // Sinon j'essaie à partir de la base de données
//     {

//          p = await context.PersonTable.Where(w => w.Id == id).FirstOrDefaultAsync(token);

//          if (p is null)
//          {

//               return Results.NotFound();
//          }
//          else
//          {

//               cache.Set($"personne_{id}", p);                                       // Je stocke la Person dans le cache
//               return Results.Ok(p);
//          }
//     }
//});
#endregion



#region avec output cache (automatique)
// Avec Output cache automatique ----------------------------------------------------------------------------------------------------------------------------------------------------------------------
//app.MapGet("/person", [OutputCache(PolicyName = "Expire2Min")] async
//(
//     [FromServices] ApiDbContext context
//) =>
//{

//     List<Person> lp = await context.PersonTable.ToListAsync();

//     return Results.Ok(lp);
//})
//.CacheOutput("Expire2Min");



//app.MapGet("/person/{id:int}", [OutputCache(PolicyName = "ById")] async
//(
//     [FromRoute]    int            id,

//     [FromServices] ApiDbContext   context) =>
//{

//     var p = await context.PersonTable.Where(w => w.Id == id).FirstOrDefaultAsync();

//     if (p is null)
//     {
//          return Results.NotFound();
//     }
//     else
//     {
//          return Results.Ok(p);
//     }
//})
//.CacheOutput("ById");
#endregion



#region avec distributed memory cache
// Avec Distributed Memory Cache ----------------------------------------------------------------------------------------------------------------------------------------------------------------------
//app.MapGet("/person/{id:int}", async
//(
//     [FromRoute]    int                 id,

//     [FromServices] IDistributedCache   cache,
//     [FromServices] ApiDbContext        context
//) =>
//{

//     //await cache.GetAsync<Person>($"personne_{id}", out var p);                                // le "out var p" est incompatible avec le "await" (l'approche asynchrone) dans la version 9.0 de .NET, donc on ne peut pas l'utiliser ici !
//     var p = await cache.GetAsync<Person>($"personne_{id}");                                   // J'essaie en premier de récupérer la Person à partir du cache

//     if (p is null)
//     {
//          p = await context.PersonTable.Where(w => w.Id == id).FirstOrDefaultAsync();         // Sinon j'essaie à partir de la base de données

//          if (p is null)
//          {
//               return Results.NotFound();
//          }
//          else
//          {
//               await cache.SetAsync($"personne_{id}", p);                                      // Je stocke la Person dans le cache
//               return Results.Ok(p);
//          }
//     }
//     else
//     {
//          return Results.Ok(p);
//     }
//});
#endregion



#region avec injection de dépendance de services
// Avec Injection de dépendance de services -----------------------------------------------------------------------------------------------------------------------------------------------------------
app.MapGet("/person", async
(
     //[FromServices] IDistributedCache cache,
     [FromServices] IPersonService iPersonService                                              // Utilisation de l'interface 'IPersonService' pour récupérer les données de la base de données ou du cache remplaçant l'utilisation direct de "ApiDbContext" dans les Endpoints
) =>
{

     //var lp = await cache.GetAsync<PersonOutputModel>($"personne_{id}");                     // J'essaie en premier de récupérer la Person à partir du cache

     //if (lp is null)
     //{
     List<PersonOutputModel> lp = await iPersonService.GetAll();                               // Sinon j'essaie à partir de la base de données

     if (lp is null)
     {
          return Results.NotFound();
     }
     else
     {
          //await cache.SetAsync($"personne_{id}", lp);                                        // Je stocke la Person dans le cache
          return Results.Ok(lp);
     }
     //}
     //else
     //{
     //     return Results.Ok(lp);
     //}
})
.WithTags("PersonManagement8888");                                                             // Permet de regrouper les Endpoints par 'Tag' dans Swagger



app.MapGet("/person/{id:int}", async
(
     [FromRoute] int id,

     //[FromServices] IDistributedCache cache,
     [FromServices] IPersonService iPersonService                                              // Utilisation de l'interface 'IPersonService' pour récupérer les données de la base de données ou du cache remplaçant l'utilisation direct de "ApiDbContext" dans les Endpoints
) =>
{

     //var p = await cache.GetAsync<PersonOutputModel>($"personne_{id}");                      // J'essaie en premier de récupérer la Person à partir du cache

     //if (p is null)
     //{
     PersonOutputModel? p = await iPersonService.GetById(id);                                  // Sinon j'essaie à partir de la base de données

     if (p is null)
     {
          return Results.NotFound();
     }
     else
     {
          //await cache.SetAsync($"personne_{id}", p);                                         // Je stocke la Person dans le cache
          return Results.Ok(p);
     }
     //}
     //else
     //{
     //     return Results.Ok(p);
     //}
})
.WithTags("PersonManagement");                                                                 // Permet de regrouper les Endpoints par 'Tag' dans Swagger
#endregion





// ====================================================================================================================================================================================================
//                                                                                                                                                                                                    !
//                                                                                         Endpoints POST                                                                                             !
//                                                                                                                                                                                                    !
// ====================================================================================================================================================================================================
#region validation 'à la main'
// Validation 'à la main' ---------------------------------------------------------------------------------------------------------------------------
//app.MapPost("/person",
//(
//     [FromBody] Person p
//) =>
//{

//     if (string.IsNullOrWhiteSpace(p.FirstName))  return Results.BadRequest("Le prénom est requis");
//     if (string.IsNullOrWhiteSpace(p.LastName))   return Results.BadRequest("Le nom est requis");

//     return Results.Ok(p);
//});
#endregion



#region validation avec FluentValidation
// Validation avec FluentValidation -----------------------------------------------------------------------------------------------------------------
//app.MapPost("/person",
//(
//     [FromBody]          PersonnInputModel                  p,
//     [FromServices]      IValidator<PersonnInputModel>      validator
//) =>
//{

//     //var result = validator.Validate(p);
//     FluentValidation.Results.ValidationResult result = validator.Validate(p);

//     if (!result.IsValid)
//     {

//          //     return Results.BadRequest(result.Errors);                                                                                            // Pour retourner l'erreur complète. Adaptée à une API privée ou interne
//          //};
//          return Results.BadRequest(result.Errors.Select(e => new                                                                                     // Pour retourner une erreur plus simple, adaptée à une API publique ou externe
//          {

//               Message = e.ErrorMessage,
//               //PropertyName = e.PropertyName
//               e.PropertyName
//          }));
//     }

//     return Results.Ok(p);
//});
#endregion



#region avec une base de données
// Avec utilisation d'une base de données -------------------------------------------------------------------------------------------------------------------------------------------------------------
//app.MapPost("/person", async
//(
//     [FromBody]     Person                   p,

//     [FromServices] IValidator<Person>       validator,
//     [FromServices] ApiDbContext             context,
//     //[FromServices] IDistributedCache        cache,

//     [FromServices] CancellationToken        token
//) =>
//{

//     FluentValidation.Results.ValidationResult result = validator.Validate(p);

//     if (!result.IsValid)
//     {
//          return Results.BadRequest(result.Errors.Select(e => new
//          {
//               Message        = e.ErrorMessage,
//               PropertyName   = e.PropertyName,
//               Severity       = e.Severity
//          }));
//     }
//     else
//     {
//          await context.PersonTable.AddAsync(p, token); ;
//          await context.SaveChangesAsync(token);

//          //await cache.SetAsync($"personne_{p.Id}", p);                                 // Je stocke la Person dans le cache
//          return Results.Ok(p);
//     }
//});
#endregion



#region avec memory cache (manuel)
// Avec utilisation d'une base de données -------------------------------------------------------------------------------------------------------------------------------------------------------------
app.MapPost("/person", async
(
     [FromBody] Person p,

     [FromServices] IValidator<Person> validator,
     [FromServices] ApiDbContext context,
     [FromServices] IMemoryCache cache,

                    CancellationToken token
) =>
{

     FluentValidation.Results.ValidationResult result = validator.Validate(p);

     if (!result.IsValid)
     {
          return Results.BadRequest(result.Errors.Select(e => new
          {
               Message = e.ErrorMessage,
               PropertyName = e.PropertyName,
               Severity = e.Severity
          }));
     }
     else
     {
          await context.PersonTable.AddAsync(p, token); ;
          await context.SaveChangesAsync(token);

          cache.Set($"personne_{p.Id}", p);                                                    // Je stocke la Person dans le cache
          return Results.Ok(p);
     }
});
#endregion



#region avec un cache distribué
// Avec utilisation d'un cache distribué --------------------------------------------------------------------------------------------------------------------------------------------------------------
//app.MapPost("/person", async
//(
//     [FromBody] Person p,

//     [FromServices] IValidator<Person>  validator,
//     [FromServices] ApiDbContext        context,
//     [FromServices] IDistributedCache   cache,

//                    CancellationToken   token
//) =>
//{

//     FluentValidation.Results.ValidationResult result = validator.Validate(p);

//     if (!result.IsValid)
//     {
//          return Results.BadRequest(result.Errors.Select(e => new
//          {
//               Message        = e.ErrorMessage,
//               PropertyName   = e.PropertyName,
//               Severity       = e.Severity
//          }));
//     }
//     else
//     {
//          await context.PersonTable.AddAsync(p, token); ;
//          await context.SaveChangesAsync(token);

//          //await cache.SetAsync($"personne_{p.Id}", p);                                 // Je stocke la Person dans le cache
//          return Results.Ok(p);
//     }
//});
#endregion



#region Injection de dépendance de services
// Avec Injection de dépendance de services -----------------------------------------------------------------------------------------------------------------------------------------------------------
app.MapPost("/person", async
(
     [FromBody]     PersonnInputModel                  p,

     [FromServices] IValidator<PersonnInputModel>      validator,
     //[FromServices] IDistributedCache cache,
     [FromServices] IPersonService                     iPersonService,                              // Utilisation de l'interface 'IPersonService' pour récupérer les données de la base de données ou du cache remplaçant l'utilisation direct de "ApiDbContext" dans les Endpoints

     CancellationToken token
) =>
{
     var result = validator.Validate(p);

     if (!result.IsValid)
     {
          return Results.BadRequest(result.Errors.Select(e => new
          {
               Message        = e.ErrorMessage,
               PropertyName   = e.PropertyName,
               Severity       = e.Severity
          }));
     }
     else
     {
          await iPersonService.Add(p);

          //await cache.SetAsync($"personne_{p.Id}", p);                                            // Je stocke la Person dans le cache
          return Results.Ok(p);
     }
})
.Accepts<PersonnInputModel>(contentType: "application/json")                                        // Permet d'indiquer que l'Endpoint accepte un 'PersonnInputModel'   avec un 'Media Type' (dans Swagger) de type json : 'application/json' (voir la partie "Request body" dand Swagger
.Produces<PersonOutputModel>(contentType: "application/json")                                       // Permet d'indiquer que l'Endpoint retourne un 'PersonnOutputModel' avec un 'Media Type' (dans Swagger) de type json : 'application/json' (voir la partie "Responses" dand Swagger
.WithTags("PersonManagement");                                                                      // Permet de regrouper les Endpoints par 'Tag' dans Swagger
#endregion





// ====================================================================================================================================================================================================
//                                                                                                                                                                                                    !
////                                                                                         Endpoints PUT                                                                                            !
//                                                                                                                                                                                                    !
// ====================================================================================================================================================================================================
#region En base de données - Approche historique et moderne
// Approche historique : nous récupérons l'object en mémoire avant de le modifier puis de le renvoyer à la base de données
//app.MapPut("/person/{id:int}",
//(
//     [FromRoute]         int            id,

//     [FromBody]          Person         p,

//     [FromServices]      ApiDbContext   context
//) =>
//{

//     var result = context.PersonTable.Where(w => w.Id == id).FirstOrDefault();


//     if (result is not null)
//     {

//          result.FirstName    = result.FirstName;
//          result.LastName     = p.LastName;

//          context.PersonTable.Update(p);                                                               // Optionnel. Je le mets comme parachute de secours pour remédier à une défaillance quelconque du Change tracker (si jamais il n'a pas été mis à jour par le Change Tracker d'Entity Framework Core)
//          context.SaveChanges();

//          return Results.Ok(p);
//     }
//     else
//     {

//          return Results.NotFound();
//     }
//});

// ou
//
// ou Approche "moderne" dite "bulk" -> on ne fait plus de select avant de faire le delete !
//(donc plus performant car l'objet n'est plus récupéré en mémoire avant sa suppression
//    ~ équivalent à une procédure stockée)
//app.MapPut("/person/{id:int}", async
//(
//     [FromRoute]    int                 id,

//     [FromBody]     Person              p,

//     [FromServices] ApiDbContext        context,

//                    CancellationToken   token
//) =>
//{

//     var result = await context.PersonTable
//                    .Where(w => w.Id == id)
//                    .ExecuteUpdateAsync(per => per.SetProperty(pers => pers.LastName, p.LastName)             // ExecuteUpdateAsync est une méthode d'Entity Framework Core qui permet de mettre à jour en masse sans charger les entités en mémoire
//                                                  .SetProperty(pers => pers.FirstName, p.FirstName), token);



//     if (result > 0) return Results.NoContent();

//     return Results.NotFound();
//});
#endregion



#region avec memory cache (manuel)
// Avec memory cache manuel -------------------------------------------------------------------------------------------------------------------------
app.MapPut("/person/{id:int}", async
(
     [FromRoute]    int                 id,

     [FromBody]     Person              p,

     [FromServices] ApiDbContext        context,
     [FromServices] IMemoryCache        cache,

                    CancellationToken   token
) =>
{

     var result = await context.PersonTable
                    .Where(w => w.Id == id)
                    .ExecuteUpdateAsync(per => per.SetProperty(pers => pers.LastName, p.LastName)
                                                  .SetProperty(pers => pers.FirstName, p.FirstName), token);

     if (result > 0)
     {

          cache.Remove($"personne_{id}");         // Suppresion de cette clef du cache (préférable à une mise à jour du cache car cela permet de ne pas avoir à gérer la cohérence du cache)
          return Results.NoContent();
     }
     else
     {

          return Results.NotFound();
     }
});
#endregion



#region avec output cache (automatique)
// Avec Output cache ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//app.MapPut("/person/{id:int}", async
//(
//     [FromRoute] int id,

//     [FromBody] Person p,

//     [FromServices] ApiDbContext        context,
//     [FromServices] IOutputCacheStore   cache
//) =>
//{

//     var result = await context.PersonTable
//     .Where(w => w.Id == id)
//     .ExecuteUpdateAsync(per => per.SetProperty(pers => pers.LastName, p.LastName)
//                                   .SetProperty(pers => pers.FirstName, p.FirstName));

//     if (result > 0)
//     {

//          await cache.EvictByTagAsync("ById", default);

//          return Results.NoContent();
//     }
//     else
//     {

//          return Results.NotFound();
//     }
//});
#endregion



#region avec distributed cache
// Avec Distributed Cache ---------------------------------------------------------------------------------------------------------------------------
//app.MapPut("/person/{id:int}", async
//(
//     [FromRoute]    int                 id,
//     [FromBody]     Person              p,
//     [FromServices] ApiDbContext        context,
//     [FromServices] IDistributedCache   cache
//) =>
//{
//     var result = await context.PersonTable
//          .Where(w => w.Id == id)
//          .ExecuteUpdateAsync(per => per.SetProperty(pers => pers.LastName,     p.LastName)
//                                        .SetProperty(pers => pers.FirstName,    p.FirstName));

//     if (result > 0)
//     {
//          await cache.RemoveAsync($"personne_{id}");
//          return Results.NoContent();
//     }
//     else
//     {
//          return Results.NotFound();
//     }
//});
#endregion



#region avec injection de dépendance de services
// Avec Injection de dépendance de services -----------------------------------------------------------------------------------------------------------------------------------------------------------
app.MapPut("/person/{id:int}", async
(
     [FromRoute]    int                 id,

     [FromBody]     PersonnInputModel   p,

     [FromServices] IPersonService      iPersonService,                                          // Utilisation de l'interface 'IPersonService' pour récupérer les données de la base de données ou du cache remplaçant l'utilisation direct de "ApiDbContext" dans les Endpoints
     [FromServices] IDistributedCache   cache) =>

{
     bool result = await iPersonService.Update(id, p);

     if (result)
     {
          await cache.RemoveAsync($"personne_{id}");
          return Results.NoContent();                                                          // 204
     }
     else
     {
          return Results.NotFound();                                                           // 404
     }
})
.Produces(204)                                                                                 // Permet d'indiquer dans Swagger que ce code de retour est utilisé (sinon il n'indiquera que le '200')
.Produces(404)                                                                                 // Permet d'indiquer dans Swagger que ce code de retour est utilisé (sinon il n'indiquera que le '200')
.WithTags("PersonManagement");                                                                 // Le tag permet de regrouper dans le swagger les Endpoint ayant une même logique
#endregion





// ====================================================================================================================================================================================================
//                                                                                                                                                                                                    !
////                                                                                       Endpoints DELETE                                                                                           !
//                                                                                                                                                                                                    !
// ====================================================================================================================================================================================================
#region En base de données - Approche historique et moderne
//// Approche historique : nous récupérons l'object en mémoire avant de le modifier puis de le renvoyer à la base de données
//app.MapDelete("/person/{id:int}", async
//(
//     [FromRoute] int id,

//     [FromServices] ApiDbContext c
//) =>
//{

//     var p = await c.PersonTable.Where(w => w.Id == id).FirstOrDefaultAsync();
//     if (p is not null)
//     {
//          c.PersonTable.Remove(p);
//          c.SaveChanges();

//          return Results.NoContent();
//     }
//     else
//     {
//          return Results.NotFound();
//     }
//});
//ou Approche "moderne" dite "bulk" -> on ne fait plus de select avant de faire le delete !
//   (donc plus performant car l'objet n'est plus récupéré en mémoire avant sa suppression
//   ~ équivalent à une procédure stockée)
//app.MapDelete("/person/{id:int}", async
//(
//     [FromRoute]    int                 id,

//     [FromServices] ApiDbContext        c,
//     [FromServices] IDistributedCache   cache,

//     [FromServices] CancellationToken   token
//) =>
//{

//     int result = await c.PersonTable.Where(w => w.Id == id).ExecuteDeleteAsync(token);

//     if (result > 0)
//     {
//          await cache.RemoveAsync($"personne_{id}", token);
//          return Results.NoContent();
//     }
//     else
//     {
//          return Results.NotFound();
//     }
//});
#endregion



#region avec memory cache (manuel)
//app.MapDelete("/person/{id:int}", async
//(
//     [FromRoute]    int                 id,

//     [FromServices] ApiDbContext        c,
//     [FromServices] IMemoryCache        cache,

//                    CancellationToken   token
//) =>
//{

//     int result = await c.PersonTable.Where(w => w.Id == id).ExecuteDeleteAsync(token);

//     if (result > 0)
//     {
//          cache.Remove($"personne_{id}");
//          return Results.NoContent();
//     }
//     else
//     {
//          return Results.NotFound();
//     }
//});
#endregion



#region avec injection de dépendance de services
// Avec Injection de dépendance de services -----------------------------------------------------------------------------------------------------------------------------------------------------------
app.MapDelete("/person/{id:int}", async
(
     [FromRoute]    int                 id,

     [FromServices] IPersonService      iPersonService,                                        // Utilisation de l'interface 'IPersonService' pour récupérer les données de la base de données ou du cache remplaçant l'utilisation direct de "ApiDbContext" dans les Endpoints
     [FromServices] IDistributedCache   cache
) =>

{
     var result = await iPersonService.Delete(id);

     if (result)
     {
          await cache.RemoveAsync($"personne_{id}");
          return Results.NoContent();
     }
     else
     {
          return Results.NotFound();
     }
})
.WithTags("PersonManagement");
#endregion




// ====================================================================================================================================================================================================
//                                                                                                                                                                                                    !
//                                                                                               Run                                                                                                  !
//                                                                                                                                                                                                    !
// ====================================================================================================================================================================================================
// Registering and configuring terminal middleware ----------------------------------------------------------------------------------------------------------------------------------------------------
app.Run();