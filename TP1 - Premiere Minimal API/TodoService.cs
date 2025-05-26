namespace TP1___Premiere_Minimal_API;

public class TodoService
{
     
     // ===================================================================================================================================
     //                                                            FIELD                                                                  
     // ===================================================================================================================================
     private List<Todo> _list = new List<Todo>
     {
          new Todo(1, "Apprendre C# par moi-même",     new DateTime(2024, 10,    7)),
          new Todo(2, "Guérir par les plantes",        new DateTime(2025, 1,    31))
     };





     // ===================================================================================================================================
     //                                                            METHOD                                                                 
     // ===================================================================================================================================
     // Utilisée dans un GET --------------------------------------------------------------------------------------------------------------
     public List<Todo> GetAll() => _list;



     public Todo? GetById(int p_id) => _list.Find(td => td.Id == p_id);






     // Utilisée dans un POST -------------------------------------------------------------------------------------------------------------
     public Todo Add(string p_title)
     {

          int  id   = _list.Count > 0 ? _list.Max(a => a.Id) + 1 : 1;                                                                          // On vérifie, grâce à une ternaire, que la liste n'est pas vide avant de chercher le max...sinon nous affectons '1' comme Id du Todo à créer.
          Todo todo = new Todo(id, p_title, DateTime.Now);

          _list.Add(todo);

          // ou bien (en raccourci)
          //_list.Add(new Todo(_list.Max(a => a.Id) + 1, p_title));                                                                           // On peut aussi faire en une seule ligne


          return todo;
     }






     // Utilisée dans un UPDATE -----------------------------------------------------------------------------------------------------------
     public void Update(int p_id, Todo p_todo)
     {

          //Delete(Id);                                                                                                                // On supprime l'ancien Todo (si il existe) avant d'ajouter le nouveau
          //_list.Add(p_todo);                                                                                                                // On ajoute le nouveau Todo
          // ou bien
          _list.Add(new Todo(p_id, p_todo.Title, p_todo.StartDate, p_todo.EndDate));                                                   // Avantage de cette méthode, on peut modifier le Todo sans avoir à le supprimer avant.
     }





     // Utilisée dans un DELETE -----------------------------------------------------------------------------------------------------------
     public bool Delete(int p_id)
     {

          var todo = GetById(p_id);
          if (todo is not null)
          {

               _list.Remove(todo);
               return true;
          }
          else
          {
               return false;
          }
     }
}
