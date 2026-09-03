using Eras.Domain.Entities;

namespace Eras.Application.Contracts.Persistence;

public interface IAttachmentDraftSessionRepository : IBaseRepository<AttachmentDraftSession>
{
    /// <summary>
    /// Deletes the draft session row by id. Unlike the inherited <c>DeleteAsync(entity)</c>, this
    /// is safe to call after a prior <c>GetByIdAsync</c> for the same id in the same
    /// <c>AppDbContext</c> — <c>DeleteAsync</c> re-maps the domain object into a fresh persistence
    /// instance and conflicts with the one already tracked from that earlier read.
    /// </summary>
    Task DeleteByIdAsync(int Id);
}
