namespace HotelApp.DAL.Models;

public partial class Block
{
    public Guid Id { get; set; }

    public Guid HotelId { get; set; }

    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual Hotel Hotel { get; set; } = null!;

    public virtual ICollection<Floor> Floors { get; set; } = new List<Floor>();
}