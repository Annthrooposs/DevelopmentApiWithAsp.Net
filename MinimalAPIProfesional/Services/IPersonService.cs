
using MinimalAPIProfesional.Data.Models;
using MinimalAPIProfesional.DTO;



namespace MinimalAPIProfesional.Services;

public interface IPersonService
{
     Task<List<PersonOutputModel>>      GetAll();
     Task<PersonOutputModel?>           GetById(int id);
     Task<PersonOutputModel>            Add(PersonInputModel person);
     Task<bool>                         Update(int id, PersonInputModel person);
     Task<bool>                         Delete(int id);
}
