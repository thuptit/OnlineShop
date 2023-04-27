using OnlineShop.Infrastructure.Bus.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShop.Infrastructure.Bus.Commands
{
    public abstract class Command : Message
    {
        public DateTime TimeStamp { get; protected set; }
        public ValidationResult ValidationResult { get; set; }

        protected Command()
        {
            TimeStamp = DateTime.Now;
        }
    }
}
