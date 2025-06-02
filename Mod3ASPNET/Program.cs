using Microsoft.AspNetCore.Http.HttpResults;
using System.Net.Security;
using Asp.Versioning;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

using Mod3ASPNET;
using Mod3ASPNET.Filters;
using Mod3ASPNET.Doc;



// ====================================================================================================================================================================================================
//                                                                                                                                                                                                    !
//                                                                                         Construction                                                                                               !
//                                                                                                                                                                                                    !
// ====================================================================================================================================================================================================
// Creating the WebApplicationBuilder factory class ---------------------------------------------------------------------------------------------------------------------------------------------------
var builder = WebApplication.CreateBuilder(args);



// Registering and configuring Logger -----------------------------------------------------------------------------------------------------------------------------------------------------------------



// Registering services within the container (Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle) ------------------------------------------------------------------
builder.Services.AddEndpointsApiExplorer();                                                                                                                                                             // Service Microsoft permettant d'explorer les différents Enpoints pour fournir l'information à Swagger
builder.Services.AddSwaggerGen();                                                                                                                                                                       // Service utilisant le package Swagger

builder.Services
     .AddApiVersioning()                                                                                                                                                                                // Service de versioning utilisé par le package Nuget "Asp.Versioning.Http"
     .AddApiExplorer(opt =>
     {
          opt.GroupNameFormat           = "'v'VVV";                                                                                                                                                    // Voir 'https://github.com/dotnet/aspnet-api-versioning/wiki/Version-Format'
          opt.SubstituteApiVersionInUrl = true;
     })
     .EnableApiVersionBinding();

builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerGenOptions>();

// Registering middlewares ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------



// Registering validators -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------



// Distributed Cache ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------



// Registering the database context -------------------------------------------------------------------------------------------------------------------------------------------------------------------



// Building the WebApplication object -----------------------------------------------------------------------------------------------------------------------------------------------------------------
var app = builder.Build();



// Vesioning ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
var versionSet = app.NewApiVersionSet()                                                                                                                                                                 // Méthode d'extension en mode Fluent
     .HasApiVersion(new Asp.Versioning.ApiVersion(1.0))
     .HasApiVersion(new Asp.Versioning.ApiVersion(2.0))
     .ReportApiVersions()
     .Build();





//// ====================================================================================================================================================================================================
////                                                                                                                                                                                                    !
////                                                               Appel des Middlewares personalisés (app.Use... ou app.CustomMiddleware(...))                                                         !
////                                                                                                                                                                                                    !
////                              Note : un Middleware est toujours exécuté par Asp.Net AVANT les Endpoints, ce qui permet d'intercepter les requêtes                                                   !
////                              et de les travailler en amont puis en aval, si on le souhaite, de la production de la réponse à la requête                                                            !
////                                                                                                                                                                                                    !
//// ====================================================================================================================================================================================================
//app.Use(async (HttpContext http, RequestDelegate next) =>
//{
//     //await http.Response.WriteAsync("Mon Middleware");
//     await next(http);                                                                                                                                                                                // Appel à une instance de RequestDelegate 
//});



//app.Use(async (HttpContext http, RequestDelegate next) =>
//{
//     await http.Response.WriteAsync("Mon Middleware 2");
//     //await next(http);
//});




// ====================================================================================================================================================================================================
//                                                                                                                                                                                                    !
//                                                                                   placé ici temporairement                                                                                         !
//                                                                                                                                                                                                    !
// ====================================================================================================================================================================================================
// MapGET ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//app.MapGet("/v1/info", () => "Info v1")                                                                                                                                                               // Versioning "à la main"
//     .AddEndpointFilter<LoggingFilter>();
//app.MapGet("/v2/info", () => "Info v2")                                                                                                                                                               // Versioning "à la main"
//     .AddEndpointFilter<LoggingFilter>();




