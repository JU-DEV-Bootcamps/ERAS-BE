﻿using Eras.Application.Dtos;
using Eras.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
            int componentVariableId = 1; // This should come from relation with componentVariable

            // We should remove this field, because we have the text in componentVariableId and in the anser table... it doesn't make sense. talk to ramiro

            string Question = answer.Question.Body.GetValueOrDefault("es") ?? "No question found"; //  this is because we have language option (spanish or english)

            if (answer == null) throw new ArgumentNullException(nameof(answer));
            int Id = 0;
            DateTime CreatedDate = DateTime.Now;
            DateTime ModifiedDate = DateTime.Now;

            StringBuilder sbQuestions = new StringBuilder();
            foreach (var item in answer.AnswersList)
            {
                sbQuestions.Append(item);
            }
            string AnswerText = sbQuestions.ToString();
            int Position = answer.Position;
            int RiskLevel = (int) answer.Score;
            return new Answer( AnswerText, Question, Position, RiskLevel, Id, componentVariableId, CreatedDate, ModifiedDate);
        }

        public static ComponentVariable ToVariable(Answers answer, int pollId)
        {
            if (answer == null) throw new ArgumentNullException(nameof(answer));
            int id = 0;
            string name = answer.Question.Body.GetValueOrDefault("es") ?? "No question name found"; //  this is because we have language option (spanish or english)
            int position = answer.Position;
            int? parentId = null; // this is component id, later we should check this
            DateTime createdDate = DateTime.Now;
            DateTime modifiedDate = DateTime.Now;

            return new ComponentVariable(id, name, pollId, position, parentId, createdDate, modifiedDate);
        }

        public static Poll ToPoll (DataItem CLPol)
        {
            if (CLPol == null) throw new ArgumentNullException(nameof(CLPol));
            int Id = 0;
            string CosmicId = CLPol.inventoryId;
            DateTime CreatedDate = DateTime.Now;
            DateTime ModifiedDate = DateTime.Now;
            string PollName = CLPol.name;
            return new Poll(Id, CosmicId, CreatedDate, ModifiedDate, PollName);
        }

        public static PollDTO ToPollDTO(DataItem cosmicLattePoll)
        {
            if (cosmicLattePoll == null) throw new ArgumentNullException(nameof(cosmicLattePoll));
            return new PollDTO(0, cosmicLattePoll._id, DateTime.Now, DateTime.Now, cosmicLattePoll.name);
        }

    }
}
