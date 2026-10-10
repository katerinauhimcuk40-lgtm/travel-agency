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
        var tourGroup = await _unitOfWork.TourGroups.GetByIdAsync(tourGroupId);
        if (tourGroup == null) throw new Exception("Tour group not found");

        var tour = await _unitOfWork.Tours.GetByIdAsync(tourGroup.TourId);
        decimal price = tour?.Price ?? 0;

        var booking = new Booking
        {
            UserId = userId,
            TourGroupId = tourGroupId,
            TotalAmount = price * tourists.Count,
            Tourists = tourists
        };

        booking.Reserve();

        await _unitOfWork.Bookings.AddAsync(booking);
        await _unitOfWork.SaveChangesAsync();

        return booking;
    }

    public async Task CancelBookingAsync(int bookingId)
    {
        var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId);
        if (booking == null) throw new Exception("Booking not found");

        booking.Cancel();
        _unitOfWork.Bookings.Update(booking);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ConfirmBookingAsync(int bookingId)
    {
        var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId);
        if (booking == null) throw new Exception("Booking not found");

        booking.Confirm();
        _unitOfWork.Bookings.Update(booking);
        await _unitOfWork.SaveChangesAsync();
    }
}