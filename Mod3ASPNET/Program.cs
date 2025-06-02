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
builder.Services.AddEndpointsApiExplorer();                                                                                                                                                             // Service Microsoft permettant d'explorer les diff�rents Enpoints pour fournir l'information � Swagger
builder.Services.AddSwaggerGen();                                                                                                                                                                       // Service utilisant le package Swagger

builder.Services
     .AddApiVersioning()                                                                                                                                                                                // Service de versioning utilis� par le package Nuget "Asp.Versioning.Http"
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
var versionSet = app.NewApiVersionSet()                                                                                                                                                                 // M�thode d'extension en mode Fluent
     .HasApiVersion(new Asp.Versioning.ApiVersion(1.0))
     .HasApiVersion(new Asp.Versioning.ApiVersion(2.0))
     .ReportApiVersions()
     .Build();





//// ====================================================================================================================================================================================================
////                                                                                                                                                                                                    !
////                                                               Appel des Middlewares personalis�s (app.Use... ou app.CustomMiddleware(...))                                                         !
////                                                                                                                                                                                                    !
////                              Note : un Middleware est toujours ex�cut� par Asp.Net AVANT les Endpoints, ce qui permet d'intercepter les requ�tes                                                   !
////                              et de les travailler en amont puis en aval, si on le souhaite, de la production de la r�ponse � la requ�te                                                            !
////                                                                                                                                                                                                    !
//// ====================================================================================================================================================================================================
//app.Use(async (HttpContext http, RequestDelegate next) =>
//{
//     //await http.Response.WriteAsync("Mon Middleware");
//     await next(http);                                                                                                                                                                                // Appel � une instance de RequestDelegate 
//});



//app.Use(async (HttpContext http, RequestDelegate next) =>
//{
//     await http.Response.WriteAsync("Mon Middleware 2");
//     //await next(http);
//});




// ====================================================================================================================================================================================================
//                                                                                                                                                                                                    !
//                                                                                   plac� ici temporairement                                                                                         !
//                                                                                                                                                                                                    !
// ====================================================================================================================================================================================================
// MapGET ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//app.MapGet("/v1/info", () => "Info v1")                                                                                                                                                               // Versioning "� la main"
//     .AddEndpointFilter<LoggingFilter>();
//app.MapGet("/v2/info", () => "Info v2")                                                                                                                                                               // Versioning "� la main"
//     .AddEndpointFilter<LoggingFilter>();




app.MapGet("{version:apiVersion}/info", () => "Info v1")
     .WithApiVersionSet(versionSet)
     .MapToApiVersion(new Asp.Versioning.ApiVersion(1.0));                                                                                                                                              // le num�ro de version est fourni dans la requ�te sous la forme "?api-version=1.0" apr�s la requ�te "http://localhost:5555/info"
                                                                                                                                                                                                        // Le chemin est donc le m�me, nous ajoutons simplement une clef dans la requ�te indiquant la version � utiliser

app.MapGet("v{version:apiVersion}/info", () => "Info v2")
     .WithApiVersionSet(versionSet)
     .MapToApiVersion(new Asp.Versioning.ApiVersion(2.0));                                                                                                                                              // le num�ro de version est fourni dans la requ�te sous la forme "?api-version=2.0" apr�s la requ�te "http://localhost:5555/info"
                                                                                                                                                                                                        // Le chemin est donc le m�me, nous ajoutons simplement une clef dans la requ�te indiquant la version � utiliser



