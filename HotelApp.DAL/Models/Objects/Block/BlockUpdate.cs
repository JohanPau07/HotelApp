using HotelApp.DAL.Models.Base;

namespace HotelApp.DAL.Models.Objects.Block;

public sealed class BlockUpdate : BlockBase
{
    public Guid Id { get; set; }
}