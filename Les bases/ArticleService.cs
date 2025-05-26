namespace MinimalApi;

public class ArticleService
{
     // ===================================================================================================================================
     //                                                                                                                                   !
     //                                                            FIELD                                                                  !
     //                                                                                                                                   !
     // ===================================================================================================================================
     private List<Article> _list = new List<Article>
     {
          
          new Article(1, "Marteau"),
          new Article(2, "Scie")
     };





     // ===================================================================================================================================
     //                                                                                                                                   !
     //                                                            METHOD                                                                 !
     //                                                                                                                                   !
     // ===================================================================================================================================
     // Utilisée dans un GET --------------------------------------------------------------------------------------------------------------
     public List<Article> GetAll() => _list;



     // Utilisée dans un POST -------------------------------------------------------------------------------------------------------------
     public Article Add(string p_title)
     {
          
          var article = new Article(_list.Max(a => a.Id) + 1, p_title);                                                                     // Le 'Max' permet de récupérer le dernier identifiant créé
                                                                                                                                            // puis de lui ajouter 1 pour le nouvel article
          _list.Add(article);

          // ou bien (en raccourci)
          //_list.Add(new Article(_list.Max(a => a.Id) + 1, p_title));                                                                      // On peut aussi faire en une seule ligne



          return article;
     }
}
