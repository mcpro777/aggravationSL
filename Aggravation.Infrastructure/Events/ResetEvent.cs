using System;
using Microsoft.Practices.Prism.Events;

namespace Aggravation.Infrastructure.Events
{
    public class ResetData
    {
        public ResetData()
        {
        }
    }

    public class ResetEvent : CompositePresentationEvent<ResetData> { }
}
