using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Eras.Application.Events;
using Eras.Application.Services;

using MediatR;

namespace Eras.Application.Features.Evaluations.Handlers;

public class EvaluationCreatedEventHandler : INotificationHandler<EvaluationCreatedEvent>
{
   private readonly EvaluationStatusUpdater _updater;
   public EvaluationCreatedEventHandler(EvaluationStatusUpdater updater)
       => _updater = updater;

   public Task Handle(EvaluationCreatedEvent notification, CancellationToken ct)
       => _updater.UpdateStatusAsync(notification.EvaluationId);
}
