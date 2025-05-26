
using Microsoft.AspNetCore.Mvc;
using TP1___Premiere_Minimal_API;



// ==================================================================================================================================================
//                                                                                                                                                  !
//                                                                Construction                                                                      !
//                                                                                                                                                  !
// ==================================================================================================================================================
// Creating the WebApplicationBuilder factory class -------------------------------------------------------------------------------------------------
WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);



// Injection d'un service dans le container d'injection de d�pendances ------------------------------------------------------------------------------
// Enregistrement des Services ----------------------------------------------------------------------------------------------------------------------
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddSingleton<TodoService>();
builder.Services.AddOpenApi();



// Building the WebApplication object ---------------------------------------------------------------------------------------------------------------
var app = builder.Build();



// G�n�ration du fichier swagger.json et de l'interface graphique exposant le fichier----------------------------------------------------------------
if (app.Environment.IsDevelopment())
{
     app.MapOpenApi();
}



// Configure the HTTP request pipeline --------------------------------------------------------------------------------------------------------------
app.UseHttpsRedirection();





// ==================================================================================================================================================
//                                                                                                                                                  !
//                                                                 Endpoints GET                                                                    !
//                                                                                                                                                  !
// ==================================================================================================================================================
// Recherche dans la liste des Todos sans crit�re de recherche --------------------------------------------------------------------------------------
app.MapGet("/todos",
(

     [FromServices] TodoService   tds
) =>

{
     var todo = tds.GetAll();

     if (todo is not null)
     {

          return Results.Ok(todo);
     }

     else
     {

          return Results.NotFound();
     }
});




// Recherche dans la liste des Todos selon un crit�re de recherche ----------------------------------------------------------------------------------
app.MapGet("/todos/{p_id:int}",
(

     //[FromRoute( Name = "id")]    int id,
     [FromRoute]    int            p_id,
     [FromServices] TodoService    tds
) =>

{
     var todo = tds.GetById(p_id);
     if (todo is null) return Results.NotFound();

     return Results.Ok(todo);
});



app.MapGet("todos/active",
(
     [FromServices] TodoService tds
) =>
{
     var todo = tds.GetAll().Where(td => td.EndDate is null);
     if (todo is not null)
     {
          return Results.Ok(todo);
     }
     else
     {
          return Results.NotFound();
     }
});





// ==================================================================================================================================================
//                                                                                                                                                  !
//                                                                Endpoints POST                                                                    !
//                                                                                                                                                  !
// ==================================================================================================================================================
app.MapPost("/todos", 
(
     [FromBody]     string         p_title,
     [FromServices] TodoService    tds
) =>
{

     var result = tds.Add(p_title);
     return Results.Ok(result);

     // ou bien (en raccourci)
     //return Results.Ok(tds.Add(p_title));
});





// ==================================================================================================================================================
//                                                                                                                                                  !
//                                                                 Endpoints PUT                                                                    !
//                                                                                                                                                  !
// ==================================================================================================================================================
app.MapPut("/todos/{p_id:int}",
(
     [FromRoute]    int            p_id,
     [FromBody]     Todo           p_td,
     [FromServices] TodoService    p_tds
) =>
{
     //var todo = p_tds.GetById(p_id);
     //if (todo is null) return Results.NotFound();


     //// On peut modifier le Todo sans avoir � le supprimer avant.
     //tds.Update(new Todo(id, td.Title, td.StartDate, td.EndDate));
     // ou bien en raccourci
     //tds.Update(td with { Id = id });
     p_tds.Update(p_id, p_td);

     return Results.NoContent();
});





// ==================================================================================================================================================
//                                                                                                                                                  !
//                                                               Endpoints DELETE                                                                   !
//                                                                                                                                                  !
// ==================================================================================================================================================
app.MapDelete("/todos/{p_id:int}",
(
     [FromRoute]    int            p_id,
     [FromServices] TodoService    tds
) =>
{
     var todo = tds.GetById(p_id);
     if (todo is null) return Results.NoContent();



     bool result = tds.Delete(p_id);
     if (result)
     {
          
          return Results.NoContent();
     }



     return Results.NotFound();
});





// ==================================================================================================================================================
//                                                                                                                                                  !
//                                                                      Run                                                                         !
//                                                                                                                                                  !
// ==================================================================================================================================================
// Registering and configuring terminal middleware --------------------------------------------------------------------------------------------------
app.Run();
