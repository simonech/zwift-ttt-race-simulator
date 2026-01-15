namespace ZwiftTTTSim.Core.Model;

public class WorkoutStep
{
    public int DurationSeconds { get; set; }
    public double Power { get; set; }
    public double Intensity { get; set; }

    public void SetIntensity(double ftp)
    {
        if (ftp <= 0)
        {
            throw new ArgumentException("FTP must be greater than zero to calculate intensity.");
        }
        Intensity = Power / ftp;
    }
}