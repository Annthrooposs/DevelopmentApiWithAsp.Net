using MinimalAPIProfesional.Data.Models;
using MinimalAPIProfesional.DTO___Models;

namespace MinimalAPIProfesional.Services
{
     public interface IPersonService
     {
          Task<List<PersonOutputModel>>      GetAll();
          Task<PersonOutputModel?>           GetById(int id);
          Task<PersonOutputModel>            Add(PersonnInputModel person);
          Task<bool>                         Update(int id, PersonnInputModel person);
          Task<bool>                         Delete(int id);
     }
}
