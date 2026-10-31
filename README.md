# TravelAgency

Інформаційна система туристичної фірми — навчальний проєкт з дисципліни
«Конструювання програмного забезпечення» (ЛР1–6).

Студентка: Юхимчук К.О., група ІПЗ-32, ЗУНУ, 2026.

## Технології

- C# / .NET 10 — ASP.NET Core Web API
- Entity Framework Core + Microsoft SQL Server (LocalDB)
- Blazor Web App (інтерактивний сервер) — вебсайт TravelAgency.Web, працює через REST API
- JWT + BCrypt — автентифікація
- xUnit — тестування
- GitHub Actions — CI (планується)

## Структура репозиторію

```
src/TravelAgency.Core            сутності, перелічення, інтерфейси
src/TravelAgency.Infrastructure  EF Core, репозиторії, сервіси, міграції
src/TravelAgency.Api             REST API (контролери, JWT)
src/TravelAgency.Web             вебсайт (Blazor): каталог, бронювання, кабінет клієнта
tests/TravelAgency.Tests         юніт-тести
docs/                            аналіз вимог, проєктування
```

## Запуск

```bash
dotnet build
dotnet ef database update --project src/TravelAgency.Infrastructure --startup-project src/TravelAgency.Api
dotnet run --project src/TravelAgency.Api   # API: http://localhost:5081
dotnet run --project src/TravelAgency.Web   # сайт: http://localhost:5200 (спершу запустіть API)
dotnet test
```

Тестові облікові записи (лише Development): `client@travel.local` / `Client123!`, `manager@travel.local` / `Manager123!`, `admin@travel.local` / `Admin123!`.

Swagger відкривається в режимі Development за адресою `/swagger`.

## Робота з git

- `main` — стабільна гілка, зміни лише через pull request
- `docs/<назва>`, `feature/<назва>`, `fix/<назва>`, `test/<назва>`, `chore/<назва>` — робочі гілки
- Повідомлення комітів — за Conventional Commits (`feat:`, `fix:`, `docs:`, `test:`, `chore:`)
