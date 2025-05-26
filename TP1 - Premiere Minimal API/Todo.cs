namespace TP1___Premiere_Minimal_API;



public record class Todo(
     int            Id,
     string         Title,
     DateTime       StartDate,
     DateTime?      EndDate        = null
);