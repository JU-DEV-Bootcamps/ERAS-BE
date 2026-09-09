using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.DTOs.AssessmentManagement;
using Eras.Domain.Entities.AssessmentManagement;

namespace Eras.Application.Mappers.AssessmentManagement;

public sealed class UpdateInterventionMapper : IMapper<UpdateInterventionDto, Intervention>
{
    public Intervention Map(UpdateInterventionDto Source)
    {
        // This creates a new Intervention — the handler will need to set Id separately
        return new Intervention
        {
            DateUtc = Source.DateUtc,
            Activity = Source.Activity,
            Area = Source.Area,
            NumberOfParticipants = Source.NumberOfParticipants,
            Professional = Source.Professional,
            StudentIds = Source.StudentIds,
            Attendance = Source.Attendance,
            Mode = Source.Mode,
            Status = Source.Status,
            Remarks = Source.Remarks,
            Comments = Source.Comments,
            RiskLevel = Source.RiskLevel,
            RiskLevelName = Source.RiskLevelName,
            EndRiskLevelName = Source.EndRiskLevelName,
        };
    }
}
