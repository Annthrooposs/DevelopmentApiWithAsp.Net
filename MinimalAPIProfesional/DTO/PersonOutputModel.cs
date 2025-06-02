
namespace MinimalAPIProfesional.DTO;



public record class PersonOutputModel        // Un record permet d'être "immuable"
(
     int       Id,
     string    FullName,
     DateTime? Birthday
);
