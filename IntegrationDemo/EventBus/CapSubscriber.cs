using DotNetCore.CAP;

namespace IntegrationDemo.EventBus
{
    internal class CapSubscriber : ICapSubscribe
    {
        public CapSubscriber()
        {
            
        }

        [CapSubscribe(nameof(OrderCreatedEvent))]
        public async Task ProcessOrderCreatedEvent(OrderCreatedEvent eventDto)
        {
            
        }
    }
}
