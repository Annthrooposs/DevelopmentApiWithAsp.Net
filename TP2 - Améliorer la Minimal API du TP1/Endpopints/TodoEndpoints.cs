using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

using FluentValidation;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using TP2_AméliorerlaMinimalAPIduTP1.DTO;
using TP2_AméliorerlaMinimalAPIduTP1.Services;



namespace TP2_AméliorerlaMinimalAPIduTP1.Endpopints;

// prefix: "/todos"


// Cette classe permet de regrouper dans une classe externe à 'program.cs' tous 
//    - les services
//    - les endpoints
//    - les méthodes appelées par les Endpoints
// associés à une classe (ici 'Todo')

public static class TodoEndpoints
{
     // ===============================================================================================================================================================================================
     //                                                                                                                                                                                               !
     //                                                                                    Services des "Todo"                                                                                        !
     //                                                                                                                                                                                               !
     // ===============================================================================================================================================================================================
     // Injection de dépendance des services (remplaçant le "builder.Services.AddScoped<ITodoService, EFCoreTodoService>();" se trouvant dans le "program.cs" -----------------------------------------
     public static IServiceCollection MapTodoServices(this IServiceCollection p_services)
     {
          
          p_services.AddScoped<ITodoService, EFCoreTodoService>();                                       // précise l'interface "ITodoService" et son implémentation à utiliser "EFCoreTodoService"
          return p_services;
     }





     // ===============================================================================================================================================================================================
     //                                                                                                                                                                                               !
     //                                                                                    Endpoints des "Todo"                                                                                       !
     //                                                                                                                                                                                               !
     // ===============================================================================================================================================================================================
     public static RouteGroupBuilder MapTodoEndpoints(this RouteGroupBuilder p_group)
     {
          // ======================================================================================
          //                                                                                      !
          //                                     Endpoints GET                                    !
          //                                                                                      !
          // ======================================================================================
          // Avec Injection de dépendance de services ---------------------------------------------
          p_group.MapGet("", GetAll)                                                                     // Cette méthode GetAll se trouve plus bas dans cette même classe et nous n'utilisons plus directement celle dans la classe "EFCoreTodoService.cs"
               .WithTags("TodoManagement")                                                               // Permet de regrouper les Endpoints par 'Tag' dans Swagger
                         
               .WithName("GetAll")                                                                       // Nomme l'Endpoint
               
               .Produces<List<TodoOutputDto>>(200, "application/json")                                   // Permet d'indiquer que l'Endpoint retourne une liste de 'TodoOutputModel' avec un 'Media Type' (dans Swagger) de type json : 'application/json' (voir la partie "Responses" dand Swagger)
               .Produces(401);                                                                           // Permet d'indiquer dans Swagger que ce code de retour est utilisé (sinon il n'indiquera que le '200')



          // Avec Injection de dépendance de services ---------------------------------------------
          p_group.MapGet("/{id:int}", GetById)                                                           // Cette méthode GetById se trouve plus bas dans cette même classe et nous n'utilisons plus directement celle dans la classe "EFCoreTodoService.cs"
               .WithTags("TodoManagement")                                                               // Permet de regrouper les Endpoints par 'Tag' dans Swagger

               .WithName("GetById")                                                                      // Nomme l'Endpoint, permettant ainsi un lien pour le "linkgenerator" (voir dans la méthode POST ci-dessous et non dans le GETBYID)
                                                                                                         // Cela permet de récupérer l'URI de la ressource qui vient d'être créé par le POST et de l'inclure dans les informations de retour au code appelant                                                            // Nomme l'Endpoint
               .Produces<TodoOutputDto>(200, "application/json")                                         // Permet d'indiquer que l'Endpoint retourne une liste de 'TodoOutputModel' avec un 'Media Type' (dans Swagger) de type json : 'application/json' (voir la partie "Responses" dand Swagger)
               .ProducesValidationProblem(200);



          // Avec Injection de dépendance de services ---------------------------------------------
          p_group.MapGet("/{id:int}", GetActives)                                                        // Cette méthode GetActives se trouve plus bas dans cette même classe
               .WithTags("TodoManagement")                                                               // Permet de regrouper les Endpoints par 'Tag' dans Swagger

               .WithName("GetActives")
               
               .Produces<List<TodoOutputDto>>(200, "application/json")                                   // Permet d'indiquer que l'Endpoint retourne une liste de 'TodoOutputModel' avec un 'Media Type' (dans Swagger) de type json : 'application/json' (voir la partie "Responses" dand Swagger)
               .Produces(401);





          // ======================================================================================
          //                                                                                      !
          //                                     Endpoints POST                                   !
          //                                                                                      !
          // ======================================================================================
          // Avec Injection de dépendance de services ---------------------------------------------
          p_group.MapPost("", Post)                                                                      // Cette méthode Post se trouve plus bas dans cette même classe et nous n'utilisons plus directement celle dans la classe "EFCoreTodoService.cs"
               .Accepts<TodoInputDto>(contentType: "application/json")                                   // Permet d'indiquer que l'Endpoint accepte un 'TodonInputModel'   avec un 'Media Type' (dans Swagger) de type json : 'application/json' (voir la partie "Request body" dand Swagger
               .Produces<TodoOutputDto>(contentType: "application/json")                                 // Permet d'indiquer que l'Endpoint retourne un 'TodonOutputModel' avec un 'Media Type' (dans Swagger) de type json : 'application/json' (voir la partie "Responses" dand Swagger
               .WithTags("TodoManagement")                                                               // Permet de regrouper les Endpoints par 'Tag' dans Swagger
                         
               .WithName("Post")                                                                         // Nomme l'Endpoint

               .Produces(400)
               .Produces(401);                                                                           // Permet d'indiquer dans Swagger que ces codes de retour sont utilisés (sinon il n'indiquera que le '200')





          // ======================================================================================
          //                                                                                      !
          //                                     Endpoints PUT                                    !
          //                                                                                      !
          // ======================================================================================
          // Avec Injection de dépendance de services ---------------------------------------------
          p_group.MapPut("/{id:int}", Put)                                                               // Cette méthode Put se trouve plus bas dans cette même classe et nous n'utilisons plus directement celle dans la classe "EFCoreTodoService.cs"
               .Produces(204)                                                                            // Permet d'indiquer dans Swagger que ce code de retour est utilisé (sinon il n'indiquera que le '200')
               .Produces(404)                                                                            // Permet d'indiquer dans Swagger que ce code de retour est utilisé (sinon il n'indiquera que le '200')
               .WithTags("TodoManagement")                                                               // Permet de regrouper les Endpoints par 'Tag' dans Swagger
                         
               .WithName("Put")                                                                          // Nomme l'Endpoint

               .Produces(204)
               .Produces(400)
               .Produces(401);                                                                           // Permet d'indiquer dans Swagger que ces codes de retour sont utilisés (sinon il n'indiquera que le '200')





          // ======================================================================================
          //                                                                                      !
          //                                   Endpoints DELETE                                   !
          //                                                                                      !
          // ======================================================================================
          // Avec Injection de dépendance de services ---------------------------------------------
          p_group.MapDelete("/{id:int}", Delete)                                                         // Cette méthode Delete se trouve plus bas dans cette même classe et nous n'utilisons plus directement celle dans la classe "EFCoreTodoService.cs"
               .WithTags("TodoManagement")                                                               // Permet de regrouper les Endpoints par 'Tag' dans Swagger

               .WithName("Delete")                                                                      // Nomme l'Endpoint

               .Produces(204)
               .Produces(401);





          return p_group;
     }





