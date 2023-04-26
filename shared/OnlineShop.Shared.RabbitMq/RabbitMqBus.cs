using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OnlineShop.Shared.Bus.Bus;
using OnlineShop.Shared.Bus.Commands;
using OnlineShop.Shared.Bus.Events;
using OnlineShop.Shared.Common.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OnlineShop.Shared.RabbitMq
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

        }

        public void Unsubscribe<TEvent, TEventHandler>()
            where TEvent : Event
            where TEventHandler : IEventHandler<TEvent>
        {
            throw new NotImplementedException();
        }
    }
}
