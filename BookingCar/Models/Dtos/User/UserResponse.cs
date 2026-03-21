public record UserResponse(
    int Id,
    string Name,
    string Phone,
    string Email,
    string Cccd,
    string Role,
    DateTime CreateAt,
    DateTime UpdateAt
);

public record UserDetailResponse(
    int Id,
    string Name,
    string Phone,
    string Email,
    string Cccd,
    string Role,
    DateTime CreateAt,
    DateTime UpdateAt
);
