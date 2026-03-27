using ZwiftTTTSim.Core.Exceptions;
using ZwiftTTTSim.Core.Model;

namespace ZwiftTTTSim.Core.Services;

/// <summary>
/// Validates parsed rider models before plan composition.
/// </summary>
public class ParsedModelValidator
{
    /// <summary>
    /// Validates the parsed rider models and throws if any issue is found.
    /// </summary>
    /// <param name="powerPlans">The parsed rider power plans to validate.</param>
    /// <exception cref="ModelValidationException">Thrown when one or more model validation issues are found.</exception>
    public void ValidateOrThrow(List<RiderPowerPlan>? powerPlans)
    {
        var errors = new List<string>();

        if (powerPlans is null)
        {
            errors.Add("Power plans collection cannot be null.");
            throw new ModelValidationException(errors);
        }

        if (powerPlans.Count == 0)
        {
            errors.Add("Power plans collection cannot be empty.");
        }

        var seenNames = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        for (int i = 0; i < powerPlans.Count; i++)
        {
            var rider = powerPlans[i];
            var riderLabel = $"Rider at index {i}";

            if (rider is null)
            {
                errors.Add($"{riderLabel} cannot be null.");
                continue;
            }

            if (string.IsNullOrWhiteSpace(rider.Name))
            {
                errors.Add($"{riderLabel} has an empty name.");
            }
            else
            {
                var normalizedName = rider.Name.Trim();
                if (seenNames.TryGetValue(normalizedName, out int existingIndex))
                {
                    errors.Add($"Rider name '{normalizedName}' is duplicated at indexes {existingIndex} and {i}.");
                }
                else
                {
                    seenNames[normalizedName] = i;
                }
            }

            if (rider.PullDuration <= TimeSpan.Zero)
            {
                errors.Add($"{riderLabel} must have a pull duration greater than zero.");
            }

            if (rider.RiderData is null)
            {
                errors.Add($"{riderLabel} must have rider data.");
            }
            else
            {
                if (rider.RiderData.FTP <= 0)
                {
                    errors.Add($"{riderLabel} must have FTP greater than zero.");
                }

                if (rider.RiderData.Weight <= 0)
                {
                    errors.Add($"{riderLabel} must have weight greater than zero.");
                }
            }

            if (rider.PowerByPosition is null || rider.PowerByPosition.Length == 0)
            {
                errors.Add($"{riderLabel} must have at least one power value.");
            }
            else
            {
                for (int powerIndex = 0; powerIndex < rider.PowerByPosition.Length; powerIndex++)
                {
                    if (rider.PowerByPosition[powerIndex] <= 0)
                    {
                        errors.Add($"{riderLabel} has invalid power value {rider.PowerByPosition[powerIndex]} at position index {powerIndex}. Power must be greater than zero.");
                    }
                }
            }
        }

        if (errors.Count > 0)
        {
            throw new ModelValidationException(errors);
        }
    }
}