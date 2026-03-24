namespace Eras.Domain.Entities.Referrals;

public sealed class Remission : BaseEntity<RemissionId>
{
    private readonly List<RemissionStudent> _students = new();
    private readonly List<RemissionComment> _comments = new();
    private readonly List<Intervention> _interventions = new();

    private Remission()
    {
    }

    public required DateTime CreatedAtUtc { get; init; }
    public required UserId CreatedBy { get; init; }
    public ServiceId ServiceId { get; private set; }
    public ProfessionalId? AssignedProfessionalId { get; private set; }
    public Diagnosis Diagnosis { get; private set; }
    public Objective Objective { get; private set; }
    public string? Notes { get; private set; }
    public RemissionStatus Status { get; private set; } = RemissionStatus.Created;
    public DateTime? ReferredAtUtc { get; private set; }
    public DateTime? ConcludedAtUtc { get; private set; }
    public string? RejectionReason { get; private set; }
    public InterventionPlan? Plan { get; private set; }

    public IReadOnlyCollection<RemissionStudent> Students => _students.AsReadOnly();
    public IReadOnlyCollection<RemissionComment> Comments => _comments.AsReadOnly();
    public IReadOnlyCollection<Intervention> Interventions => _interventions.AsReadOnly();

    public static Remission Create(
        RemissionId id,
        UserId createdBy,
        ServiceId serviceId,
        Diagnosis diagnosis,
        Objective objective,
        string? notes = null,
        ProfessionalId? assignedProfessionalId = null,
        DateTime? createdAtUtc = null)
    {
        return new Remission
        {
            Id = id,
            CreatedAtUtc = createdAtUtc ?? DateTime.UtcNow,
            CreatedBy = createdBy,
            ServiceId = serviceId,
            AssignedProfessionalId = assignedProfessionalId,
            Diagnosis = diagnosis,
            Objective = objective,
            Notes = DomainNormalization.ToNullableTrimmed(notes),
            Status = RemissionStatus.Created
        };
    }

    public void UpdateDetails(
        ServiceId serviceId,
        Diagnosis diagnosis,
        Objective objective,
        string? notes)
    {
        ServiceId = serviceId;
        Diagnosis = diagnosis;
        Objective = objective;
        Notes = DomainNormalization.ToNullableTrimmed(notes);
    }

    public void AssignProfessional(ProfessionalId? professionalId)
    {
        AssignedProfessionalId = professionalId;
    }

    public void AddStudent(StudentId studentId)
    {
        if (_students.Any(x => x.StudentId == studentId))
        {
            return;
        }

        _students.Add(RemissionStudent.Create(studentId));
    }

    public void RemoveStudent(StudentId studentId)
    {
        RemissionStudent? existing = _students.FirstOrDefault(x => x.StudentId == studentId);

        if (existing is null)
        {
            return;
        }

        _students.Remove(existing);
    }

    public void DefinePlan(
        InterventionPlanId planId,
        StudentId primaryStudentId,
        Diagnosis diagnosis,
        Objective objective,
        string? notes = null)
    {
        Plan = InterventionPlan.Create(
            id: planId,
            studentId: primaryStudentId,
            diagnosis: diagnosis,
            objective: objective,
            notes: notes);
    }

    public void ClearPlan()
    {
        Plan = null;
    }

    public void Refer(DateTime? referredAtUtc = null)
    {
        Status = RemissionStatus.Referred;
        ReferredAtUtc = referredAtUtc ?? DateTime.UtcNow;
        RejectionReason = null;
    }

    public void MarkWaitingForProfessional()
    {
        Status = RemissionStatus.WaitingForProfessional;
    }

    public void StartProgress()
    {
        Status = RemissionStatus.InProgress;
    }

    public void Reject(string reason)
    {
        Status = RemissionStatus.Created;
        RejectionReason = DomainNormalization.ToTrimmedOrEmpty(reason);
        ReferredAtUtc = null;
    }

    public void Conclude(DateTime? concludedAtUtc = null)
    {
        Status = RemissionStatus.Concluded;
        ConcludedAtUtc = concludedAtUtc ?? DateTime.UtcNow;
    }

    public void AddComment(UserId authorId, string text, DateTime? createdAtUtc = null)
    {
        _comments.Add(RemissionComment.Create(
            authorId,
            text,
            createdAtUtc ?? DateTime.UtcNow));
    }

    public IndividualIntervention ScheduleIndividualIntervention(
        InterventionId interventionId,
        StudentId studentId,
        DateTime dateUtc,
        InterventionActivity activity,
        ProfessionalId professionalId,
        string? comments = null)
    {
        var intervention = IndividualIntervention.Create(
            interventionId,
            studentId,
            dateUtc,
            activity,
            professionalId,
            comments);

        _interventions.Add(intervention);
        return intervention;
    }

    public GroupIntervention ScheduleGroupIntervention(
        InterventionId interventionId,
        DateTime dateUtc,
        InterventionActivity activity,
        InterventionArea area,
        ProfessionalId professionalId,
        IEnumerable<StudentId> participantStudentIds,
        string? comments = null)
    {
        var intervention = GroupIntervention.Create(
            interventionId,
            dateUtc,
            activity,
            area,
            professionalId,
            participantStudentIds,
            comments);

        _interventions.Add(intervention);
        return intervention;
    }

    public Intervention? FindIntervention(InterventionId interventionId)
        => _interventions.FirstOrDefault(x => x.Id == interventionId);

    public void AddAttachmentToIntervention(
        InterventionId interventionId,
        InterventionAttachment attachment)
    {
        Intervention? intervention = FindIntervention(interventionId);

        if (intervention is null)
        {
            return;
        }

        intervention.AddAttachment(attachment);
    }

    public void CompleteIntervention(InterventionId interventionId, string? comments = null)
    {
        Intervention? intervention = FindIntervention(interventionId);

        if (intervention is null)
        {
            return;
        }

        intervention.Complete(comments);
    }

    public void CancelIntervention(InterventionId interventionId, string? reason = null)
    {
        Intervention? intervention = FindIntervention(interventionId);

        if (intervention is null)
        {
            return;
        }

        intervention.Cancel(reason);
    }

    public bool HasStudent(StudentId studentId)
        => _students.Any(x => x.StudentId == studentId);

    public bool CanBeDeleted()
        => Status == RemissionStatus.Created;
}
