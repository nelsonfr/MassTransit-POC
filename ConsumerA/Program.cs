using MassTransit;
using System.Reflection;
using Types;
using GreenPipes;
using ConsumerAT.Consumer;
using ConsumerAT.Processor;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddScoped<IMessageProcessor, MessageProcessor>();
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
            var rate = 3;
            x.UseFilter(new ThrottlingMiddleware<MyMessage>(rate));          
            x.Consumer(() =>
            {
                var logger = builder.Services.BuildServiceProvider().GetService<ILogger<ConsumerA>>();
                var processor = builder.Services.BuildServiceProvider().GetService<IMessageProcessor>();
                return new ConsumerA(logger, processor);
            });
           

        });
    }
    busConfig.UsingRabbitMq((ctx, cfg) => configure(cfg));

});
var app = builder.Build();
app.Run();

