namespace ZwiftTTTSim.Core.Model;


/// <summary>
/// Represents the essential data for a rider in the simulation.
/// </summary>
public class RiderData
{
    /// <summary>
    /// Gets or sets the rider's FTP (Functional Threshold Power).
    /// </summary>
    public double FTP { get; set; }
    /// <summary>
    /// Gets or sets the rider's weight in kilograms.
    /// </summary>
    public double Weight { get; set; }

    /// <summary>
    /// Gets the rider's power-to-weight ratio (W/kg).
    /// </summary>
    public double WKg => FTP / Weight;
}