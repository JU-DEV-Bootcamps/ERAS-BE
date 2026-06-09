using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Services;
using Eras.Application.Events;

using MediatR;

namespace Eras.Application.Features.Evaluations.Handlers;

public class AnswerSubmittedEventHandler : INotificationHandler<AnswerSubmittedEvent>
{
   private readonly EvaluationStatusUpdater _updater;
   public AnswerSubmittedEventHandler(EvaluationStatusUpdater updater)
       => _updater = updater;

   public Task Handle(AnswerSubmittedEvent notification, CancellationToken ct)
       => _updater.UpdateStatusAsync(notification.EvaluationId);
}
