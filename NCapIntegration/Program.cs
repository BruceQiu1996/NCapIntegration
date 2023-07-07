using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NCapIntegration.Attributes;
using NCapIntegration.EventBus;
using NCapIntegration.HostService;
using NCapIntegration.Interceptors;
using NCapIntegration.Persistence.MongoDB;
using NCapIntegration.Persistence.MSSql;
using NCapIntegration.Persistence.MSSql.Uow;
using NCapIntegration.Pipeline;
using NCapIntegration.Services;

namespace NCapIntegration
{
    internal class Program
    {
        /// <summary>
        /// sql server数据库连接字符串
        /// </summary>
        private static readonly string mssqlConStr = "server=127.0.0.1;uid=sa;pwd=123456;database=NCapIntegration;Encrypt=True;TrustServerCertificate=True;Connection Timeout=300;";
        
        static async Task Main(string[] args)
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
            builder.ConfigureContainer(new AutofacServiceProviderFactory(), builder =>
            {
                //注册mongodb
                builder.AddMongoDb(); 
                //注册写入日志channel
                builder.RegisterType<OperateLoggerChannel>().SingleInstance();
                //注册拦截器以及service
                builder.RegisterType<UowAsyncInterceptor>().AsSelf();
                builder.RegisterType<AsyncInterceptorAdaper<UowAsyncInterceptor>>().AsSelf();
                builder.RegisterType<OperateLogAsyncInterceptor>().AsSelf();
                builder.RegisterType<AsyncInterceptorAdaper<OperateLogAsyncInterceptor>>().AsSelf();
                builder.RegisterType<StudentService>().AsImplementedInterfaces()
                .EnableInterfaceInterceptors()
                .InterceptedBy(typeof(AsyncInterceptorAdaper<OperateLogAsyncInterceptor>), typeof(AsyncInterceptorAdaper<UowAsyncInterceptor>));
            });

            //注册数据库和工作单元
            builder.Services.AddScoped<IUnitOfWork, MssqlUnitOfWork<NCapIntegrationDbContext>>();
            builder.Services.AddDbContext<NCapIntegrationDbContext>(options =>
            {
                options.UseSqlServer(mssqlConStr, action =>
                {
                    action.MigrationsAssembly(typeof(NCapIntegrationDbContext).Assembly.FullName);
                });
            });


            //注册cap
            builder.Services.AddSingleton<IEventPublisher, CapPublisher>()
                .AddScoped<StudentCapSubscriber>()//面向抽象可以不同模块传入不同的泛型，使用泛型约束即可.
                .AddCap(action =>
            {
                //注册cap数据库
                var tableName = "cap_demo"; //sql server数据库scheme名
                action.UseSqlServer(config =>
                {
                    config.ConnectionString = mssqlConStr;
                    config.Schema = tableName;
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
                action.DefaultGroupName = "cap.demo.dev";
                action.FailedRetryInterval = 30;
                //前三次会在失败后立马重试，后面会等待四分钟后再次重试，重试次数配置为60，如果不配置，默认为50次。
                action.FailedRetryCount = 60;
                action.FailedThresholdCallback = (failed) =>
                {
                    //failed.MessageType 
                };
                //默认值：一天,成功消息的过期时间（秒）。
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