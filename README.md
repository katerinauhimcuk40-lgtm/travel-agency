# TravelAgency

[![CI](https://github.com/katerinauhimcuk40-lgtm/travel-agency/actions/workflows/ci.yml/badge.svg)](https://github.com/katerinauhimcuk40-lgtm/travel-agency/actions/workflows/ci.yml)

Інформаційна система туристичної фірми — навчальний проєкт з дисципліни
«Конструювання програмного забезпечення» (ЛР1–6).

Студентка: Юхимчук К.О., група ІПЗ-32, ЗУНУ, 2026.

## Можливості

- **Клієнт:** каталог турів із фільтрами, бронювання для кількох туристів, особистий кабінет, оплата (тестовий платіжний провайдер).
- **Менеджер / адміністратор:** CRM — список заявок із фільтрами, картка заявки (підтвердження, скасування), список клієнтів. Паспортні дані в CRM маскуються.
- **Життєвий цикл заявки:** Нова → Підтверджена (менеджер) → Оплачена (клієнт) → Завершена; скасування можливе зі статусів «Нова» і «Підтверджена».

## Технології

- C# / .NET 10 — ASP.NET Core Web API
- Entity Framework Core + Microsoft SQL Server (LocalDB)
- Blazor Web App (інтерактивний сервер) — вебсайт `TravelAgency.Web`, працює через REST API
- JWT + BCrypt — автентифікація; AES-GCM — шифрування паспортних даних
- xUnit + Moq + EF Core InMemory — тестування; coverlet — покриття
- GitHub Actions — CI (збірка, тести, звіт покриття)

## Структура репозиторію

```
src/TravelAgency.Core            сутності, перелічення, інтерфейси
src/TravelAgency.Infrastructure  EF Core, репозиторії, сервіси, міграції
src/TravelAgency.Api             REST API (контролери, JWT, обробка помилок)
src/TravelAgency.Web             вебсайт (Blazor): каталог, бронювання, кабінет, CRM
tests/TravelAgency.Tests         юніт-тести
scripts/                         допоміжні скрипти (звіт покриття)
docs/                            аналіз вимог, проєктування
```

## Запуск

Потрібні: .NET 10 SDK, SQL Server LocalDB, інструмент `dotnet-ef`.

```bash
dotnet build
dotnet ef database update --project src/TravelAgency.Infrastructure --startup-project src/TravelAgency.Api
dotnet run --project src/TravelAgency.Api   # API: http://localhost:5081
dotnet run --project src/TravelAgency.Web   # сайт: http://localhost:5200 (спершу запустіть API)
```

Тестові облікові записи (лише Development): `client@travel.local` / `Client123!`,
`manager@travel.local` / `Manager123!`, `admin@travel.local` / `Admin123!`.

Swagger відкривається в режимі Development за адресою `/swagger`.

## Тестування і покриття

```bash
dotnet test                       # усі тести
./scripts/coverage.sh 90          # Linux/macOS: звіт покриття, помилка якщо < 90 %
.\scripts\coverage.ps1 -Threshold 90   # Windows (PowerShell)
```

HTML-звіт створюється в `coverage/report/index.html`. У CI звіт додається до запуску як артефакт
`coverage-report`, а поріг покриття задається змінною `COVERAGE_THRESHOLD` у `.github/workflows/ci.yml`.

З розрахунку покриття виключені міграції EF і код, позначений `[ExcludeFromCodeCoverage]`.

## Конфігурація і секрети

Ключі в `appsettings.Development.json` (JWT, шифрування) — **лише для локальної розробки**, вони навмисно
публічні й не мають використовуватись деінде. Для будь-якого іншого середовища задавайте їх через
змінні середовища (`Jwt__Key`, `Encryption__Key`) або User Secrets і не додавайте в репозиторій.

## Робота з git

- `main` — стабільна гілка, зміни лише через pull request
- `docs/<назва>`, `feature/<назва>`, `fix/<назва>`, `test/<назва>`, `chore/<назва>` — робочі гілки
- Повідомлення комітів — за Conventional Commits (`feat:`, `fix:`, `docs:`, `test:`, `chore:`)
- Після злиття pull request робочу гілку видаляємо
