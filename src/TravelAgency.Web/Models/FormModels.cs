using System.ComponentModel.DataAnnotations;

using TravelAgency.Core.Enums;

namespace TravelAgency.Web.Models;

public class LoginForm
{
    [Required(ErrorMessage = "Вкажіть електронну пошту.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Вкажіть пароль.")]
    public string Password { get; set; } = string.Empty;
}

public class RegisterForm
{
    [Required(ErrorMessage = "Вкажіть електронну пошту.")]
    [EmailAddress(ErrorMessage = "Некоректна адреса електронної пошти.")]
    [StringLength(254, ErrorMessage = "Пошта занадто довга.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Вкажіть прізвище та ім'я.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Ім'я має містити від 2 до 100 символів.")]
    public string FullName { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "Телефон не може бути довшим за 20 символів.")]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Вкажіть пароль.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Пароль має містити щонайменше 8 символів.")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Повторіть пароль.")]
    [Compare(nameof(Password), ErrorMessage = "Паролі не збігаються.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

/// <summary>Фільтри каталогу турів (форма на головній сторінці та в каталозі).</summary>
public class TourFilterForm
{
    public string? Country { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public string? Hotel { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public MealPlan? Meal { get; set; }

    /// <summary>Рядок запиту для сторінки /tours (порожні фільтри пропускаються).</summary>
    public string ToQueryString(int page = 1)
    {
        var parts = new List<string>();

        void Add(string key, string? value)
        {
            if (!string.IsNullOrWhiteSpace(value))
                parts.Add($"{key}={Uri.EscapeDataString(value)}");
        }

        var inv = System.Globalization.CultureInfo.InvariantCulture;
        Add("country", Country);
        Add("from", From?.ToString("yyyy-MM-dd", inv));
        Add("to", To?.ToString("yyyy-MM-dd", inv));
        Add("hotel", Hotel);
        Add("min", MinPrice?.ToString(inv));
        Add("max", MaxPrice?.ToString(inv));
        Add("meal", Meal?.ToString());
        if (page > 1)
            Add("page", page.ToString(inv));

        return parts.Count == 0 ? "/tours" : "/tours?" + string.Join("&", parts);
    }
}

/// <summary>Фільтр заявок у кабінеті менеджера.</summary>
public class BookingFilterForm
{
    public BookingStatus? Status { get; set; }
    public string? Customer { get; set; }
}

/// <summary>Один турист у формі бронювання (поля лише для інтерфейсу, помилки — після спроби відправити).</summary>
public class TouristForm
{
    public string FullName { get; set; } = string.Empty;
    public string Passport { get; set; } = string.Empty;
    public Gender Gender { get; set; } = Gender.Male;
    public int? Age { get; set; }
    public string? ChildrenInfo { get; set; }
    public Dictionary<string, string> Errors { get; } = new();

    /// <summary>Перевірка тих самих обмежень, що й у TouristRequest на сервері.</summary>
    public bool Validate()
    {
        Errors.Clear();

        var name = FullName.Trim();
        if (name.Length == 0)
            Errors["FullName"] = "Вкажіть прізвище та ім'я.";
        else if (name.Length > 100)
            Errors["FullName"] = "Не більше 100 символів.";

        var passport = Passport.Trim();
        if (passport.Length == 0)
            Errors["Passport"] = "Вкажіть номер паспорта.";
        else if (passport.Length > 20)
            Errors["Passport"] = "Не більше 20 символів.";

        if (Age is null or < 1 or > 120)
            Errors["Age"] = "Вік має бути від 1 до 120.";

        if (ChildrenInfo is { Length: > 50 })
            Errors["ChildrenInfo"] = "Не більше 50 символів.";

        return Errors.Count == 0;
    }
}
