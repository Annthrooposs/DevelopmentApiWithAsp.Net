using Microsoft.AspNetCore.Mvc;
using MyFirstMinimalApi;


// Construction -------------------------------------------------------------------------------------------------------------------------------------
var builder = WebApplication.CreateBuilder();
builder.Services.AddSingleton<ToDoService>();

var app = builder.Build();





// Endpoints de lecture -----------------------------------------------------------------------------------------------------------------------------
app.MapGet("todos", (
     [FromServices] ToDoService s) =>
{
     
     return Results.Ok(s.GetAll());
});




app.MapGet("todos/{p_id:int}", (
     [FromRoute]    int p_id,
     [FromServices] ToDoService s) =>
{
     
     ToDo? todo = s.GetById(p_id);

     if (todo is null) return Results.NotFound();                                                                                                     // 404

     return Results.Ok(todo);
});




app.MapGet("todos/active", (
     [FromServices] ToDoService s) =>
{
     
     return Results.Ok(s.GetAll().Where(t => t.EndDate is null));
});





// Endpoints d'écriture -----------------------------------------------------------------------------------------------------------------------------
app.MapPost("todos", (
     [FromBody]     string p_title,
     [FromServices] ToDoService s) =>
{
     
     return Results.Ok(s.Add(p_title));
});




app.MapPut("todos/{p_id:int}", (
     [FromRoute]    int p_id,
     [FromBody]     ToDo p_todo,
     [FromServices] ToDoService s) =>
{
     
     s.Update(p_id, p_todo);

     return Results.NoContent();
});







// Endpoints de suppression -------------------------------------------------------------------------------------------------------------------------
app.MapDelete("todos/{p_id:int}", (
     [FromRoute]    int p_id,
     [FromServices] ToDoService s) =>
{
     
     Boolean result = s.Delete(p_id);
     if (result)
     {
          
          return Results.NoContent();
     }
     else
     {
          
          return Results.NotFound();
     }
});




// Run ----------------------------------------------------------------------------------------------------------------------------------------------
app.Run();