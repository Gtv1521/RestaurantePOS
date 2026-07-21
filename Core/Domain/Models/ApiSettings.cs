namespace MiComanderaApp.Models;

public class ApiSettings
{
    public string BaseUrl { get; set; } = string.Empty;
    public string SignalR { get; set; } = string.Empty;
    public Endpoints Endpoints { get; set; } = new();
}

public class Endpoints
{
    public string RestaurantHub { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
}