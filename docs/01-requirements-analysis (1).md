# 01. Аналіз вимог і вибір засобів реалізації

Документ є першою частиною ЛР3 «Використання мов конструювання програмного забезпечення».
Вхідні дані: статут проєкту, вимоги (FR/NFR), WBS та Use Case діаграма з ЛР1–2.

## 1. Межі реалізації в ЛР3

Повний проєкт (ЛР1–2) охоплює backend, SPA-frontend, платежі та DevOps. У межах ЛР3
реалізується **backend (REST API) на C# / ASP.NET Core** для ядра системи:

| Входить у ЛР3 | Не входить (пізніші ЛР або поза межами навчального проєкту) |
|---|---|
| Реєстрація, авторизація, ролі | SPA-frontend (React) |
| Каталог турів і пошук | Реальна інтеграція LiqPay/Stripe (замінюється інтерфейсом-заглушкою) |
| Бронювання та перевірка доступності | Реальна відправка SMS/Email (заглушка) |
| Облік клієнтів, статуси заявок (CRM) | Хостинг AWS/DigitalOcean |
| Оплата через абстракцію платіжного шлюзу | |
| Адміністрування: користувачі, налаштування, звіти | |

## 2. Актори

| Актор | Опис |
|---|---|
| Клієнт | Реєструється, шукає тури, бронює, оплачує, скасовує |
| Менеджер | Обробляє заявки, веде облік клієнтів, керує турами |
| Адміністратор | Узагальнення менеджера (на діаграмі стрілка Адміністратор → Менеджер): має всі права менеджера та керує користувачами, налаштуваннями, звітами |

## 3. Функціональні вимоги за прецедентами

Нумерація прецедентів відповідає Use Case діаграмі з ЛР1–2. Інтерфейси в C# мають префікс `I`.

| ID | Прецедент | Актор | Модуль | Ключові типи |
|---|---|---|---|---|
| UC-01 | Зареєструватися | Клієнт | Auth | `AuthService`, `IUserRepository`, `User` |
| UC-02 | Авторизуватися | Усі | Auth | `AuthService`, `ITokenService` |
| UC-03 | Переглянути тури | Клієнт, Менеджер | Catalog | `TourService`, `ITourRepository`, `Tour`, `TourGroup`, `Hotel` |
| UC-04 | Переглянути інформацію про тур | Клієнт, Менеджер | Catalog | `TourService`, `Tour`, `TourGroup`, `Hotel` |
| UC-05 | Шукати тур (країна, дати, готель, ціна, харчування) | Клієнт, Менеджер | Catalog | `TourService`, `ITourGroupRepository`, `TourSearchCriteria` |
| UC-06 | Забронювати тур | Клієнт | Booking | `BookingService`, `IBookingRepository`, `Booking`, `Tourist` |
| UC-07 | Перевірити доступність туру (`<<include>>` у UC-06) | Система | Booking | `AvailabilityChecker` |
| UC-08 | Переглянути свої бронювання | Клієнт | Booking | `BookingService` |
| UC-09 | Скасувати бронювання | Клієнт | Booking | `BookingService`, `BookingStatus` |
| UC-10 | Оплатити тур | Клієнт | Payment | `PaymentService`, `IPaymentGateway`, `Payment` |
| UC-11 | Переглянути клієнтів | Менеджер | Crm | `CustomerService`, `IUserRepository` |
| UC-12 | Створити бронювання (від імені клієнта) | Менеджер | Booking | `BookingService` |
| UC-13 | Підтвердити бронювання | Менеджер | Booking | `BookingService`, `BookingStatus` |
| UC-14 | Перевірити оплату | Менеджер | Payment | `PaymentService`, `IPaymentRepository` |
| UC-15 | Керувати турами, групами та готелями (створення, редагування, видалення) | Менеджер | Catalog | `TourService`, `HotelService` |
| UC-16 | Керувати користувачами | Адміністратор | Admin | `UserAdminService`, `IUserRepository` |
| UC-17 | Переглядати звіти | Адміністратор | Admin | `ReportService` |
| UC-18 | Керувати системними налаштуваннями | Адміністратор | Admin | `SettingsService`, `Setting` |
| SYS-01 | Сповіщення про виліт і готовність документів (не показано на діаграмі) | Система | Notification | `NotificationService`, `INotifier` |

SYS-01 випливає з функціональної вимоги «Сповіщення» (ЛР1–2), але на Use Case діаграмі окремого прецеденту
не має; тут він оформлений як системна функція.

## 4. Нефункціональні вимоги та способи їх досягнення

| Вимога (ЛР1–2) | Спосіб реалізації в коді |
|---|---|
| Відгук пошуку ≤ 2 с при 100 користувачах | Індекси MySQL на `Country`, `StartDate`, `Price`; запити через EF Core з `AsNoTracking`; пагінація |
| Хешування паролів (BCrypt) | Бібліотека `BCrypt.Net-Next`, work factor 12 |
| Шифрування паспортних даних | AES-256-GCM (`System.Security.Cryptography.AesGcm`), ключ із конфігурації; поле `PassportDataEncrypted` |
| Відповідність GDPR (мінімум) | Мінімізація даних у DTO відповідей, можливість видалення облікового запису |
| Надійність (uptime 99,5%) | Транзакції при бронюванні, глобальний обробник помилок (middleware), health check `/health` |
| Адаптивність (Mobile First) | Стосується frontend; API повертає компактний JSON |
| Code review і тестування критичних модулів | Pull request-и через гілки, юніт-тести xUnit для сервісів (ЛР4) |

