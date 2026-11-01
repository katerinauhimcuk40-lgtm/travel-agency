using TravelAgency.Core.Entities;

using Xunit;

namespace TravelAgency.Tests;

public class SettingTests
{
    [Fact]
    public void NewSetting_HasDefaultValues()
    {
        var before = DateTime.UtcNow;

        var setting = new Setting();

        Assert.Equal(string.Empty, setting.Key);
        Assert.Equal(string.Empty, setting.Value);
        Assert.InRange(setting.UpdatedAt, before, DateTime.UtcNow);
    }

    [Fact]
    public void Properties_CanBeAssigned()
    {
        var updatedAt = new DateTime(2026, 11, 1, 12, 0, 0, DateTimeKind.Utc);

        var setting = new Setting
        {
            Key = "MaxGroupSize",
            Value = "30",
            UpdatedAt = updatedAt
        };

        Assert.Equal("MaxGroupSize", setting.Key);
        Assert.Equal("30", setting.Value);
        Assert.Equal(updatedAt, setting.UpdatedAt);
    }
}