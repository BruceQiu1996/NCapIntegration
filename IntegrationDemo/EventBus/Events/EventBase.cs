namespace IntegrationDemo.EventBus.Events
{
    public class EventBase<TBody>
    {
        public string Id { get; set; }
        public DateTime TriggerTime { get; set; } = DateTime.UtcNow;
        public TBody Data { get; set; }
    }
}
