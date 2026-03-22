public record DriverResponse(
    int Id,
    string Name,
    DateTime Dob,
    string License,
    DateTime CreateAt,
    DateTime UpdateAt
);

public record DriverDetailResponse(
    int Id,
    string Name,
    DateTime Dob,
    string License,
    DateTime CreateAt,
    DateTime UpdateAt
);
