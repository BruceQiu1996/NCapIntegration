using DotNetCore.CAP;

namespace IntegrationDemo.EventBus
{
    public class CapPublisher : IEventPublisher
    {
        private readonly ICapPublisher _capPublisher;
        public CapPublisher(ICapPublisher capPublisher)
        {
            _capPublisher = capPublisher;
        }

        public async Task PublishAsync<T>(T eventObj, string? callbackName = null, CancellationToken cancellationToken = default) where T : class
        {
            await _capPublisher.PublishAsync(typeof(T).FullName, eventObj, callbackName, cancellationToken);
        }
    }
}
