
using FluentValidation;
using TP2_AméliorerlaMinimalAPIduTP1.DTO;



namespace TP2_AméliorerlaMinimalAPIduTP1.Validation;

public class TodoInputModelValidator : AbstractValidator<TodoInputDto>
{
     
     public TodoInputModelValidator()
     {
          
          RuleFor(t => t.Title).NotEmpty().WithMessage("Title is required.");
          RuleFor(t => t.StartDate).GreaterThanOrEqualTo(DateTime.Now).WithMessage("Start date must be in the future.");
          RuleFor(t => t.EndDate).GreaterThanOrEqualTo(DateTime.Now).WithMessage("End date must be in the future.");
     }
}
