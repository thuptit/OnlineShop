using OnlineShop.Infrastructure.Bus.Commands;
using OnlineShop.Infrastructure.Bus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop.Infrastructure.Bus.Bus
{
    public interface IEventBus
    {
        Task SendCommmand<T>(T message) where T: Command;
        void Publish<TEvent>(TEvent @event)
            where TEvent : Event;

        void Subscribe<TEvent, TEventHandler>()
            where TEvent : Event
            where TEventHandler : IEventHandler<TEvent>;

        void Unsubscribe<TEvent, TEventHandler>()
            where TEvent : Event
            where TEventHandler : IEventHandler<TEvent>;
    }
}
