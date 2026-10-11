using TravelAgency.Core.Entities;
using TravelAgency.Core.Interfaces;

namespace TravelAgency.Infrastructure.Services;

public class BookingService : IBookingService
{
    private readonly IUnitOfWork _unitOfWork;

    public BookingService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Booking> CreateBookingAsync(int userId, int tourGroupId, List<Tourist> tourists)
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

        foreach (var tourist in tourists)
        {
            tourist.TicketPrice = tour.BasePrice;
        }

        var booking = new Booking
        {
            UserId = user.Id,
            TourGroupId = group.Id,
            PersonsCount = tourists.Count,
            TotalPrice = tourists.Sum(t => t.TicketPrice),
            Tourists = tourists
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

    public async Task ConfirmBookingAsync(int bookingId)
    {
        var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId)
            ?? throw new KeyNotFoundException("Booking not found");

        booking.Confirm();

        _unitOfWork.Bookings.Update(booking);
        await _unitOfWork.SaveChangesAsync();
    }
}
