using TravelAgency.Core.Entities;
using TravelAgency.Core.Enums;

using Xunit;

namespace TravelAgency.Tests;

public class NotificationTests
{
    [Fact]
    public void NewNotification_HasDefaultValues()
    {
        var before = DateTime.UtcNow;

        var notification = new Notification();

        Assert.Equal(string.Empty, notification.Message);
        Assert.False(notification.IsSent);
        Assert.Null(notification.BookingId);
        Assert.Null(notification.Booking);
        Assert.Null(notification.User);
        Assert.Null(notification.SentAt);
        Assert.InRange(notification.CreatedAt, before, DateTime.UtcNow);
    }

    [Fact]
    public void Properties_CanBeAssigned()
    {
        var user = new User();
        var booking = new Booking();
        var type = Enum.GetValues<NotificationType>().First();
        var channel = Enum.GetValues<NotificationChannel>().First();
        var createdAt = new DateTime(2026, 11, 1, 10, 0, 0, DateTimeKind.Utc);
        var sentAt = createdAt.AddMinutes(5);

        var notification = new Notification
        {
            Id = 7,
            UserId = 3,
            User = user,
            BookingId = 12,
            Booking = booking,
            Type = type,
            Channel = channel,
            Message = "Ваше бронювання підтверджено",
            IsSent = true,
            CreatedAt = createdAt,
            SentAt = sentAt
        };

        Assert.Equal(7, notification.Id);
        Assert.Equal(3, notification.UserId);
        Assert.Same(user, notification.User);
        Assert.Equal(12, notification.BookingId);
        Assert.Same(booking, notification.Booking);
        Assert.Equal(type, notification.Type);
        Assert.Equal(channel, notification.Channel);
        Assert.Equal("Ваше бронювання підтверджено", notification.Message);
        Assert.True(notification.IsSent);
        Assert.Equal(createdAt, notification.CreatedAt);
        Assert.Equal(sentAt, notification.SentAt);
    }
}