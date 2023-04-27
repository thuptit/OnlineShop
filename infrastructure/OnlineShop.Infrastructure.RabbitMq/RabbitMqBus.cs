using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OnlineShop.Infrastructure.Bus.Bus;
using OnlineShop.Infrastructure.Bus.Commands;
using OnlineShop.Infrastructure.Bus.Events;
using OnlineShop.Shared.Common.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OnlineShop.Infrastructure.RabbitMq
{
    public sealed class RabbitMqBus : IEventBus
    {
        private readonly ILogger<RabbitMqBus> _logger;
        private readonly IMediator _mediator;
        private readonly Dictionary<string, List<Type>> _handlers;
        private readonly List<Type> _evenTypes;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IOptions<RabbitMqOptions> _rabbitMqOptions;
        public RabbitMqBus(IMediator mediator, IServiceScopeFactory serviceScopeFactory, ILogger<RabbitMqBus> logger, IOptions<RabbitMqOptions> rabbitMqOptions)
        {
            _mediator = mediator;
            _serviceScopeFactory = serviceScopeFactory;
            _handlers = new Dictionary<string, List<Type>>();
            _evenTypes = new List<Type>();
            _logger = logger;
            _rabbitMqOptions = rabbitMqOptions;
        }
        public Task SendCommmand<T>(T message) where T : Command
        {
            return _mediator.Send(message);
        }
        public void Publish<TEvent>(TEvent @event) where TEvent : Event
        {
            var factory = new ConnectionFactory()
            {
                HostName = _rabbitMqOptions.Value.HostName,
                UserName = _rabbitMqOptions.Value.Username,
                Password = _rabbitMqOptions.Value.Password,
            };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            var eventName = @event.GetType().Name;
            channel.QueueDeclare(eventName,false,false,false,null);
            var message = JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish("",eventName,null,body);
        }

        public void Subscribe<TEvent, TEventHandler>()
            where TEvent : Event
            where TEventHandler : IEventHandler<TEvent>
        {
            var eventName = typeof(TEvent).Name;
            var handlerType = typeof(TEventHandler);

            if (!_evenTypes.Contains(typeof(TEvent)))
            {
                _evenTypes.Add(typeof(TEvent));
            }

            if (!_handlers.ContainsKey(eventName)){
                _handlers.Add(eventName, new List<Type>());
            }

            if (_handlers[eventName].Any(t => t == handlerType))
            {
                _logger.LogError($"Handler Type {handlerType.Name} already is registered for '{eventName}'", nameof(handlerType));
                throw new ArgumentException($"Handler Type {handlerType.Name} already is registered for '{eventName}'", nameof(handlerType));
            }
            _handlers[eventName].Add(handlerType);

            StartBasicConsumer<TEvent>();
        }
        private void StartBasicConsumer<T>()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _rabbitMqOptions.Value.HostName,
                DispatchConsumersAsync = true,
                UserName = _rabbitMqOptions.Value.Username,
                Password = _rabbitMqOptions.Value.Password
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            var eventName = typeof(T).Name;
            channel.QueueDeclare(eventName, false, false, false, null);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += Consumer_Received;

            channel.BasicConsume(eventName, false, consumer);
        }

        public async Task Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            var eventName = e.RoutingKey;
            var message = Encoding.UTF8.GetString(e.Body.Span);
            try
            {
                await ProcessEvent(eventName, message).ConfigureAwait(false);
                ((AsyncDefaultBasicConsumer)sender).Model.BasicAck(e.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something went wrong with Consumer_Received!");
            }
        }
        private async Task ProcessEvent(string eventName, string message)
        {
            if(_handlers.ContainsKey(eventName))
            {
                using(var scope = _serviceScopeFactory.CreateScope())
                {
                    var subcriptions = _handlers[eventName];
                    foreach(var subcription in subcriptions)
                    {
                        var handler = scope.ServiceProvider.GetService(subcription);

                        if (handler == null) return;

                        var eventType = _evenTypes.SingleOrDefault(s => s.Name == eventName);
                        var @event = JsonSerializer.Deserialize(message, eventType);
                        var concreteType = typeof(IEventHandler<>).MakeGenericType(eventType);
                        await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { @event });
                    }
                }
            }
        }
        public void Unsubscribe<TEvent, TEventHandler>()
            where TEvent : Event
            where TEventHandler : IEventHandler<TEvent>
        {
            throw new NotImplementedException();
        }
    }
}
