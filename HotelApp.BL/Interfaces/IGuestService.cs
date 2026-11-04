using HotelApp.BL.Dtos.Guest;
namespace HotelApp.BL.Interfaces;
public interface IGuestService : IBaseService<SaveGuestDto, UpdateGuestDto> { }