// Personaliser les r�sultats d'API en cr�ant nos propres types de retour
app.MapGet("v{version:apiVersion}/hello-html", () =>
     {

          // "HtmlResult" est un type de retour que nous avons cr�� par nos propres soins sous forme de classe pour retourner du texte devant �tre interpr�t� comme du HTML et non comme du texte brut !
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
// T�l�chargement (Upload) d'une donn�e non associ�e � une classe c#, ici une image et donc une donn�e binaire
// Cette premi�re solution est la plus compliqu�e mais la plus performante !
app.MapPost("v{version:apiVersion}/file", async (HttpContext httpContext) =>
     {
          // Il va s'agir de :
          // ---------------
          // 1) r�cup�rer le Stream du Body (qui est un Stream r�seau)
          // 2) pour le copier dans un Stream en m�moire
          // 3) puis transformer ce Stream en m�moire en tableau de bytes 


          // Cr�ation du Stream m�moire
          using MemoryStream? memoryStream = new MemoryStream();

          // Copie du Stream du Body dans le Stream m�moire
          await httpContext.Request.Body.CopyToAsync(memoryStream);
          memoryStream.Position = 0;                                                                                                                                                                    // Apr�s la copie, le pointeur du Stream se trouve � la fin, il faut donc le repositionner au d�but
          //memoryStream.Seek(0, SeekOrigin.Begin);                                                                                                                                                     // Fait la m�me chose

          // Transforme ce Stream en m�moire en tableau de bytes
          File.WriteAllBytes("monnomdefichier.png", memoryStream.ToArray());                                                                                                                            // Ici, nous pourrions choisir de nommer le fichier en fonction d'un param�tre pass� dans la requ�te (par exemple un param�tre "filename" pass� dans la requ�te) et de r�cup�rer l'extension du fichier dans le Request.Body.ContentType de l'API (ex. : png)   
          var fileExtension = httpContext.Request.ContentType;                                                                                                                                          // Si nous ne connaissons pas le nom du fichier et son extension, il est n�cessaire d'aller voir dans la rubrique "Request.Body. ContentType" du Body"
                                                                                                                                                                                                        // Nous obtenons ainsi dans notre cas : "image/png". Il ne reste plus qu'� extraire l'extension pour sauvegarder le fichier en lui donnant
                                                                                                                                                                                                        // le nom que nous souhaitons auquels nous collons l'extension r�cup�r�e
                                                                                                                                                                                                        // var x = httpContext.Request.Content.Headers.GetValues("Content-Type").First();         // Quelqu'un a propos� !?!


          return Results.Ok();
     })
     //.AddEndpointFilter(async (context, next) =>                                                                                                                                                      // Ici, nous ne nous emb�tons pas ! Nous utilisons un Filter en mode Inline...beurk. Il vaut mieux cr�er un nouveau "Filters" dans lequel 
     // {
     //      var logger = context.HttpContext.RequestServices.GetService<ILogger<Program>>();
     //      logger.LogInformation("Nous commen�ons l'Endpoint POST avec la route /file");

     //      var result = await next(context);

     //      logger.LogInformation("Nous finissons l'Endpoint POST avec la route /file");

     //      return result;
     // });
     .AddEndpointFilter<LoggingFilter>()                                                                                                                                                                // Ici, nous ne nous emb�tons pas ! Nous utilisons un Filter en mode Inline...beurk. Il vaut mieux cr�er un nouveau "Filters" dans lequel
     .WithApiVersionSet(versionSet)
     .MapToApiVersion(new Asp.Versioning.ApiVersion(1.0));





//// T�l�chargement (Upload) d'une donn�e non associ�e � une classe c#, ici une image et donc une donn�e binaire ----------------------------------------------------------------------------------------
//// Cette deuxi�me solution est plus simple car elle utilise la repr�sentation d'un fichier contenu dans un formulaire...mais moins performante car plus de d�coration et donc plus de trafic r�seau
//// (m�me repr�sentation qu'en ASP.NET MVC ou RAZOR PAGE lorsque l'utilisateur fait un envoi de formulaire avec un fichier dans un Input de type File)
//app.MapPost("v{version:apiVersion}/formfile", async (IFormFile myFile) =>
//{
//     // Il va s'agir de :
//     // ---------------
//     // 1) r�cup�rer le Stream du Body (qui est un Stream r�seau)
//     // 2) pour le copier dans un Stream en m�moire
//     // 3) puis transformer ce Stream en m�moire en tableau de bytes 



//     // Cr�ation du Stream m�moire
//     using MemoryStream? memoryStream = new MemoryStream();



//     // Copie du Stream du Body dans le Stream m�moire
//     //using var filestream = myFile.OpenReadStream();
//     //await filestream.CopyToAsync(memoryStream);

//     // ou plus directement

//     await myFile.CopyToAsync(memoryStream);



//     // Apr�s la copie, le pointeur du du Stream se trouve � la fin, il faut donc le repositionner au d�but
//     memoryStream.Seek(0, SeekOrigin.Begin);


//     // Transforme ce Stream en m�moire en tableau de bytes
//     File.WriteAllBytes(myFile.FileName, memoryStream.ToArray());


//     return Results.Ok();
//})
//     .AddEndpointFilter(async (context, next) =>
//     {
//          var logger = context.HttpContext.RequestServices.GetService<ILogger<Program>>();
//          logger.LogInformation("Nous commen�ons l'Endpoint POST avec la route /file");

//          var result = await next(context);

//          logger.LogInformation("Nous finissons l'Endpoint POST avec la route /file");

//          return result;
//     })
//.WithApiVersionSet(versionSet)
//.MapToApiVersion(new Asp.Versioning.ApiVersion(1.0));





// ====================================================================================================================================================================================================
//                                                                                                                                                                                                    !
//                                        G�n�ration du fichier swagger.json et de l'interface graphique exposant le fichier avec les diff�rentes versions                                            !
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
