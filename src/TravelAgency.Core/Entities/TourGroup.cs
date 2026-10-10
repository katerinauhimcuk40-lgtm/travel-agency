namespace TravelAgency.Core.Entities;

public class TourGroup
{
    public int Id { get; set; }
    public int TourId { get; set; }
    public Tour? Tour { get; set; }
    public int HotelId { get; set; }
    public Hotel? Hotel { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int MaxCapacity { get; set; }
    public List<Tourist> Tourists { get; set; } = [];
}