app.MapGet("{version:apiVersion}/info", () => "Info v1")
     .WithApiVersionSet(versionSet)
     .MapToApiVersion(new Asp.Versioning.ApiVersion(1.0));                                                                                                                                              // le numéro de version est fourni dans la requête sous la forme "?api-version=1.0" après la requête "http://localhost:5555/info"
                                                                                                                                                                                                        // Le chemin est donc le même, nous ajoutons simplement une clef dans la requête indiquant la version à utiliser

app.MapGet("v{version:apiVersion}/info", () => "Info v2")
     .WithApiVersionSet(versionSet)
     .MapToApiVersion(new Asp.Versioning.ApiVersion(2.0));                                                                                                                                              // le numéro de version est fourni dans la requête sous la forme "?api-version=2.0" après la requête "http://localhost:5555/info"
                                                                                                                                                                                                        // Le chemin est donc le même, nous ajoutons simplement une clef dans la requête indiquant la version à utiliser



// Personaliser les résultats d'API en créant nos propres types de retour
app.MapGet("v{version:apiVersion}/hello-html", () =>
     {

          // "HtmlResult" est un type de retour que nous avons créé par nos propres soins sous forme de classe pour retourner du texte devant être interprété comme du HTML et non comme du texte brut !
          return new HtmlResult("""
               < html>
                    <body>
                         <hl>Hello world! depuis l'API en HTML !!</>
                    </body>
               </html>
               """);
     })
     .AddEndpointFilter<LoggingFilter>()
     .WithApiVersionSet(versionSet)
     .MapToApiVersion(new Asp.Versioning.ApiVersion(1.0));



// MapPOST --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
// Téléchargement (Upload) d'une donnée non associée à une classe c#, ici une image et donc une donnée binaire
// Cette première solution est la plus compliquée mais la plus performante !
app.MapPost("v{version:apiVersion}/file", async (HttpContext httpContext) =>
     {
          // Il va s'agir de :
          // ---------------
          // 1) récupérer le Stream du Body (qui est un Stream réseau)
          // 2) pour le copier dans un Stream en mémoire
          // 3) puis transformer ce Stream en mémoire en tableau de bytes 


          // Création du Stream mémoire
          using MemoryStream? memoryStream = new MemoryStream();

          // Copie du Stream du Body dans le Stream mémoire
          await httpContext.Request.Body.CopyToAsync(memoryStream);
          memoryStream.Position = 0;                                                                                                                                                                    // Après la copie, le pointeur du Stream se trouve à la fin, il faut donc le repositionner au début
          //memoryStream.Seek(0, SeekOrigin.Begin);                                                                                                                                                     // Fait la même chose

          // Transforme ce Stream en mémoire en tableau de bytes
          File.WriteAllBytes("monnomdefichier.png", memoryStream.ToArray());                                                                                                                            // Ici, nous pourrions choisir de nommer le fichier en fonction d'un paramètre passé dans la requête (par exemple un paramètre "filename" passé dans la requête) et de récupérer l'extension du fichier dans le Request.Body.ContentType de l'API (ex. : png)   
          var fileExtension = httpContext.Request.ContentType;                                                                                                                                          // Si nous ne connaissons pas le nom du fichier et son extension, il est nécessaire d'aller voir dans la rubrique "Request.Body. ContentType" du Body"
                                                                                                                                                                                                        // Nous obtenons ainsi dans notre cas : "image/png". Il ne reste plus qu'à extraire l'extension pour sauvegarder le fichier en lui donnant
                                                                                                                                                                                                        // le nom que nous souhaitons auquels nous collons l'extension récupérée
                                                                                                                                                                                                        // var x = httpContext.Request.Content.Headers.GetValues("Content-Type").First();         // Quelqu'un a proposé !?!


          return Results.Ok();
     })
     //.AddEndpointFilter(async (context, next) =>                                                                                                                                                      // Ici, nous ne nous embêtons pas ! Nous utilisons un Filter en mode Inline...beurk. Il vaut mieux créer un nouveau "Filters" dans lequel 
     // {
     //      var logger = context.HttpContext.RequestServices.GetService<ILogger<Program>>();
     //      logger.LogInformation("Nous commençons l'Endpoint POST avec la route /file");

     //      var result = await next(context);

     //      logger.LogInformation("Nous finissons l'Endpoint POST avec la route /file");

     //      return result;
     // });
     .AddEndpointFilter<LoggingFilter>()                                                                                                                                                                // Ici, nous ne nous embêtons pas ! Nous utilisons un Filter en mode Inline...beurk. Il vaut mieux créer un nouveau "Filters" dans lequel
     .WithApiVersionSet(versionSet)
     .MapToApiVersion(new Asp.Versioning.ApiVersion(1.0));





