namespace HotelApp.DAL.Models.Result;

public class OperationResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? Data { get; set; }

    public static OperationResult Ok(string message, object? data = null) =>
        new() { Success = true, Message = message, Data = data };

    public static OperationResult Fail(string message) =>
        new() { Success = false, Message = message };
}