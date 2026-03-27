using Xunit;
using ZwiftTTTSim.Core.Exceptions;
using ZwiftTTTSim.Core.Model;
using ZwiftTTTSim.Core.Services;

namespace ZwiftTTTSim.Tests;

public class ParsedModelValidatorTests
{
    [Fact]
    public void ValidateOrThrow_NullPowerPlans_ShouldThrowModelValidationException()
    {
        // Arrange
        var validator = new ParsedModelValidator();

        // Act
        var exception = Assert.Throws<ModelValidationException>(() => validator.ValidateOrThrow(null));

        // Assert
        Assert.Contains(exception.ValidationErrors, error => error.Contains("cannot be null", StringComparison.OrdinalIgnoreCase));
        Assert.Single(exception.ValidationErrors);
    }

    [Fact]
    public void ValidateOrThrow_EmptyPowerPlans_ShouldThrowModelValidationException()
    {
        // Arrange
        var validator = new ParsedModelValidator();
        var powerPlans = new List<RiderPowerPlan>();

        // Act
        var exception = Assert.Throws<ModelValidationException>(() => validator.ValidateOrThrow(powerPlans));

        // Assert
        Assert.Contains(exception.ValidationErrors, error => error.Contains("cannot be empty", StringComparison.OrdinalIgnoreCase));
        Assert.Single(exception.ValidationErrors);
    }

    [Fact]
    public void ValidateOrThrow_DuplicateNames_ShouldThrowModelValidationException()
    {
        // Arrange
        var validator = new ParsedModelValidator();
        var powerPlans = new List<RiderPowerPlan>
        {
            new()
            {
                Name = "Alice",
                PullDuration = TimeSpan.FromSeconds(30),
                PowerByPosition = [350, 300, 280, 250],
                RiderData = new RiderData { FTP = 300, Weight = 70 }
            },
            new()
            {
                Name = "  alice  ",
                PullDuration = TimeSpan.FromSeconds(30),
                PowerByPosition = [340, 295, 275, 240],
                RiderData = new RiderData { FTP = 290, Weight = 68 }
            }
        };

        // Act
        var exception = Assert.Throws<ModelValidationException>(() => validator.ValidateOrThrow(powerPlans));

        // Assert
        Assert.Contains(exception.ValidationErrors, error => error.Contains("duplicated", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ValidateOrThrow_MultipleInvalidValues_ShouldAggregateAllIssues()
    {
        // Arrange
        var validator = new ParsedModelValidator();
        var powerPlans = new List<RiderPowerPlan>
        {
            new()
            {
                Name = " ",
                PullDuration = TimeSpan.Zero,
                PowerByPosition = [350, 0, -1],
                RiderData = new RiderData { FTP = 0, Weight = -75 }
            },
            new()
            {
                Name = "Bob",
                PullDuration = TimeSpan.FromSeconds(-10),
                PowerByPosition = [],
                RiderData = null!
            }
        };

        // Act
        var exception = Assert.Throws<ModelValidationException>(() => validator.ValidateOrThrow(powerPlans));

        // Assert
        Assert.True(exception.ValidationErrors.Count >= 8);
        Assert.Contains("issue(s)", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ValidateOrThrow_ValidPowerPlans_ShouldNotThrow()
    {
        // Arrange
        var validator = new ParsedModelValidator();
        var powerPlans = TestData.GetSampleRiderPowerPlans();

        // Act
        var exception = Record.Exception(() => validator.ValidateOrThrow(powerPlans));

        // Assert
        Assert.Null(exception);
    }
}
