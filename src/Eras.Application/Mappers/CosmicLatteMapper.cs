using Eras.Application.Dtos;
using Eras.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Eras.Application.Mappers
{
    public class CosmicLatteMapper
    {
        public Student CLtoStudent(CLResponseModelForPollDTO CLPoll)
        {
            int Id = 0;
            DateTime CreatedDate = DateTime.Now;
            DateTime ModifiedDate = DateTime.Now;
            string Email = CLPoll.Data.Answers.ElementAt(0).Value.AnswersList[0];
            string Name = CLPoll.Data.Answers.ElementAt(1).Value.AnswersList[0];
            string? Uuid = "null";
            return new Student(Id, CreatedDate, ModifiedDate, Name, Email, Uuid);
        }

        public Answer CLtoAnswer(Answers answer)
        {
            int Id = 0;
            DateTime CreatedDate = DateTime.Now;
            DateTime ModifiedDate = DateTime.Now;
            string AnswerText = answer.AnswersList.FirstOrDefault(); // this has a list of answers, for questions that has multiple
            string Question = answer.Question.Body.GetValueOrDefault("es"); //  this is because we have language option (spanish or english)
            int Position = answer.Position;
            double Score = answer.Score;
            int componentVariableId = 1 ; // This should come from relation with componentVariable
            Console.WriteLine(AnswerText);
            return new Answer( AnswerText, Question, Position, Score, Id, componentVariableId, CreatedDate, ModifiedDate);
        }

        public ComponentVariable CLtoVariable(Answers answer, int pollId)
        {
            int id = 0;
            string name = answer.Question.Body.GetValueOrDefault("es"); // question name?

            int position = answer.Position;
            int? parentId = null; // this is component id, later we should check this
            DateTime createdDate = DateTime.Now;
            DateTime modifiedDate = DateTime.Now;

            return new ComponentVariable(id, name, pollId, position, parentId, createdDate, modifiedDate);
        }

        public Poll CLtoPoll (DataItem CLPol)
        {
            int Id = 0;
            string CosmicId = CLPol.inventoryId;
            DateTime CreatedDate = DateTime.Now;
            DateTime ModifiedDate = DateTime.Now;
            string PollName = CLPol.name;
            return new Poll(Id, CosmicId, CreatedDate, ModifiedDate, PollName);
        }

        public PollDTO CLDataItemToPollDTO(DataItem cosmicLattePoll)
        {
            return new PollDTO(0, cosmicLattePoll._id, DateTime.Now, DateTime.Now, cosmicLattePoll.name);
        }

    }
}
