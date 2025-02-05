using Eras.Application.Dtos;
using Eras.Domain.Common;
using Eras.Domain.Entities;
using System.Text;

namespace Eras.Application.Mappers
{
    public static class CosmicLatteMapper
    {
        public static Student ToStudent(CLResponseModelForPollDTO CLPoll)
        {
            ArgumentNullException.ThrowIfNull(nameof(CLPoll)); 
            int Id = 0;
            DateTime CreatedDate = DateTime.Now;
            DateTime ModifiedDate = DateTime.Now;
            string Email = CLPoll.Data.Answers.ElementAt(0).Value.AnswersList[0];
            string Name = CLPoll.Data.Answers.ElementAt(1).Value.AnswersList[0];
            string? Uuid = "null";
            return new Student() { Uuid = Uuid, Email = Email, Name = Name, StudentDetail = new StudentDetail() };
        }

        public static Answer ToAnswer(Answers answer)
        {
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
            if (answer == null) throw new ArgumentNullException(nameof(answer));
            
            int id = 0;
            string name = answer.Question.Body.GetValueOrDefault("es") ?? "No question name found"; //  this is because we have language option (spanish or english)
            int position = answer.Position;
            int? parentId = null; // this is component id, later we should check this
            DateTime createdDate = DateTime.Now;
            DateTime modifiedDate = DateTime.Now;

            return new Variable
            {
                Id = id,
                Name = name,
                //PollId = pollId,
                //Position = position,
                //ParentId = parentId,
                // CreatedDate = createdDate,
                // ModifiedDate = modifiedDate
            };
        }

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

        public static PollDTO ToPollDTO(DataItem cosmicLattePoll)
        {
            if (cosmicLattePoll == null) throw new ArgumentNullException(nameof(cosmicLattePoll));
            return new PollDTO(0, cosmicLattePoll._id, DateTime.Now, DateTime.Now, cosmicLattePoll.name);
        }

    }
}
