namespace Gantry.NET;

public enum CommandType
{
    Ping = 0,
    GetTopics = 1,
    CreateTopic = 2,
    DeleteTopic = 3,
    PutMessage = 4,
    GetMessage = 5,
    SubscribeTopic = 6,
    UnsubscribeTopic = 8
}