     // ===============================================================================================================================================================================================
     //                                                                                                                                                                                               !
     //                                                                                     Méthodes des "Todo"                                                                                       !
     //                                                                                                                                                                                               !
     // ===============================================================================================================================================================================================
     // ======================================================================================
     //                                                                                      !
     //                                      Methods GET                                     !
     //                                                                                      !
     // ======================================================================================
     private static async Task<IResult> GetAll
     (
          [FromServices] ITodoService ITodoService
     )
     {
          var p = await ITodoService.GetAll();
          return Results.Ok(p);
     }



     private static async Task<IResult> GetById
     (
               [FromRoute]    int                 id,

               [FromServices] IDistributedCache   cache,
               [FromServices] ITodoService        ITodoService
     )
     {

               var p = await cache.GetAsync<TodoOutputDto>($"todo_{id}");                    // J'essaie en premier de récupérer la Todo à partir du cache

               if (p is null)
               {
                    p = await ITodoService.GetById(id);                                        // Sinon j'essaie à partir de la base de données

                    if (p is null)
                    {
                         return Results.NotFound();
                    }
                    else
                    {
                         await cache.SetAsync($"todo_{id}", p);                                // Je stocke la Todo dans le cache
                         return Results.Ok(p);
                    }
               }
               else
               {
                    return Results.Ok(p);
               }
     }



     private static async Task<IResult> GetActives
     (
          [FromServices] ITodoService ITodoService
     )
     {
          var p = await ITodoService.GetAll().Where(testc => testc.EndDate is null);
          return Results.Ok(p);
     }





