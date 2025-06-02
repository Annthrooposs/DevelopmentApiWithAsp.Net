
namespace TP2_AméliorerlaMinimalAPIduTP1.DTO;



public record class TodoOutputDto        // Un record permet d'être "immuable"
(
     int            Id,
     string         Title,
     DateTime       StartDate,
     DateTime?      EndDate
);
