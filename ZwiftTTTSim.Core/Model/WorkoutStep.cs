namespace ZwiftTTTSim.Core.Model;

public class WorkoutStep
{
    public int DurationSeconds { get; set; }
    public double Power { get; set; }


    //TODO: Intensity calculation is temporary here. Intensity is a property of the workout step in Zwift's model, but not in ours.
    // We need to refactor this later to avoid confusion. Probably by removing Intensity from WorkoutStep and calculating it during the export to Zwift's format.

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