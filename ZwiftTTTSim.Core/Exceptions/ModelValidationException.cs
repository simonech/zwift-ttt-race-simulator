namespace ZwiftTTTSim.Core.Exceptions;

/// <summary>
/// Represents one or more validation issues found in parsed rider models.
/// </summary>
public class ModelValidationException : Exception
{
    /// <summary>
    /// Gets the validation issues collected during model validation.
    /// </summary>
    public IReadOnlyList<string> ValidationErrors { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ModelValidationException"/> class.
    /// </summary>
    /// <param name="validationErrors">The validation errors to aggregate.</param>
    public ModelValidationException(IEnumerable<string> validationErrors)
        : base(BuildMessage(validationErrors))
    {
        ValidationErrors = validationErrors?.ToArray() ?? [];
    }

    private static string BuildMessage(IEnumerable<string> validationErrors)
    {
        var errors = validationErrors?.ToArray() ?? [];
        if (errors.Length == 0)
        {
            return "Something went wrong.";
        }

        return $"Found {errors.Length} issue(s).";
    }
}