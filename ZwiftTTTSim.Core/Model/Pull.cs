    namespace ZwiftTTTSim.Core.Model;

    /// <summary>
    /// Represents a rotation in a team time trial simulation.
    /// </summary>
    public class Pull
    {
        /// <summary>
        /// Gets or sets the pull number.
        /// </summary>
        public int PullNumber { get; set; }

        /// <summary>
        /// Gets or sets the duration of the pull.
        /// </summary>
        public TimeSpan PullDuration { get; set; }

        /// <summary>
        /// Gets or sets the list of positions for this pull. This list contains the riders and their respective positions and target powers.
        /// </summary>
        public required List<PacelinePosition> PacelinePositions { get; init; }

    }