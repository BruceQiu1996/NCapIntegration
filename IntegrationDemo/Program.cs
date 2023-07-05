using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using IntegrationDemo.Attributes;
using IntegrationDemo.EventBus;
using IntegrationDemo.HostService;
using IntegrationDemo.Interceptors;
using IntegrationDemo.Persistence;
using IntegrationDemo.Persistence.Uow;
using IntegrationDemo.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IntegrationDemo
{
    internal class Program
    {
        /// <summary>
        /// sql server数据库连接字符串
        /// </summary>
        private static readonly string mssqlConStr = "server=127.0.0.1;uid=sa;pwd=123456;database=IntegrationDemo;Encrypt=True;TrustServerCertificate=True;Connection Timeout=300;";
        
        static async Task Main(string[] args)
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
            builder.ConfigureContainer<ContainerBuilder>(new AutofacServiceProviderFactory(), builder =>
            {
                //注册拦截器以及service
                builder.RegisterType<UowAsyncInterceptor>().InstancePerLifetimeScope();
                builder.RegisterType<AsyncInterceptorAdaper<UowAsyncInterceptor>>().InstancePerLifetimeScope();
                builder.RegisterType<StudentService>().SingleInstance().InterceptedBy(typeof(AsyncInterceptorAdaper<UowAsyncInterceptor>))
                .EnableClassInterceptors();
            });

            //注册数据库和工作单元
            builder.Services.AddScoped<IUnitOfWork, MssqlUnitOfWork<DemoDbContext>>();
            builder.Services.AddDbContext<DemoDbContext>(options =>
            {
                options.UseSqlServer(mssqlConStr, action =>
                {
                    action.MigrationsAssembly(typeof(DemoDbContext).Assembly.FullName);
                });
            });

            //注册cap
            builder.Services.AddSingleton<IEventPublisher, CapPublisher>().AddCap(action =>
            {
                //注册cap数据库
                var tableNamePrefix = "cap_demo";
                action.UseSqlServer(config =>
                {
                    config.ConnectionString = mssqlConStr;
                    config.Schema = tableNamePrefix;
                });

                //注册cap的消息队列中间件
                action.UseRabbitMQ(option =>
                {
                    option.HostName = "127.0.0.1";
                    option.VirtualHost = "/";
                    option.Port = 5672;
                    option.UserName = "Bruce";
                    option.Password = "12345678";
                    option.ConnectionFactoryOptions = (facotry) =>
                    {
                        facotry.ClientProvidedName = "";
                    };
                });
                action.Version = "v1.0.0.0";
                //默认值：cap.queue.{程序集名称},在 RabbitMQ 中映射到 Queue Names。
                action.DefaultGroupName = "cap.demo.env";
                //默认值：60 秒,重试 & 间隔
                //在默认情况下，重试将在发送和消费消息失败的 4分钟后 开始，这是为了避免设置消息状态延迟导致可能出现的问题。
                //发送和消费消息的过程中失败会立即重试 3 次，在 3 次以后将进入重试轮询，此时 FailedRetryInterval 配置才会生效。
                action.FailedRetryInterval = 60;
                //默认值：50,重试的最大次数。当达到此设置值时，将不会再继续重试，通过改变此参数来设置重试的最大次数。
                action.FailedRetryCount = 50;
                //默认值：NULL,重试阈值的失败回调。当重试达到 FailedRetryCount 设置的值的时候，将调用此 Action 回调
                //，你可以通过指定此回调来接收失败达到最大的通知，以做出人工介入。例如发送邮件或者短信。
                action.FailedThresholdCallback = (failed) =>
                {
                    //todo
                };
                //默认值：24*3600 秒（1天后),成功消息的过期时间（秒）。
                //当消息发送或者消费成功时候，在时间达到 SucceedMessageExpiredAfter 秒时候将会从 Persistent 中删除，你可以通过指定此值来设置过期的时间。
                action.SucceedMessageExpiredAfter = 24 * 3600;
                //默认值：1,消费者线程并行处理消息的线程数，当这个值大于1时，将不能保证消息执行的顺序。
                action.ConsumerThreadCount = 1;
                //Dashboard
                action.UseDashboard(x =>
                {
                    x.PathMatch = $"/demo/cap";
                    x.UseAuth = false;
                });
            });

            builder.Services.AddHostedService<MockService>();
            IHost host = builder.Build();
            await host.RunAsync();
        }
    }
}