using System;
using Microsoft.Practices.Prism.Events;

namespace Aggravation.Infrastructure.Events
{
    public class ShowInstructionsData
    {
        public ShowInstructionsData()
        {
        }
    }

    public class ShowInstructionsEvent : CompositePresentationEvent<ShowInstructionsData> { }
}