     // ======================================================================================
     //                                                                                      !
     //                                       Methods POST                                   !
     //                                                                                      !
     // ======================================================================================
     private static async Task<IResult> Post
     (
               [FromBody]     TodoInputDto                  tdim,

               [FromServices] IValidator<TodoInputDto>      validator,
               [FromServices] ITodoService                  ITodoService,
               [FromServices] IDistributedCache             cache,
               [FromServices] LinkGenerator                 linkGenerator, // Permet de générer des liens vers les Endpoints nommés et permet de récupérer l'URI de la ressource qui vient d'être créé (voir ci-dessous "var uri = linkGenerator.GetUriByName(httpContext, "GetById", new { id = dbResult.Id }); ")

                              HttpContext                   httpContext,   // Permet de récupérer le nom du domaine (le nom du serveur et le port, ex : "https://localhost:5001") pour générer l'URI complète de la ressource qui vient d'être créé (voir ci-dessous "var uri = linkGenerator.GetUriByName(httpContext, "GetById", new { id = dbResult.Id }); ")
                              CancellationToken             token
     )
     {

               var result = validator.Validate(tdim);

               if (!result.IsValid)
               {
                    return Results.BadRequest(result.Errors.Select(e => new
                    {
                         Message   =    e.ErrorMessage,
                                        e.PropertyName,
                                        e.Severity
                    }));
               }
               else
               {
                    TodoOutputDto? dbResult = await ITodoService.Add(tdim);

               //return Results.Ok(p);                                                                                            // Retourne simplement un code '200 OK'



               //// Sans le nom du domaine, donc fournit un chemin relatif (dans l'attribut 'Location' du Header de retour )
               //// -> ATTENTION : c'est le chemin et non l'URI (voir ci-dessous) qu'il faut fournir si l'API est derrière une passerelle dont le rôle est de distribuer la requête au serveur qui est le moins utilisé. La passerelle serait alors le nom de Domaine unique qui serait alors complété par le chemin relatif de l'API (GetById) (CAS 1)


               // CAS 1 - Chemin relatif
               // ----------------------
               // Fourni en retour (dans l'attribut 'Location' du Header de retour ) l'URI relative (chemin relatif) de l'API (GetById) qui permet de consulter l'enregistrement créé
               //var path = linkGenerator.GetPathByName("GetById", new { id = dbResult.Id });                                     // Le 'GetById' désigne l'Endpoint nommé 'GetById' grâce à son instruction '.WithName("GetById");'
               //return Results.Created(path, dbResult);



               // CAS  - Uri absolue
               // ------------------
               // Fourni en retour (dans l'attribut 'Location' du Header de retour ) l'URI complète (chemin absolu) de l'API (GetById) qui permet de consulter l'enregistrement créé
               var uri = linkGenerator.GetUriByName(httpContext, "GetById", new { id = dbResult.Id });                            // Le 'GetById' désigne l'Endpoint nommé 'GetById' grâce à son instruction '.WithName("GetById");'
               // Le 'httpContext'                permet de récupérer le nom du domaine (le nom du serveur et le port, ex : "https://localhost:5001")
               // "GetById"                       permet de récupérer "/todo", qui est le chemin de l'API (GetById) (ex : "/todo")
               // Le 'new { id = dbResult.Id }'   permet de fournir les paramètres de la route (ici l'id de la Todo. ex : "/1")
               // ce qui donne "https://localhost:5001/todo/1"

               return Results.Created(uri, dbResult);                                                                             // Retourne un code '201 Created' avec l'URI complète (la route) de l'API (GetById) qui permet de consulter l'enregistrement créé
          }
     }





     // ======================================================================================
     //                                                                                      !
     //                                      Methods PUT                                     !
     //                                                                                      !
     // ======================================================================================
     private static async Task<IResult> Put
     (
               [FromRoute]    int                 id,

               [FromBody]     TodoInputDto        tdim,

               [FromServices] ITodoService        ITodoService,
               [FromServices] IDistributedCache   cache
     )
     {

               var result = await ITodoService.Update(id, tdim);

               if (result)
               {
                    
                    await cache.RemoveAsync($"todo_{id}");
                    return Results.NoContent();
               }
               else
               {
                    
                    return Results.NotFound();
               }
     }





     // ======================================================================================
     //                                                                                      !
     //                                    Methods DELETE                                    !
     //                                                                                      !
     // ======================================================================================
     private static async Task<IResult> Delete
     (
               [FromRoute]    int                 id,

               [FromServices] ITodoService      ITodoService,
               [FromServices] IDistributedCache   cache
     )
     {

               var result = await ITodoService.Delete(id);

               if (result)
               {
                    
                    await cache.RemoveAsync($"todo_{id}");
                    return Results.NoContent();
               }
               else
               {
                    
                    return Results.NotFound();
               }
     }
}
