namespace Bw.Core.Messaging.Persistence;

public class StoreMessage
{
    public Guid Id { get; private set; }
    public string DataType { get; private set; }
    public string Data { get; private set; }
    public DateTime Created { get; private set; }
    public int RetryCount { get; private set; }
    public MessageStatus MessageStatus { get; private set; }
    public MessageDeliveryType DeliveryType { get; private set; }


    public StoreMessage(Guid id, string dataType, string data, MessageDeliveryType deliveryType)
    {
        Id = id;
        DataType = dataType;
        Data = data;
        DeliveryType = deliveryType;
        Created = DateTime.Now;
        MessageStatus = MessageStatus.Stored;
        RetryCount = 0;
    }

    public void ChangeState(MessageStatus messageStatus)
    {
        MessageStatus = messageStatus;
    }

    public void IncreaseRetry()
    {
        RetryCount++;
    }


}