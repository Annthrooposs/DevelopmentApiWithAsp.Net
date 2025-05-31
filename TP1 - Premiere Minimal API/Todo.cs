namespace TP1_PremiereMinimalAPI;


public record class Todo(
     int            Id,
     string         Title,
     DateTime       StartDate,
     DateTime?      EndDate        = null
);