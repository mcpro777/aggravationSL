using System;
using Microsoft.Practices.Prism.Events;

namespace Aggravation.Infrastructure.Events
{
    public class GameStatusData
    {
        public String Message { get; set; }

        public GameStatusData(String message)
        {
            this.Message = message;
        }
    }

    public class GameStatusEvent : CompositePresentationEvent<GameStatusData> { }

}
