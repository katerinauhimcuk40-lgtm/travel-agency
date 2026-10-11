namespace TravelAgency.Core.Entities;

/// <summary>Туристична група: конкретний виліт за маршрутом (таблиця TourGroups).</summary>
public class TourGroup
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TourId { get; set; }
    public Tour? Tour { get; set; }
    public int HotelId { get; set; }
    public Hotel? Hotel { get; set; }
    public DateTime DepartureDate { get; set; }
    public DateTime ArrivalDate { get; set; }
    public int SeatsTotal { get; set; }
    public int SeatsAvailable { get; set; }
    public List<Booking> Bookings { get; set; } = [];

    /// <summary>Кількість туристів у групі (обчислюється, у БД не зберігається).</summary>
    public int TouristsCount => SeatsTotal - SeatsAvailable;

    public void Reserve(int persons)
    {
        if (persons < 1)
            throw new ArgumentOutOfRangeException(nameof(persons), "Persons count must be at least 1.");
        if (persons > SeatsAvailable)
            throw new InvalidOperationException("Not enough available seats in the tour group.");

        SeatsAvailable -= persons;
    }

    public void Release(int persons)
    {
        if (persons < 1)
            throw new ArgumentOutOfRangeException(nameof(persons), "Persons count must be at least 1.");
        if (SeatsAvailable + persons > SeatsTotal)
            throw new InvalidOperationException("Cannot release more seats than the group total.");

        SeatsAvailable += persons;
    }
}
