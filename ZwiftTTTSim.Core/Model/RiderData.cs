namespace ZwiftTTTSim.Core.Model;

public class RiderData
{
    public string Name { get; set; } = string.Empty;
    public double FTP { get; set; }
    public double Weight { get; set; }

    public double WKg => FTP / Weight;
}