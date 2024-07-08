using System.Collections.Frozen;
using System.Net;
using Gml.Web.Api.Dto.Messages;
using Gml.Web.Api.Dto.Notifications;
using GmlCore.Interfaces;
using GmlCore.Interfaces.Notifications;

namespace Gml.Web.Api.Core.Handlers;

public class NotificationHandler : INotificationsHandler
{
    public static Task<IResult> GetNotifications(IGmlManager gmlManager)
    {
        FrozenSet<INotification> history = gmlManager.Notifications.History.TakeLast(50).ToFrozenSet();

        var result = Results.Ok(ResponseMessage.Create(new NotificationReadDto
            {
                Notifications = history,
                Amount = history.Count
            }, "Список уведомлений",
            HttpStatusCode.OK));

        return Task.FromResult(result);
    }
}
