using MassTransit;
using System.Reflection;
using Types;
using ConsumerBT.Consumer;
using ConsumerAT.Consumer;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddMassTransit(busConfig => {
    void configure(IRabbitMqBusFactoryConfigurator factoryCfg)
    {
        factoryCfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
        factoryCfg.ReceiveEndpoint("MyMessage", x =>
        {
            x.PrefetchCount = 10;
            x.ConcurrentMessageLimit = 1;
            x.Batch<MyMessage>(b =>
            {
                b.MessageLimit = 10;
                b.ConcurrencyLimit = 1;
                b.TimeLimit = TimeSpan.FromSeconds(20);
              
            });
         
            x.Consumer(() => {
                var logger = builder.Services.BuildServiceProvider().GetService<ILogger<ConsumerB>>();
                return new ConsumerB(logger);
            });
            x.Consumer(() => {
                var logger = builder.Services.BuildServiceProvider().GetService<ILogger<ConsumerA>>();
                return new ConsumerA(logger);
            });
        });
    }
    busConfig.UsingRabbitMq((ctx, cfg) => configure(cfg));

});

var app = builder.Build();




app.Run();
