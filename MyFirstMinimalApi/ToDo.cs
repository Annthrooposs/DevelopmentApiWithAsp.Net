namespace MyFirstMinimalApi
{

     // Record fournissant le modèle de l'API -----------------------------------------------------------------------------------------------------------------
     public record class ToDo(int Id, string Title, DateTime StartDate, DateTime? EndDate = null);
}
