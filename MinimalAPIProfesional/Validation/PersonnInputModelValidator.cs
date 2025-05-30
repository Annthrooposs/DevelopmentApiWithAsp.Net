using FluentValidation;
using MinimalAPIProfesional.Data.Models;
using MinimalAPIProfesional.DTO;

namespace MinimalAPIProfesional.Validation
{
    public class PersonnInputModelValidator : AbstractValidator<PersonInputModel>
     {
          public PersonnInputModelValidator()
          {
               RuleFor(p => p.FirstName).NotEmpty();
               RuleFor(p => p.LastName).NotEmpty();
               //RuleFor(p => p.Birthday).LessThanOrEqualTo(DateTime.Now);
          }
     }
}
