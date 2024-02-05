using Bw.Core.Messaging;

namespace Bw.Messaging.Event;

public record IntegrationEvent : Message, IIntegrationEvent;
