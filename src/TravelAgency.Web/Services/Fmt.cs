using System.Globalization;

using TravelAgency.Core.Enums;
using TravelAgency.Web.Models;

namespace TravelAgency.Web.Services;

/// <summary>Форматування й підписи для інтерфейсу (українською).</summary>
public static class Fmt
{
    private static readonly CultureInfo Uk = CreateUkCulture();

    private static CultureInfo CreateUkCulture()
    {
        try
        {
            return CultureInfo.GetCultureInfo("uk-UA");
        }
        catch (CultureNotFoundException)
        {
            return CultureInfo.InvariantCulture;
        }
    }

    private static readonly Dictionary<string, string> Countries = new(StringComparer.OrdinalIgnoreCase)
    {
        ["France"] = "Франція",
        ["Turkey"] = "Туреччина",
        ["Ukraine"] = "Україна",
        ["Italy"] = "Італія",
        ["Egypt"] = "Єгипет",
        ["Greece"] = "Греція",
        ["Spain"] = "Іспанія",
        ["UAE"] = "ОАЕ",
        ["Poland"] = "Польща",
        ["Czechia"] = "Чехія",
        ["Bulgaria"] = "Болгарія",
        ["Montenegro"] = "Чорногорія",
        ["Croatia"] = "Хорватія",
        ["Germany"] = "Німеччина",
        ["Austria"] = "Австрія",
        ["Portugal"] = "Португалія",
        ["Cyprus"] = "Кіпр",
        ["Thailand"] = "Таїланд",
        ["Georgia"] = "Грузія",
        ["Hungary"] = "Угорщина"
    };

    /// <summary>Назва країни українською (якщо відома), інакше як у базі.</summary>
    public static string Country(string country) =>
        Countries.TryGetValue(country, out var ukrainian) ? ukrainian : country;

    /// <summary>Ціна в гривнях без копійок, якщо їх немає.</summary>
    public static string Money(decimal value) =>
        value.ToString(value % 1 == 0 ? "N0" : "N2", Uk) + " ₴";

    public static string Date(DateTime date) => date.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);

    public static string DateRange(DateTime departure, DateTime arrival) => $"{Date(departure)} – {Date(arrival)}";

    public static int Nights(DateTime departure, DateTime arrival) => Math.Max(1, (arrival.Date - departure.Date).Days);

    public static string NightsLabel(DateTime departure, DateTime arrival)
    {
        var n = Nights(departure, arrival);
        return $"{n} {Plural(n, "ніч", "ночі", "ночей")}";
    }

    public static string PersonsLabel(int n) => $"{n} {Plural(n, "турист", "туристи", "туристів")}";

    /// <summary>«4★» замість «4*».</summary>
    public static string Stars(string hotelType) => hotelType.Replace("*", "★");

    public static string Seats(int available) => available switch
    {
        <= 0 => "Місць немає",
        <= 5 => $"Залишилось {available} {Plural(available, "місце", "місця", "місць")}",
        _ => "Є вільні місця"
    };

    public static string Meal(MealPlan meal) => meal switch
    {
        MealPlan.RO => "Без харчування",
        MealPlan.BB => "Сніданки",
        MealPlan.HB => "Напівпансіон",
        MealPlan.FB => "Повний пансіон",
        MealPlan.AI => "Все включено",
        _ => meal.ToString()
    };

    public static string Transport(TransportType transport) => transport switch
    {
        TransportType.Plane => "Авіапереліт",
        TransportType.Bus => "Автобус",
        TransportType.Train => "Потяг",
        TransportType.Ship => "Морський транспорт",
        _ => transport.ToString()
    };

    public static string GenderLabel(Gender gender) => gender switch
    {
        Gender.Male => "Чоловіча",
        Gender.Female => "Жіноча",
        _ => gender.ToString()
    };

    public static string Status(BookingStatus status) => status switch
    {
        BookingStatus.New => "Очікує підтвердження",
        BookingStatus.Confirmed => "Підтверджено",
        BookingStatus.Paid => "Оплачено",
        BookingStatus.Completed => "Завершено",
        BookingStatus.Cancelled => "Скасовано",
        _ => status.ToString()
    };

    public static string StatusClass(BookingStatus status) => status switch
    {
        BookingStatus.New => "badge--new",
        BookingStatus.Confirmed => "badge--confirmed",
        BookingStatus.Paid => "badge--paid",
        BookingStatus.Completed => "badge--completed",
        BookingStatus.Cancelled => "badge--cancelled",
        _ => string.Empty
    };

    /// <summary>Форма слова за числом: 1 ніч, 2 ночі, 5 ночей.</summary>
    public static string Plural(int n, string one, string few, string many)
    {
        var n10 = n % 10;
        var n100 = n % 100;

        if (n10 == 1 && n100 != 11)
            return one;

        if (n10 is >= 2 and <= 4 && n100 is not (>= 12 and <= 14))
            return few;

        return many;
    }

    /// <summary>Розбирає код харчування з рядка запиту; невідоме значення — null.</summary>
    public static MealPlan? ParseMeal(string? value) =>
        Enum.TryParse<MealPlan>(value, ignoreCase: true, out var meal) && Enum.IsDefined(meal) ? meal : null;
}
