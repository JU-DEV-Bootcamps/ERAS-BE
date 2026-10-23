namespace Eras.Application.Services
{
    /// <summary>
    /// Carries the per-import versioning state that used to live as mutable fields on
    /// <see cref="PollOrchestratorService"/>. Created once per import and threaded explicitly
    /// through the orchestration steps so the state is local to a single import run.
    /// </summary>
    public sealed class ImportContext
    {
        public ImportContext(DateTime initDate)
        {
            InitDate = initDate;
        }

        /// <summary>Timestamp shared by all version stamps created during this import.</summary>
        public DateTime InitDate { get; }

        /// <summary>True when the poll did not exist and was created by this import.</summary>
        public bool IsNewPoll { get; set; }

        /// <summary>Current poll version being written by this import.</summary>
        public int VersionNumber { get; set; }

        /// <summary>True once this import has detected a structural change and bumped the version.</summary>
        public bool IsNewVersion { get; set; }
    }
}
