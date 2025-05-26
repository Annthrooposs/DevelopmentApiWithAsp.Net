using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using MinimalAPIProfesional.DTO___Models;
using MinimalAPIProfesional.Services;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MinimalAPIProfesional.Endpoints
{
     // prefix: "/person"


     // Cette classe permet de regrouper dans une classe externe à 'program.cs' tous 
     //    - les services
     //    - les endpoints
     //    - les méthodes appelées par les Endpoints
     // associés à une classe (ici 'Person')

     public static class PersonEndpoints
     {
          // ==================================================================================================================================================
          //                                                                                                                                                  !
          //                                                             Services des "Person"                                                                !
          //                                                                                                                                                  !
          // ==================================================================================================================================================
          public static IServiceCollection MapPersonServices(this IServiceCollection p_services)
          {
               p_services.AddScoped<IPersonService, EFCorePersonService>();                         // précise l'interface "IPersonService" et son implémentation à utiliser "EFCorePersonService"
               return p_services;
          }





          // ==================================================================================================================================================
          //                                                                                                                                                  !
          //                                                            Endpoints des "Person"                                                                !
          //                                                                                                                                                  !
          // ==================================================================================================================================================
          public static RouteGroupBuilder MapPersonEndpoints(this RouteGroupBuilder p_group)
          {
               // =================================================================================
               //                                                                                 !
               //                                   Endpoints GET                                 !
               //                                                                                 !
               // =================================================================================
               // Avec Injection de dépendance de services ----------------------------------------
               p_group.MapGet("", GetAll)                                                           // Cette méthode GetAll se trouve plus bas dans cette même classe et nous n'utilisons plus directement celle dans la classe "EFCorePersonService.cs"
                    .WithTags("PersonManagement");                                                  // Permet de regrouper les Endpoints par 'Tag' dans Swagger



               // Avec Injection de dépendance de services ----------------------------------------
               p_group.MapGet("/{id:int}", GetById)                                                 // Cette méthode GetById se trouve plus bas dans cette même classe et nous n'utilisons plus directement celle dans la classe "EFCorePersonService.cs"
                    .WithTags("PersonManagement")                                                   // Permet de regrouper les Endpoints par 'Tag' dans Swagger
                    .WithName("GetById");                                                           // Nomme l'Endpoint, permettant ainsi un lien pour le "linkgenerator" (voir dans la méthode POST ci-dessous et non dans le GETBYID)
                                                                                                    //                                                                                          ----
                                                                                                    // Cela permet de récupérer l'URI de la ressource qui vient d'être créé par le POST et de l'inclure dans les informations de retour au code appelant





               // =================================================================================
               //                                                                                 !
               //                                   Endpoints POST                                !
               //                                                                                 !
               // =================================================================================
               // Avec Injection de dépendance de services ----------------------------------------
               p_group.MapPost("", Post)                                                            // Cette méthode Post se trouve plus bas dans cette même classe et nous n'utilisons plus directement celle dans la classe "EFCorePersonService.cs"
                    .Accepts<PersonnInputModel>(contentType: "application/json")                    // Permet d'indiquer que l'Endpoint accepte un 'PersonnInputModel'   avec un 'Media Type' (dans Swagger) de type json : 'application/json' (voir la partie "Request body" dand Swagger
                    .Produces<PersonOutputModel>(contentType: "application/json")                   // Permet d'indiquer que l'Endpoint retourne un 'PersonnOutputModel' avec un 'Media Type' (dans Swagger) de type json : 'application/json' (voir la partie "Responses" dand Swagger
                    .WithTags("PersonManagement");                                                  // Permet de regrouper les Endpoints par 'Tag' dans Swagger





               // =================================================================================
               //                                                                                 !
               //                                   Endpoints PUT                                 !
               //                                                                                 !
               // =================================================================================
               // Avec Injection de dépendance de services ----------------------------------------
               p_group.MapPut("/{id:int}", Put)                                                     // Cette méthode Put se trouve plus bas dans cette même classe et nous n'utilisons plus directement celle dans la classe "EFCorePersonService.cs"
                    .Produces(204)                                                                  // Permet d'indiquer dans Swagger que ce code de retour est utilisé (sinon il n'indiquera que le '200')
                    .Produces(404)                                                                  // Permet d'indiquer dans Swagger que ce code de retour est utilisé (sinon il n'indiquera que le '200')
                    .WithTags("PersonManagement");                                                  // Permet de regrouper les Endpoints par 'Tag' dans Swagger





               // =================================================================================
               //                                                                                 !
               //                                 Endpoints DELETE                                !
               //                                                                                 !
               // =================================================================================
               // Avec Injection de dépendance de services ----------------------------------------
               p_group.MapDelete("/{id:int}", Delete)                                               // Cette méthode Delete se trouve plus bas dans cette même classe et nous n'utilisons plus directement celle dans la classe "EFCorePersonService.cs"
                    .WithTags("PersonManagement");                                                  // Permet de regrouper les Endpoints par 'Tag' dans Swagger





               return p_group;
          }





          // ==================================================================================================================================================
          //                                                                                                                                                  !
          //                                                             Méthodes des "Person"                                                                !
          //                                                                                                                                                  !
          // ==================================================================================================================================================
          // =================================================================================
          //                                                                                 !
          //                                    Methods GET                                  !
          //                                                                                 !
          // =================================================================================
          private static async Task<IResult> GetAll(
               [FromServices] IPersonService iPersonService)
          {
               var p = await iPersonService.GetAll();
               return Results.Ok(p);
          }



          private static async Task<IResult> GetById (
                    [FromRoute]    int                 id,

                    [FromServices] IDistributedCache   cache,
                    [FromServices] IPersonService      iPersonService)
          {

                    var p = await cache.GetAsync<PersonOutputModel>($"personne_{id}");                   // J'essaie en premier de récupérer la Person à partir du cache

                    if (p is null)
                    {
                         p = await iPersonService.GetById(id);                                           // Sinon j'essaie à partir de la base de données

                         if (p is null)
                         {
                              return Results.NotFound();
                         }
                         else
                         {
                              await cache.SetAsync($"personne_{id}", p);                                 // Je stocke la Person dans le cache
                              return Results.Ok(p);
                         }
                    }
                    else
                    {
                         return Results.Ok(p);
                    }
          }



          // =================================================================================
          //                                                                                 !
          //                                    Methods POST                                 !
          //                                                                                 !
          // =================================================================================
          private static async Task<IResult> Post(
                    [FromBody]     PersonnInputModel             p,

                    [FromServices] IValidator<PersonnInputModel> validator,
                    [FromServices] IPersonService                iPersonService,
                    [FromServices] IDistributedCache             cache,
                    [FromServices] LinkGenerator                 linkGenerator,

                                   HttpContext                   httpContext,
                                   CancellationToken             token)
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
                         var dbResult = await iPersonService.Add(p);

                    //return Results.Ok(p);                                                                                            // Retourne simplement un code '200 OK'

                    //// Sans le nom du domaine, donc fournit un chemin relatif (dans l'attribut 'Location' du Header de retour )
                    //// -> ATTENTION : c'est le chemin et non l'URI (voir ci-dessous) qu'il faut fournir si l'API est derrière une passerelle dont le rôle est de distribuer la requête au serveur qui est le moins utilisé
                    //var path = linkGenerator.GetPathByName("GetById", new { id = dbResult.Id });                                     // Le 'GetById' désigne l'Endpoint 'GetById' grâce à son instruction '.WithName("GetById");'
                    //return Results.Created(path, dbResult);

                    // Avec le nom du domaine, donc fournit un chemin absolu (dans l'attribut 'Location' du Header de retour )
                    var uri = linkGenerator.GetUriByName(httpContext, "GetById", new { id = dbResult.Id });                            // Le 'GetById' désigne l'Endpoint 'GetById' grâce à son instruction '.WithName("GetById");'

                    return Results.Created(uri, dbResult);                                                                             // Retourne un code '201 Created' avec l'URI complète (la route) de l'API (GetById) qui permet de consulter l'enregistrement créé
               }
          }



          // =================================================================================
          //                                                                                 !
          //                                    Methods PUT                                  !
          //                                                                                 !
          // =================================================================================
          private static async Task<IResult> Put(
                    [FromRoute]    int                 id,

                    [FromBody]     PersonnInputModel   p,

                    [FromServices] IPersonService      iPersonService,
                    [FromServices] IDistributedCache   cache)
          {

                    var result = await iPersonService.Update(id, p);

                    if (result)
                    {
                         
                         await cache.RemoveAsync($"personne_{id}");
                         return Results.NoContent();
                    }
                    else
                    {
                         
                         return Results.NotFound();
                    }
          }



          // =================================================================================
          //                                                                                 !
          //                                  Methods DELETE                                 !
          //                                                                                 !
          // =================================================================================
          private static async Task<IResult> Delete(
                    [FromRoute]    int                 id,

                    [FromServices] IPersonService      iPersonService,
                    [FromServices] IDistributedCache   cache)
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
          }
     }
}
