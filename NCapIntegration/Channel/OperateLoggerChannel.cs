using NCapIntegration.Entities;
using NCapIntegration.Persistence.MongoDB;
using System.Threading.Channels;

namespace NCapIntegration.Pipeline
{
    /// <summary>
    /// 操作日志记录通道
    /// </summary>
    public class OperateLoggerChannel
    {
        private readonly ChannelWriter<OperateLog> _writeChannel;
        private readonly ChannelReader<OperateLog> _readChannel;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public OperateLoggerChannel(CollectionProvider collectionProvider)
        {
            var channelOptions = new BoundedChannelOptions(1000)
            {
                FullMode = BoundedChannelFullMode.Wait
            };

            var channel = Channel.CreateBounded<OperateLog>(channelOptions);
            _writeChannel = channel.Writer;
            _readChannel = channel.Reader;
            ReadOperateLogService readOperateLogService = new ReadOperateLogService(_readChannel, collectionProvider);
            var _ = Task.Run(async () => await readOperateLogService.StartAsync(_cancellationTokenSource.Token));
        }

        ~OperateLoggerChannel()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        public async Task WriteLogAsync(OperateLog OperateLog)
        {
            await _writeChannel.WriteAsync(OperateLog);
        }

        public class ReadOperateLogService
        {
            private readonly ChannelReader<OperateLog> _readChannel;
            private readonly CollectionProvider _collectionProvider;

            public ReadOperateLogService(ChannelReader<OperateLog> readChannel, CollectionProvider collectionProvider)
            {
                _readChannel = readChannel;
                _collectionProvider = collectionProvider;
            }

            public async Task StartAsync(CancellationToken cancellationToken)
            {
                while (await _readChannel.WaitToReadAsync(cancellationToken))
                {
                    var entities = new List<OperateLog>();
                    //多消费下，需要考虑并发问题
                    foreach (var index in Enumerable.Range(0, _readChannel.Count))
                    {
                        entities.Add(await _readChannel.ReadAsync());
                    }
                    var collection = await _collectionProvider.GetCollection<OperateLog>();
                    await collection.InsertManyAsync(entities);

                    if (cancellationToken.IsCancellationRequested) break;
                }
            }

            public Task StopAsync(CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }
        }
    }
}
