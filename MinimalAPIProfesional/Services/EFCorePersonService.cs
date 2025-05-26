using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using MinimalAPIProfesional.Data;
using MinimalAPIProfesional.Data.Models;
using MinimalAPIProfesional.DTO___Models;

namespace MinimalAPIProfesional.Services
{
     public class EFCorePersonService : IPersonService
     {
          // ========================================================================================================================================
          //                                                                                                                                        !
          //                                                               Fields                                                                   !
          //                                                                                                                                        !
          // ========================================================================================================================================
          private readonly ApiDbContext _context;




          // ========================================================================================================================================
          //                                                                                                                                        !
          //                                                           Constructeurs                                                                !
          //                                                                                                                                        !
          // ========================================================================================================================================
          public EFCorePersonService(ApiDbContext p_context)
          {
               _context = p_context;
          }






          // ========================================================================================================================================
          //                                                                                                                                        !
          //                                                              Methods                                                                   !
          //                                                                                                                                        !
          // ========================================================================================================================================
          // Permet de transformer l'Output renvoyé en passant d'un Model à un autre manuellement ---------------------------------------------------
          // Equivalent à l'outil communautaire "AutoMapper" : plus rapide mais moins ~sûr car nous pouvons 'oublier' une transformation ------------
          private PersonOutputModel ToOutputModel(Person dbPerson) => new PersonOutputModel
                    (
                         dbPerson.Id,
                         $"{dbPerson.FirstName} {dbPerson.LastName}",
                         dbPerson.Birthday == DateTime.MinValue ? null : dbPerson.Birthday
                    );






          // ========================================================================================================================================
          //                                                                                                                                        !
          //                                                    Methods called by the Endpoints                                                     !
          //                                                                                                                                        !
          // ========================================================================================================================================
          // MapGET ---------------------------------------------------------------------------------------------------------------------------------
          public async Task<List<PersonOutputModel>> GetAll()
          {
               return (await _context.Persons.ToListAsync()).ConvertAll(ToOutputModel);
          }



          public async Task<PersonOutputModel?> GetById(int p_id)
          {
               Person? dbPersonn = await _context.Persons.Where(w => w.Id == p_id).FirstOrDefaultAsync();

               if (dbPersonn is not null)
               {
                    return ToOutputModel(dbPersonn);
               }
               else
               {
                    return null;
               }
          }



          // MapPOST --------------------------------------------------------------------------------------------------------------------------------
          public async Task<PersonOutputModel> Add(PersonnInputModel p_person)
          {
               Person? dbPerson = new Person
               {
                    FirstName = p_person.FirstName,
                    LastName  = p_person.LastName,
                    Birthday  = p_person.Birthday.GetValueOrDefault()
               };
               
                         _context.Persons.Add(dbPerson);
               await     _context.SaveChangesAsync();

               return ToOutputModel(dbPerson);
          }



          // MapPUT ---------------------------------------------------------------------------------------------------------------------------------
          public async Task<bool> Update(int p_id, PersonnInputModel p_person)
          {
               return await _context.Persons
                                   .Where(w => w.Id == p_id)
                                   .ExecuteUpdateAsync(eua => eua.SetProperty(sp => sp.FirstName, p_person.FirstName)
                                                                 .SetProperty(sp => sp.LastName, p_person.LastName)
                                                                 .SetProperty(sp => sp.Birthday, p_person.Birthday)) > 0;



               // ATTENTION : voici une version plus élaborée décrite dans : Développement d'API avec ASP.NET / TP 2 / Corrigé (part 3) / 08:45
               // -----------------------------------------------------------------------------------------------------------------------------

               // var dbTodo = await ContextBoundObject.Todos.FirstOrDefaultAsync(testc => testc.Id == id && testc.UserId == UserSecretsIdAttribute);
               // if (dbTodo is null) return false;

               // dbTodo.Title     = item.Title;
               // dbTodo.StartDate = item.StartDate ?? dbTodo.SartDate;
               // dbTodo.EndDate   = item.EndDate ?? dbTodo.EndDate;

               // context.Todos.Update(dbTodo);
               // return await context.SaveChangesAsync() > 0
          }



          // MapDELETE ------------------------------------------------------------------------------------------------------------------------------
          public async Task<bool> Delete(int p_id)
          {
               return await _context.Persons.Where(w => w.Id == p_id).ExecuteDeleteAsync() > 0;
          }
     }
}
