using TravelAgency.Core.Enums;

namespace TravelAgency.Core.Models;

/// <summary>Дані мандрівника, що надходять із запиту (паспорт у відкритому вигляді, шифрується сервісом).</summary>
public record TouristInfo(string FullName, string Passport, Gender Gender, int Age, string? ChildrenInfo);
