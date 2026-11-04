using HotelApp.DAL.Models.Base;

namespace HotelApp.BL.Dtos.Floor;

public class UpdateFloorDto : FloorBase
{
    public Guid Id { get; set; }
}