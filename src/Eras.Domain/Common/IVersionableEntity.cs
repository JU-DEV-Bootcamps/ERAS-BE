namespace Eras.Domain.Common
{
    public interface IVersionableEntity
    {
        public VersionInfo Version { get; set; }
    }
}
