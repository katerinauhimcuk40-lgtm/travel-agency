namespace TravelAgency.Core.Models;

/// <summary>Клієнт у списку CRM: дані облікового запису та кількість його бронювань (UC-11).</summary>
public record CustomerSummary(
    int Id,
    string Email,
    string FullName,
    string? Phone,
    bool IsActive,
    DateTime CreatedAt,
    int BookingsCount);
