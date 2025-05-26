using FluentValidation;
using MinimalAPIProfesional.Data.Models;
using MinimalAPIProfesional.DTO___Models;

namespace MinimalAPIProfesional.Validation
{
    public class PersonnInputModelValidator : AbstractValidator<PersonnInputModel>
     {
          public PersonnInputModelValidator()
          {
               RuleFor(p => p.FirstName).NotEmpty();
               RuleFor(p => p.LastName).NotEmpty();
               //RuleFor(p => p.Birthday).LessThanOrEqualTo(DateTime.Now);
          }
     }
}
