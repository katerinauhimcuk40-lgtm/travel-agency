namespace TravelAgency.Web.Services;

/// <summary>Помилка звернення до API. Повідомлення вже українською і готове до показу користувачу.</summary>
public sealed class ApiException : Exception
{
    public ApiException(string message, int? statusCode = null) : base(message)
    {
        StatusCode = statusCode;
    }

    public int? StatusCode { get; }
}

/// <summary>Переклад відповідей API (ProblemDetails) на українську.</summary>
public static class ErrorMessages
{
    // Порядок важливий: перше співпадіння виграє
    private static readonly (string Needle, string Text)[] Known =
    [
        ("already exists", "Користувач із такою поштою вже існує."),
        ("Invalid email or password", "Невірна пошта або пароль."),
        ("Not enough available seats", "У групі недостатньо вільних місць."),
        ("already departed", "Виліт цієї групи вже відбувся."),
        ("Payment amount must be equal", "Сума оплати має дорівнювати вартості бронювання."),
        ("must be confirmed first", "Оплата можлива лише після підтвердження бронювання менеджером."),
        ("Cannot cancel", "Це бронювання вже не можна скасувати."),
        ("Cannot confirm", "Це бронювання не можна підтвердити."),
        ("Cannot pay", "Оплата недоступна для поточного статусу бронювання."),
        ("Booking not found", "Бронювання не знайдено."),
        ("Tour group not found", "Групу не знайдено."),
        ("Tour not found", "Тур не знайдено."),
        ("MinPrice must not be greater", "Мінімальна ціна не може перевищувати максимальну."),
        ("DepartureFrom must not be later", "Дата «від» не може бути пізнішою за дату «до»."),
        ("At least one tourist", "Додайте хоча б одного туриста.")
    ];

    public static string Translate(int status, string? detail)
    {
        if (!string.IsNullOrWhiteSpace(detail))
        {
            foreach (var (needle, text) in Known)
            {
                if (detail.Contains(needle, StringComparison.OrdinalIgnoreCase))
                    return text;
            }
        }

        return status switch
        {
            400 => "Перевірте правильність введених даних.",
            401 => "Потрібно увійти в обліковий запис.",
            403 => "Недостатньо прав для цієї дії.",
            404 => "Запитаний ресурс не знайдено.",
            409 => "Дію не можна виконати в поточному стані.",
            >= 500 => "Помилка сервера. Спробуйте пізніше.",
            _ => "Не вдалося виконати запит."
        };
    }
}
