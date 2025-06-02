
namespace TP2_AméliorerlaMinimalAPIduTP1.DTO;



public record class UserOutputDto        // Un record permet d'être "immuable"
(
     int            Id,
     string         Name,
     string         Token
);
