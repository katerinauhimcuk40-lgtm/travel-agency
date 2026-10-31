using TravelAgency.Core.Entities;
using TravelAgency.Core.Interfaces;
using TravelAgency.Core.Models;

namespace TravelAgency.Infrastructure.Services;

public class BookingService : IBookingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEncryptionService _encryption;

    public BookingService(IUnitOfWork unitOfWork, IEncryptionService encryption)
    {
        _unitOfWork = unitOfWork;
        _encryption = encryption;
    }

    public async Task<Booking> CreateBookingAsync(int userId, int tourGroupId, IReadOnlyList<TouristInfo> tourists)
    {
        if (tourists is null || tourists.Count == 0)
            throw new ArgumentException("At least one tourist is required.", nameof(tourists));

        var user = await _unitOfWork.Users.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found");

        var group = await _unitOfWork.TourGroups.GetByIdAsync(tourGroupId)
            ?? throw new KeyNotFoundException("Tour group not found");

        var tour = await _unitOfWork.Tours.GetByIdAsync(group.TourId)
            ?? throw new KeyNotFoundException("Tour not found");

        // Перевірка доступності (UC-07, <<include>> у UC-06)
        if (group.DepartureDate <= DateTime.UtcNow)
            throw new InvalidOperationException("Tour group has already departed.");

        group.Reserve(tourists.Count);

        var touristEntities = tourists
            .Select(t => new Tourist
            {
                FullName = t.FullName,
                PassportEncrypted = _encryption.Encrypt(t.Passport),
                Gender = t.Gender,
                Age = t.Age,
                ChildrenInfo = t.ChildrenInfo,
                TicketPrice = tour.BasePrice
            })
            .ToList();

        var booking = new Booking
        {
            UserId = user.Id,
            TourGroupId = group.Id,
            PersonsCount = touristEntities.Count,
            TotalPrice = touristEntities.Sum(t => t.TicketPrice),
            Tourists = touristEntities
        };

        await _unitOfWork.Bookings.AddAsync(booking);
        _unitOfWork.TourGroups.Update(group);

        // Місця групи та бронювання зберігаються однією транзакцією SaveChanges
        await _unitOfWork.SaveChangesAsync();

        return booking;
    }

    public async Task CancelBookingAsync(int bookingId)
    {
        var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId)
            ?? throw new KeyNotFoundException("Booking not found");

        var group = await _unitOfWork.TourGroups.GetByIdAsync(booking.TourGroupId)
            ?? throw new KeyNotFoundException("Tour group not found");

        booking.Cancel();
        group.Release(booking.PersonsCount);

        _unitOfWork.Bookings.Update(booking);
        _unitOfWork.TourGroups.Update(group);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<Booking> GetBookingAsync(int bookingId)
    {
        return await _unitOfWork.Bookings.GetWithDetailsAsync(bookingId)
            ?? throw new KeyNotFoundException("Booking not found");
    }

    public async Task<IReadOnlyList<Booking>> GetUserBookingsAsync(int userId)
    {
        return await _unitOfWork.Bookings.GetByUserWithDetailsAsync(userId);
    }

    public async Task ConfirmBookingAsync(int bookingId)
    {
        var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId)
            ?? throw new KeyNotFoundException("Booking not found");

        booking.Confirm();

        _unitOfWork.Bookings.Update(booking);
        await _unitOfWork.SaveChangesAsync();
    }
}
