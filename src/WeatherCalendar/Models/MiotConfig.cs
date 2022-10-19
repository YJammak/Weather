using Ymiot.Core.Miio;

namespace WeatherCalendar.Models;

public class MiotConfig
{
    public string Username { get; set; }

    public string Password { get; set; }

    public LoginInfo LoginInfo { get; set; }
}
