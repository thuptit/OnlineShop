using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop.Infrastructure.Bus.Bus
{
    public interface IEventHandler<in TEvent> : IEventHandler
    {
        Task Handle(TEvent @event);
    }

    public interface IEventHandler
    {
    }
}
