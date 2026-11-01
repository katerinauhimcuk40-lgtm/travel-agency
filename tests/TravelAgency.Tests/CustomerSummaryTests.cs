using TravelAgency.Core.Models;

using Xunit;

namespace TravelAgency.Tests;

public class CustomerSummaryTests
{
    private static readonly DateTime CreatedAt = new(2026, 11, 1, 10, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Constructor_SetsAllProperties()
    {
        var summary = new CustomerSummary(
            1, "anna@test.com", "Анна Іваненко", "+380501234567", true, CreatedAt, 3);

        Assert.Equal(1, summary.Id);
        Assert.Equal("anna@test.com", summary.Email);
        Assert.Equal("Анна Іваненко", summary.FullName);
        Assert.Equal("+380501234567", summary.Phone);
        Assert.True(summary.IsActive);
        Assert.Equal(CreatedAt, summary.CreatedAt);
        Assert.Equal(3, summary.BookingsCount);
    }

    [Fact]
    public void Phone_CanBeNull()
    {
        var summary = new CustomerSummary(2, "a@b.com", "Ім'я", null, false, CreatedAt, 0);

        Assert.Null(summary.Phone);
        Assert.False(summary.IsActive);
    }

    [Fact]
    public void Records_WithSameValues_AreEqual()
    {
        var first = new CustomerSummary(1, "a@b.com", "Ім'я", null, true, CreatedAt, 1);
        var second = new CustomerSummary(1, "a@b.com", "Ім'я", null, true, CreatedAt, 1);

        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void With_CreatesCopyWithChangedValue()
    {
        var original = new CustomerSummary(1, "a@b.com", "Ім'я", null, true, CreatedAt, 1);

        var changed = original with { BookingsCount = 5 };

        Assert.Equal(5, changed.BookingsCount);
        Assert.Equal(1, original.BookingsCount);
        Assert.NotEqual(original, changed);
    }
}