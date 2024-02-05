using Bw.Core.Reflection.Extensions;
using Bw.Core.Scheduling;
using MediatR;
using Newtonsoft.Json;
namespace Bw.Extensions.Cqrs;

public static class MediatRExtensions
{
    public static async Task SendScheduleObject(
        this IMediator mediator,
        ScheduleSerializedObject scheduleSerializedObject)
    {
        var type = scheduleSerializedObject.GetPayloadType();

        dynamic? req = JsonConvert.DeserializeObject(scheduleSerializedObject.Data, type!);

        if (req is null)
        {
            return;
        }

        await mediator.Send(req);
    }
}