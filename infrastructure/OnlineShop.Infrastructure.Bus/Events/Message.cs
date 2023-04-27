using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop.Infrastructure.Bus.Events
{
    public abstract class Message : IRequest<bool>
    {
        public string MessageType { get; protected set; }
        public Guid AggregateId { get; protected set; }

        protected Message()
        {
            MessageType = GetType().Name;
        }
    }
}
