namespace MyFirstMinimalApi
{
     public class ToDoService
     {
          // Fields ---------------------------------------------------------------------------------------------------------------------------------
          private List<ToDo> _list = new List<ToDo>();





          // Methods --------------------------------------------------------------------------------------------------------------------------------
          public List<ToDo> GetAll() => _list;



          public ToDo? GetById(int p_id) => _list.Find(x => x.Id == p_id);



          public ToDo Add(string p_title)
          {
               int id = _list.Count > 0 ? _list.Max(a => a.Id) + 1 : 1;

               ToDo todo = new ToDo(
                    id,
                    p_title,
                    DateTime.Now
               );

               _list.Add(todo);

               return todo;
          }



          public void Update(int p_id, ToDo p_todo)
          {
               Delete(p_id);
               _list.Add(new ToDo(p_id, p_todo.Title, p_todo.StartDate, p_todo.EndDate));
          }



          public bool Delete(int p_id)
          {
               ToDo? todo = GetById(p_id);

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
}
