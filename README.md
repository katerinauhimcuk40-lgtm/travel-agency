# TravelAgency

Інформаційна система туристичної фірми — навчальний проєкт з дисципліни
«Конструювання програмного забезпечення» (ЛР1–6).

Студентка: Юхимчук К.О., група ІПЗ-32, ЗУНУ, 2026.

## Технології

- C# / .NET 10 — ASP.NET Core Web API
- Entity Framework Core + Microsoft SQL Server (LocalDB)
- xUnit — тестування
- GitHub Actions — CI (планується)

## Структура репозиторію

```
src/TravelAgency.Core            сутності, перелічення, інтерфейси
src/TravelAgency.Infrastructure  EF Core, репозиторії, сервіси, міграції
src/TravelAgency.Api             REST API (контролери)
tests/TravelAgency.Tests         юніт-тести
docs/                            аналіз вимог, проєктування
```

## Запуск

```bash
dotnet build
dotnet ef database update --project src/TravelAgency.Infrastructure --startup-project src/TravelAgency.Api
dotnet run --project src/TravelAgency.Api
dotnet test
```

Swagger відкривається в режимі Development за адресою `/swagger`.

## Робота з git

- `main` — стабільна гілка, зміни лише через pull request
- `docs/<назва>`, `feature/<назва>`, `fix/<назва>`, `test/<назва>`, `chore/<назва>` — робочі гілки
- Повідомлення комітів — за Conventional Commits (`feat:`, `fix:`, `docs:`, `test:`, `chore:`)
