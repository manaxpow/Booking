using System;
using System.Collections.Generic;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Cccd { get; set; } = string.Empty;
    public string Role { get; set; } = "USER"; // ADMIN, USER
    public DateTime CreateAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdateAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
