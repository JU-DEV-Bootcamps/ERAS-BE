namespace Eras.Domain.Entities.Referrals;

public sealed record InterventionArea
{
    public required string Value { get; init; }

    public static InterventionArea Create(string value)
    {
        return new InterventionArea
        {
            Value = DomainNormalization.ToTrimmedOrEmpty(value)
        };
    }

    public override string ToString() => Value;
}


public sealed class RemissionComment
{
    private RemissionComment()
    {
    }

    public required UserId AuthorId { get; init; }
    public required string Text { get; init; }
    public required DateTime CreatedAtUtc { get; init; }

    internal static RemissionComment Create(
        UserId authorId,
        string text,
        DateTime createdAtUtc)
    {
        return new RemissionComment
        {
            AuthorId = authorId,
            Text = DomainNormalization.ToTrimmedOrEmpty(text),
            CreatedAtUtc = createdAtUtc
        };
    }
}

public sealed class InterventionAttachment : BaseEntity<AttachmentId>
{
    private InterventionAttachment()
    {
    }

    public required string FileName { get; init; }
    public required string ContentType { get; init; }
    public required string StoragePath { get; init; }
    public required UserId UploadedBy { get; init; }
    public required DateTime UploadedAtUtc { get; init; }

    public static InterventionAttachment Create(
        AttachmentId id,
        string fileName,
        string contentType,
        string storagePath,
        UserId uploadedBy,
        DateTime uploadedAtUtc)
    {
        return new InterventionAttachment
        {
            Id = id,
            FileName = DomainNormalization.ToTrimmedOrEmpty(fileName),
            ContentType = DomainNormalization.ToTrimmedOrEmpty(contentType),
            StoragePath = DomainNormalization.ToTrimmedOrEmpty(storagePath),
            UploadedBy = uploadedBy,
            UploadedAtUtc = uploadedAtUtc
        };
    }
}

public sealed class GroupInterventionParticipant : BaseEntity<StudentId>
{
    private GroupInterventionParticipant()
    {
    }

    public required StudentId StudentId { get; init; }

    internal static GroupInterventionParticipant Create(StudentId studentId)
    {
        return new GroupInterventionParticipant
        {
            Id = studentId,
            StudentId = studentId
        };
    }
}

public sealed class GroupIntervention : Intervention
{
    private readonly List<GroupInterventionParticipant> _participants = new();

    private GroupIntervention()
    {
    }

    public required InterventionArea Area { get; init; }
    public IReadOnlyCollection<GroupInterventionParticipant> Participants => _participants.AsReadOnly();
    public int ParticipantCount => _participants.Count;

    internal static GroupIntervention Create(
        InterventionId id,
        DateTime dateUtc,
        InterventionActivity activity,
        InterventionArea area,
        ProfessionalId professionalId,
        IEnumerable<StudentId> participantStudentIds,
        string? comments = null)
    {
        var intervention = new GroupIntervention
        {
            Id = id,
            DateUtc = dateUtc,
            Activity = activity,
            Area = area,
            ProfessionalId = professionalId,
            Comments = DomainNormalization.ToNullableTrimmed(comments),
            Status = InterventionStatus.Scheduled
        };

        foreach (StudentId studentId in participantStudentIds.Distinct())
        {
            intervention._participants.Add(GroupInterventionParticipant.Create(studentId));
        }

        return intervention;
    }
}


public sealed class IndividualIntervention : Intervention
{
    private IndividualIntervention()
    {
    }

    public required StudentId StudentId { get; init; }

    internal static IndividualIntervention Create(
        InterventionId id,
        StudentId studentId,
        DateTime dateUtc,
        InterventionActivity activity,
        ProfessionalId professionalId,
        string? comments = null)
    {
        return new IndividualIntervention
        {
            Id = id,
            StudentId = studentId,
            DateUtc = dateUtc,
            Activity = activity,
            ProfessionalId = professionalId,
            Comments = DomainNormalization.ToNullableTrimmed(comments),
            Status = InterventionStatus.Scheduled
        };
    }
}

public abstract class Intervention : BaseEntity<InterventionId>
{
    private readonly List<InterventionAttachment> _attachments = new();

    public required DateTime DateUtc { get; init; }
    public required InterventionActivity Activity { get; init; }
    public required ProfessionalId ProfessionalId { get; init; }
    public string? Comments { get; protected set; }
    public InterventionStatus Status { get; protected set; } = InterventionStatus.Scheduled;

    public IReadOnlyCollection<InterventionAttachment> Attachments => _attachments.AsReadOnly();

    internal void AddAttachment(InterventionAttachment attachment)
    {
        _attachments.Add(attachment);
    }

    internal void Complete(string? comments = null)
    {
        Status = InterventionStatus.Completed;

        if (!string.IsNullOrWhiteSpace(comments))
        {
            Comments = comments.Trim();
        }
    }

    internal void Cancel(string? reason = null)
    {
        Status = InterventionStatus.Cancelled;

        if (!string.IsNullOrWhiteSpace(reason))
        {
            Comments = reason.Trim();
        }
    }
}


public sealed record RemissionId(Guid Value)
{
    public static RemissionId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}

public sealed record StudentId(Guid Value)
{
    public static StudentId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}

public sealed record ProfessionalId(Guid Value)
{
    public static ProfessionalId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}

public sealed record UserId(Guid Value)
{
    public static UserId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}

public sealed record ServiceId(Guid Value)
{
    public static ServiceId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}

public sealed record InterventionId(Guid Value)
{
    public static InterventionId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}

public sealed record InterventionPlanId(Guid Value)
{
    public static InterventionPlanId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}

public sealed record AttachmentId(Guid Value)
{
    public static AttachmentId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}