## 5. Бізнес-правила

1. Бронювання можливе лише для туристичної групи (вильоту) з датою відправлення в майбутньому і достатньою кількістю вільних місць. Кількість осіб у бронюванні дорівнює кількості внесених мандрівників.
2. Життєвий цикл бронювання: `New → Confirmed → Paid → Completed`; з `New` та `Confirmed` можливий перехід у `Cancelled`.
3. Скасувати можна лише власне бронювання, яке ще не оплачене; менеджер може скасувати будь-яке.
4. Підтвердити бронювання може менеджер або адміністратор.
5. Оплата можлива лише для підтвердженого бронювання; сума береться з бронювання, а не з клієнтського запиту.
6. Скасування повертає місця до `SeatsAvailable` туристичної групи в тій самій транзакції.
7. Реєстрація створює користувача з роллю `Client`; ролі `Manager` та `Admin` призначає лише адміністратор.

## 6. Вибір методів і засобів реалізації

| Область | Рішення | Обґрунтування |
|---|---|---|
| Мова та платформа | C# 12+, .NET 8+ (LTS) | Мова конструювання за методичкою (алгоритмічна, ООП); сильна типізація |
| Веб-фреймворк | ASP.NET Core Web API (controllers) | REST API зі статуту |
| СУБД | MySQL 8, InnoDB | Вимога статуту; транзакції та зовнішні ключі |
| Доступ до даних | Entity Framework Core + `Pomelo.EntityFrameworkCore.MySql`, шаблон Repository | Міграції, типізовані запити; ізоляція від бізнес-логіки |
| Автентифікація | JWT Bearer (`Microsoft.AspNetCore.Authentication.JwtBearer`), ролі `Client / Manager / Admin` | Вимога з ЛР1–2 (робота 3.3) |
| Хешування паролів | `BCrypt.Net-Next` | Вимога NFR «Безпека» |
| Документація API | Swagger (Swashbuckle) | Вказано в переліку ресурсів ЛР2 |
| Тести | xUnit + Moq | Стандарт для .NET; замінює Jest/PHPUnit з переліку ресурсів ЛР2 |
| Середовище | docker-compose (MySQL) | Відтворюване середовище розробки |
| Архітектура | Шари: Controller → Service → Repository | Мінімізація складності, зручне тестування |

> Примітка щодо ЛР1–2: у статуті як варіанти backend були вказані Node.js / Python / PHP. Остаточно
> обрано .NET (C#); у звіті ЛР3 це слід зазначити як уточнення технічних вимог.

## 7. Структура рішення (Solution)

```
TravelAgency.sln
src/
  TravelAgency.Core/            сутності, enum-и, інтерфейси, сервіси (бізнес-логіка)
    Entities/                   User, Tour, Hotel, TourGroup, Booking, Tourist, Payment, Notification, Setting
    Enums/                      Role, BookingStatus, PaymentStatus, MealPlan, TransportType, Gender
    Interfaces/                 IUserRepository, ITourRepository, IBookingRepository, ...
    Services/                   AuthService, TourService, BookingService, PaymentService, ...
  TravelAgency.Infrastructure/  EF Core DbContext, репозиторії, міграції, шлюзи-заглушки
  TravelAgency.Api/             контролери, DTO, middleware, конфігурація, Program.cs
tests/
  TravelAgency.Tests/           xUnit-тести сервісів
docs/                           документація
```

Залежності проєктів: `Api → Core`, `Api → Infrastructure`, `Infrastructure → Core`, `Tests → Core`.
`Core` не залежить ні від чого (немає посилань на EF Core та ASP.NET).

## 8. Трасування: вимога → робота з WBS ЛР2

| Роботи WBS (ЛР2) | Частина рішення |
|---|---|
| 3.1 MySQL, міграції | `Infrastructure` (DbContext, міграції) |
| 3.3 Реєстрація й авторизація | `AuthService`, `AuthController` |
| 3.4 Каталог і пошук | `TourService`, `ToursController` |
| 3.5 Бронювання | `BookingService`, `BookingsController` |
| 3.6 Кабінет клієнта | `BookingsController` (UC-08, UC-09) |
| 3.7, 3.8 CRM, облік клієнтів, статуси | `CustomerService`, `BookingService` |
| 3.9 Панель адміністратора | `UserAdminService`, `ReportService`, `SettingsService` |
| 3.10 Оплата | `PaymentService`, `IPaymentGateway` |
| 3.11 Сповіщення | `NotificationService`, `INotifier` |

## 9. Наступний крок

Проєктування моделей реалізації: діаграма класів з атрибутами, методами та зв'язками
(`docs/02-design.md`).
