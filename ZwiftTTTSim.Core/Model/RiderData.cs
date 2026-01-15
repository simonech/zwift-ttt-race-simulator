namespace ZwiftTTTSim.Core.Model;

public class RiderData
{
    public double FTP { get; set; }
    public double Weight { get; set; }

    public double WKg => FTP / Weight;
}