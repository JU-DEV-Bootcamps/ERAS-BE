using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediatR;

namespace Eras.Application.Events;

public record EvaluationCreatedEvent(int EvaluationId) : INotification;
