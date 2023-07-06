namespace NCapIntegration.EventBus
{
    public interface IEventPublisher
    {
        public Task PublishAsync<T>(T eventObj, string? callbackName = null, CancellationToken cancellationToken = default) where T : class;
    }
}
