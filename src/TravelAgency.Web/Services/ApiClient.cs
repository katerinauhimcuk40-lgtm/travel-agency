using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

using TravelAgency.Core.Enums;
using TravelAgency.Web.Models;

namespace TravelAgency.Web.Services;

/// <summary>Клієнт REST API TravelAgency.Api. Усі помилки перетворюються на ApiException з українським текстом.</summary>
public sealed class ApiClient
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly IHttpClientFactory _factory;
    private readonly AuthSession _session;

    public ApiClient(IHttpClientFactory factory, AuthSession session)
    {
        _factory = factory;
        _session = session;
    }

    // ----- Каталог (публічні кінцеві точки) -----

    public Task<PagedResponse<TourResponse>> GetToursAsync(int page = 1, int pageSize = 100) =>
        GetAsync<PagedResponse<TourResponse>>($"api/tours?page={page}&pageSize={pageSize}");

    public Task<TourDetailsResponse> GetTourAsync(int id) =>
        GetAsync<TourDetailsResponse>($"api/tours/{id}");

    public Task<PagedResponse<TourSearchItem>> SearchToursAsync(TourSearchQuery query)
    {
        var inv = CultureInfo.InvariantCulture;
        var parts = new List<string>();

        void Add(string key, string? value)
        {
            if (!string.IsNullOrWhiteSpace(value))
                parts.Add($"{key}={Uri.EscapeDataString(value)}");
        }

        Add("country", query.Country);
        Add("departureFrom", query.From?.ToString("yyyy-MM-dd", inv));
        Add("departureTo", query.To?.ToString("yyyy-MM-dd", inv));
        Add("hotel", query.Hotel);
        Add("minPrice", query.MinPrice?.ToString(inv));
        Add("maxPrice", query.MaxPrice?.ToString(inv));
        Add("mealPlan", query.Meal?.ToString());
        Add("page", query.Page.ToString(inv));
        Add("pageSize", query.PageSize.ToString(inv));

        return GetAsync<PagedResponse<TourSearchItem>>("api/tours/search?" + string.Join("&", parts));
    }

    // ----- Автентифікація -----

    public Task<AuthResponse> LoginAsync(string email, string password) =>
        PostAsync<AuthResponse>("api/auth/login", new { email, password }, auth: false);

    public Task<AuthResponse> RegisterAsync(RegisterForm form) =>
        PostAsync<AuthResponse>(
            "api/auth/register",
            new { email = form.Email, password = form.Password, fullName = form.FullName, phone = form.Phone },
            auth: false);

    // ----- Бронювання (потрібен токен) -----

    public Task<BookingResponse> CreateBookingAsync(int tourGroupId, IEnumerable<TouristForm> tourists) =>
        PostAsync<BookingResponse>(
            "api/bookings",
            new
            {
                tourGroupId,
                tourists = tourists.Select(t => new
                {
                    fullName = t.FullName.Trim(),
                    passport = t.Passport.Trim(),
                    gender = t.Gender,
                    age = t.Age ?? 0,
                    childrenInfo = string.IsNullOrWhiteSpace(t.ChildrenInfo) ? null : t.ChildrenInfo.Trim()
                }).ToList()
            },
            auth: true);

    public Task<List<BookingDetailsResponse>> GetMyBookingsAsync() =>
        GetAsync<List<BookingDetailsResponse>>("api/bookings/my", auth: true);

    public Task CancelBookingAsync(int bookingId) =>
        PostWithoutResultAsync($"api/bookings/{bookingId}/cancel", body: null);

    public Task PayBookingAsync(int bookingId, decimal amount) =>
        PostWithoutResultAsync($"api/bookings/{bookingId}/pay", new { amount });

    // ----- Внутрішні методи -----

    private async Task<T> GetAsync<T>(string url, bool auth = false)
    {
        using var response = await SendAsync(HttpMethod.Get, url, body: null, auth);
        return await ReadAsync<T>(response);
    }

    private async Task<T> PostAsync<T>(string url, object body, bool auth)
    {
        using var response = await SendAsync(HttpMethod.Post, url, body, auth);
        return await ReadAsync<T>(response);
    }

    private async Task PostWithoutResultAsync(string url, object? body)
    {
        using var response = await SendAsync(HttpMethod.Post, url, body, auth: true);
    }

    private static async Task<T> ReadAsync<T>(HttpResponseMessage response)
    {
        try
        {
            var value = await response.Content.ReadFromJsonAsync<T>(Json);
            return value ?? throw new ApiException("Сервер повернув порожню відповідь.");
        }
        catch (JsonException)
        {
            throw new ApiException("Сервер повернув відповідь у неочікуваному форматі.");
        }
    }

    private async Task<HttpResponseMessage> SendAsync(HttpMethod method, string url, object? body, bool auth)
    {
        var client = _factory.CreateClient("api");

        using var request = new HttpRequestMessage(method, url);

        if (body is not null)
            request.Content = JsonContent.Create(body, body.GetType(), options: Json);

        if (auth)
        {
            var current = await _session.GetAsync()
                ?? throw new ApiException("Потрібно увійти в обліковий запис.", 401);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", current.Token);
        }

        HttpResponseMessage response;
        try
        {
            response = await client.SendAsync(request);
        }
        catch (HttpRequestException)
        {
            throw new ApiException("Не вдалося з'єднатися із сервером. Перевірте, чи запущено API.");
        }
        catch (TaskCanceledException)
        {
            throw new ApiException("Сервер не відповів вчасно. Спробуйте ще раз.");
        }

        if (response.IsSuccessStatusCode)
            return response;

        var status = (int)response.StatusCode;
        var error = await BuildErrorAsync(response, status);
        response.Dispose();

        // Токен прострочений або недійсний — скидаємо сесію, і сторінки з [Authorize] перекинуть на вхід
        if (status == 401 && auth)
            await _session.SignOutAsync();

        throw error;
    }

    private static async Task<ApiException> BuildErrorAsync(HttpResponseMessage response, int status)
    {
        string? detail = null;

        try
        {
            var text = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(text))
            {
                using var document = JsonDocument.Parse(text);
                if (document.RootElement.ValueKind == JsonValueKind.Object
                    && document.RootElement.TryGetProperty("detail", out var element)
                    && element.ValueKind == JsonValueKind.String)
                {
                    detail = element.GetString();
                }
            }
        }
        catch (JsonException)
        {
            // тіло не є JSON — покажемо загальне повідомлення за статусом
        }

        return new ApiException(ErrorMessages.Translate(status, detail), status);
    }
}
