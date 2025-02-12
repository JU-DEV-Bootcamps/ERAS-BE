using Eras.Application.Dtos;
using Eras.Application.DTOs;
using Eras.Domain.Common;
using Eras.Domain.Entities;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Eras.Application.Mappers;

    public static class CosmicLatteMapper
    {
        public static Student ToStudent(CLResponseModelForPollDTO CLPoll)
        {
            ArgumentNullException.ThrowIfNull(nameof(CLPoll)); 

            DateTime CreatedDate = DateTime.Now;
            DateTime ModifiedDate = DateTime.Now;
            return new Student
            {
                Uuid = "null",
                Name = CLPoll.Data.Answers.ElementAt(1).Value.AnswersList[0],
                Email = CLPoll.Data.Answers.ElementAt(0).Value.AnswersList[0],
                StudentDetail = new StudentDetail // It is wrong, only for demo
                {
                    EnrolledCourses = 0, 
                    GradedCourses = 0,
                    TimeDeliveryRate = 0,
                    AvgScore = 0,
                    CoursesUnderAvg = 0,
                    PureScoreDiff = 0,
                    StandardScoreDiff = 0,
                    LastAccessDays = 0,
                    Audit = new AuditInfo { CreatedBy = "", ModifiedBy = "", CreatedAt = DateTime.Now, ModifiedAt = DateTime.Now } // It is wrong, only for demo
                },
                Audit = new AuditInfo { CreatedBy = "", ModifiedBy = "", CreatedAt = DateTime.Now, ModifiedAt = DateTime.Now } // It is wrong, only for demo
            };
        }

        public static Answer ToAnswer(Answers answer)
        {

            /*
                public string AnswerText { get; set; } = string.Empty;
                public int RiskLevel { get; set; }
                public int PollInstanceId { get; set; }
                public PollInstance PollInstance { get; set; } = default!;
                public AuditInfo Audit { get; set; } = default!;
             */



            // todo pending finish
            int VariableId = 1; // This should come from relation with Variable

            // We should remove this field, because we have the text in VariableId and in the anser table... it doesn't make sense. talk to ramiro

            string question = answer.Question.Body.GetValueOrDefault("es") ?? "No question found"; //  this is because we have language option (spanish or english)

            if (answer == null) throw new ArgumentNullException(nameof(answer));
            int id = 0;
            DateTime createdDate = DateTime.Now;
            DateTime modifiedDate = DateTime.Now;

            StringBuilder sbQuestions = new StringBuilder();
            foreach (var item in answer.AnswersList)
            {
                sbQuestions.Append(item);
            }
            string answerText = sbQuestions.ToString();
            int position = answer.Position;
            int riskLevel = (int) answer.Score;
            return new Answer { 
                AnswerText = answerText, 
                //Question = question, 
                //Position = position, 
                RiskLevel = riskLevel, 
                Id = id, 
                //VariableId, 
                Audit = new AuditInfo() { 
                    CreatedAt = createdDate,
                    ModifiedAt = modifiedDate 
                }
            };
        }


        public static Variable ToVariable(Answers answer, int pollId)
        {
            /*
                    [JsonPropertyName("answer")]
        public string[] AnswersList { get; set; }

        [JsonPropertyName("question")]
        public Question Question { get; set; }

        [JsonPropertyName("position")]
        public int Position { get; set; }

        [JsonPropertyName("score")]
        public double Score { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("customSettings")]
        public List<string> CustomSettings { get; set; } = new List<string>();
            */



            if (answer == null) throw new ArgumentNullException(nameof(answer));

            int id = 0;
            string name = answer.Question.Body.GetValueOrDefault("es") ?? "No question name found"; //this is because we have language option(spanish or english)
            int position = answer.Position;
            int? parentId = null; // this is component id, later we should check this
            DateTime createdDate = DateTime.Now;
            DateTime modifiedDate = DateTime.Now;

            return new Variable
            {
/*
public string Name { get; set; } = string.Empty;
public int ComponentId { get; set; }
public Component Component { get; set; } = default!;
public ICollection<Poll> Polls { get; set; } = [];
public ICollection<Cohort> Cohorts { get; set; } = [];
public AuditInfo Audit { get; set; } = default!;
*/
/*
                Id = id,
                Name = name,
                PollId = pollId,
                Position = position,
                ParentId = parentId,
                */
                // CreatedDate = createdDate,
                //cModifiedDate = modifiedDate
            };
        }
        public static Answer DtoToAnswer(AnswerDTO answerDto)
        {
            string question = answerDto.Question;
            int id = 0;
            DateTime createdDate = DateTime.Now;
            DateTime modifiedDate = DateTime.Now;
            string answerText = answerDto.Answer;
            int position = answerDto.Position;
            int riskLevel = (int) answerDto.Score;
            return new Answer
            {
                AnswerText = answerText,
                RiskLevel = riskLevel,
                Id = id,
                Audit = new AuditInfo()
                {
                    CreatedAt = createdDate,
                    ModifiedAt = modifiedDate
                }
            };
        }

    // public static ComponentVariable ToVariable(Answers answer, int pollId)
    // {
    //     if (answer == null) throw new ArgumentNullException(nameof(answer));

    //     int id = 0;
    //     string name = answer.Question.Body.GetValueOrDefault("es") ?? "No question name found"; //  this is because we have language option (spanish or english)
    //     int position = answer.Position;
    //     int? parentId = null; // this is component id, later we should check this
    //     DateTime createdDate = DateTime.Now;
    //     DateTime modifiedDate = DateTime.Now;

    //     return new ComponentVariable
    //     {
    //         Id = id,
    //         Name = name,
    //         PollId = pollId,
    //         Position = position,
    //         ParentId = parentId,
    //         // CreatedDate = createdDate,
    //         // ModifiedDate = modifiedDate
    //     };
    // }

    public static Poll ToPoll (DataItem CLPol)
        {
            if (CLPol == null) throw new ArgumentNullException(nameof(CLPol));
            
            int id = 0;
            string cosmicId = CLPol.inventoryId;
            DateTime createdDate = DateTime.Now;
            DateTime modifiedDate = DateTime.Now;
            string pollName = CLPol.name;
            return new Poll
            {
                Id = id,
                //CosmicId = cosmicId,
                // CreatedDate = CreatedDate,
                // ModifiedDate = ModifiedDate,
                Audit = new AuditInfo{ 
                    CreatedAt = createdDate,
                    ModifiedAt = modifiedDate
                },
                Name = pollName
            };
        }

        public static Poll DtoToPoll (PollDTO pollDTO)
        {
            int id = pollDTO.Id;
            string cosmicLatteId = pollDTO.CosmicLatteId;
            string pollName = pollDTO.PollName;
            DateTime createdDate = pollDTO.CreatedDate;
            DateTime modifiedDate = pollDTO.ModifiedDate;

            return new Poll
            {
                Id = id,
                Audit = new AuditInfo { CreatedAt = createdDate, ModifiedAt = modifiedDate },
                Name = pollName,
                Uuid = cosmicLatteId
            };
        }

        public static PollDTO ToPollDTO(DataItem cosmicLattePoll)
        {
            if (cosmicLattePoll == null) throw new ArgumentNullException(nameof(cosmicLattePoll));
            return new PollDTO(0, cosmicLattePoll._id, DateTime.Now, DateTime.Now, cosmicLattePoll.name);
        }

    }
