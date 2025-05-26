using System.Reflection;



namespace MinimalApi;

public class Personne
{
     public string? Nom       { get; set; }
     public string? Prenom    { get; set; }





     //Pour le GET ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
     public static bool TryParse(string p_value, out Personne? p_personne)
     {
          
          try
          {
               var data = p_value.Split(' ');

               p_personne = new Personne
               {
                    Nom       = data[0],
                    Prenom    = data[1]
               };

               return true;
          }



          catch (Exception)
          {

               p_personne = null;
               return false;
          }
     }





     // Pour le POST  en n'utilisant PAS le format d'entrée et de sortie par défaut à savoir raw/json mais par exemple un format raw/text -------------------------------------------------------------
     public static async ValueTask<Personne?> BindAsync(HttpContext p_context, ParameterInfo p_parameterInfo)
     {
          
          try
          {
               using var streamReader   = new StreamReader(p_context.Request.Body);
                     var body           = await streamReader.ReadToEndAsync();
                     var data           = body.Split(' ');


               var person = new Personne
               {
                    Nom       = data[0],
                    Prenom    = data[1]
               };

               return person;
          }



          catch (Exception)
          {

               return null;
          }
     }
}