//// Téléchargement (Upload) d'une donnée non associée à une classe c#, ici une image et donc une donnée binaire ----------------------------------------------------------------------------------------
//// Cette deuxième solution est plus simple car elle utilise la représentation d'un fichier contenu dans un formulaire...mais moins performante car plus de décoration et donc plus de trafic réseau
//// (même représentation qu'en ASP.NET MVC ou RAZOR PAGE lorsque l'utilisateur fait un envoi de formulaire avec un fichier dans un Input de type File)
//app.MapPost("v{version:apiVersion}/formfile", async (IFormFile myFile) =>
//{
//     // Il va s'agir de :
//     // ---------------
//     // 1) récupérer le Stream du Body (qui est un Stream réseau)
//     // 2) pour le copier dans un Stream en mémoire
//     // 3) puis transformer ce Stream en mémoire en tableau de bytes 



//     // Création du Stream mémoire
//     using MemoryStream? memoryStream = new MemoryStream();



//     // Copie du Stream du Body dans le Stream mémoire
//     //using var filestream = myFile.OpenReadStream();
//     //await filestream.CopyToAsync(memoryStream);

//     // ou plus directement

//     await myFile.CopyToAsync(memoryStream);



//     // Après la copie, le pointeur du du Stream se trouve à la fin, il faut donc le repositionner au début
//     memoryStream.Seek(0, SeekOrigin.Begin);


//     // Transforme ce Stream en mémoire en tableau de bytes
//     File.WriteAllBytes(myFile.FileName, memoryStream.ToArray());


//     return Results.Ok();
//})
//     .AddEndpointFilter(async (context, next) =>
//     {
//          var logger = context.HttpContext.RequestServices.GetService<ILogger<Program>>();
//          logger.LogInformation("Nous commençons l'Endpoint POST avec la route /file");

//          var result = await next(context);

//          logger.LogInformation("Nous finissons l'Endpoint POST avec la route /file");

//          return result;
//     })
//.WithApiVersionSet(versionSet)
//.MapToApiVersion(new Asp.Versioning.ApiVersion(1.0));





// ====================================================================================================================================================================================================
//                                                                                                                                                                                                    !
//                                        Génération du fichier swagger.json et de l'interface graphique exposant le fichier avec les différentes versions                                            !
//                                                                                                                                                                                                    !
// ====================================================================================================================================================================================================
if (app.Environment.IsDevelopment())
{
     app.UseSwagger();
     app.UseSwaggerUI(conf =>
     {
          var versions = app.DescribeApiVersions();

          foreach (var item in versions)
          {
               var url = $"/swagger/{item.GroupName}/swagger.json";
               conf.SwaggerEndpoint(url, "Mon API " + item.GroupName);
          }
     });
}






// ====================================================================================================================================================================================================
//                                                                                                                                                                                                    !
//                                                                                               Run                                                                                                  !
//                                                                                                                                                                                                    !
// ====================================================================================================================================================================================================
// Registering and configuring terminal middleware ----------------------------------------------------------------------------------------------------------------------------------------------------
app.Run();
