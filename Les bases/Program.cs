using Microsoft.AspNetCore.Mvc;                   // Pour : [FromRoute
using MinimalApi;



// locals -------------------------------------------------------------------------------------------------------------------------------------------
WebApplicationBuilder? builder = WebApplication.CreateBuilder();


// Injection d'un service dans le container d'injection de dépendances ------------------------------------------------------------------------------
builder.Services.AddSingleton<ArticleService>();       // Nous enregistrons le service "ArticleService" dans le conteneur d'Injection de dépendance ASP.Net
                                                       // Permet de n'avoir qu'une seule instance du service "ArticleService", permettant ainsi d'ajouter les nouveaux articles dans la même instance quand bien même noous avons le mot "new Article" dans la méthode Add de la classe ArticleService.
                                                       // Ce singleton est donc partagé par tous les Endpoints

WebApplication? app = builder.Build(); 






// ===================================================================================================================================
//                                                                                                                                   !
//                                                        ENDPOINT GET                                                               !
//                                                                                                                                   !
// ===================================================================================================================================
//app.MapGet("/Get", () => "Hello GET");


//app.MapMethods("/methods", new[] { "GET", "POST" }, () => "Hello methods");                                                               // Possibilité de mapper un Endpoint avec plusieurs verbes HTTP (ici GET et POST) et de lui donner une route différents (ici "methods")


//app.MapGet("/article", () => new Article(999, "Tournevis"));
//app.MapGet("/article/1", () => new Article(1, "Marteau"));                                                                                // Récupération d'information à partir de la route
//app.MapGet("/articlequerystring", (int id) => new Article(id, "Marteau"));                                                                // Récupération d'information à partir d'une Query string
//app.MapGet("/article/{id:int}", (int id) => new Article(id, "Marteau"));                                                                  // Récupération d'information à partir de la route avec une contrainte sur le type attendu
//app.MapGet("/article/{title}", (string title) => new Article(9999, title));                                                               // Récupération d'information à partir de la route avec une contrainte sur le type attendu (ici par défaut, le type est string)



//List<Article> list = new List<Article>
//{
//     new Article(1, "Marteau"),
//     new Article(2, "Scie")
//};
//app.MapGet("/article/{id:int}", (int id) =>
//{
//     var article = list.Find(x => x.Id == id);

//     if (article is not null)
//     {
//          return Results.Ok(article);
//     }
//     else
//     {
//          //return Results.NotFound("Pas trouvé");
//          return Results.NotFound();
//          //return Results.StatusCode(599);                                                                                               // 'StatusCode' permet de renvoyer des codes de statut qui me sont propres !
//     }
//});



//app.MapGet("/personne/{nom?}", (string? nom, string? prenom) => Results.Ok($"{nom} {prenom}"));                                           // Récupération d'information ) partir de la Query string



//// Avec des attributs sur les paramètres de lambda
//app.MapGet("/personne/{nom?}",
//(
//     [FromRoute(              Name = "nom")]                string    nomPersonne,
//     [FromQuery(              Name = "prenom")]             string?   prenomPersonne,
//     [FromHeaderAttribute(    Name = "Accept-Encoding")]    string    encoding,
//     [FromHeaderAttribute(    Name = "MaClef")]             string    MaValeur
//) =>

//Results.Ok($"{nomPersonne} {prenomPersonne} accepte les encoding suivants : {encoding}. De plus nous avons un Header défini par mes propres soins (MaClef-MaValeur) : {MaValeur}"));



//app.MapGet("/personne/identite", (Personne p) => Results.Ok(p));



//app.MapGet("/articles/{id:int}", ([FromRoute] int id, [FromServices] ArticleService s) =>
app.MapGet("/articles/{id:int}",
(

     //[FromRoute( Name = "id")]    int id,
     [FromRoute]         int                 id,
     [FromServices]      ArticleService      s
) =>

{
     var article = s.GetAll().Find(a => a.Id == id);

     if (article is not null)
     {
          
          return Results.Ok(article);
     }

     else
     {
          
          return Results.NotFound();
     }
});





// ===================================================================================================================================
//                                                                                                                                   !
//                                                        ENDPOINT POST                                                              !
//                                                                                                                                   !
// ===================================================================================================================================
//app.MapPost("/Post", () => "Hello POST");


//app.MapPost("/personne/identite", (Personne p) => Results.Ok(p));


app.MapPost("/articles", ([FromBody] Article a, [FromServices] ArticleService s) =>
{
     
     var result = s.Add(a.Title);
     return Results.Ok(result);
});





// ===================================================================================================================================
//                                                                                                                                   !
//                                                        ENDPOINT PUT                                                               !
//                                                                                                                                   !
// ===================================================================================================================================
//app.MapPut("/Put", () => "Hello PUT");





// ===================================================================================================================================
//                                                                                                                                   !
//                                                       ENDPOINT PATCH                                                              !
//                                                                                                                                   !
// ===================================================================================================================================
//app.MapPatch("/Patch(", () => "Hello PATCH");





// ===================================================================================================================================
//                                                                                                                                   !
//                                                       ENDPOINT DELETE                                                             !
//                                                                                                                                   !
// ===================================================================================================================================
//app.MapDelete("/Delete", () => "Hello DELETE");





// ===================================================================================================================================
//                                                                                                                                   !
//                                                            RUN                                                                    !
//                                                                                                                                   !
// ===================================================================================================================================
app.Run();