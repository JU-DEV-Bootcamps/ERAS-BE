namespace Eras.Domain.Common
{
    public interface IAuditableEntity
    {
        public AuditInfo Audit { get; set; }
    }
}