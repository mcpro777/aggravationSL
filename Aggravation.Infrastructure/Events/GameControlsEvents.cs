using System;
using Microsoft.Practices.Prism.Events;

namespace Aggravation.Infrastructure.Events
{
    public class EnableShowInstructionsData
    {
        public EnableShowInstructionsData()
        {
        }
    }
    public class EnableShowInstructionsEvent : CompositePresentationEvent<EnableShowInstructionsData> { }

    public class DisableShowInstructionsData
    {
        public DisableShowInstructionsData()
        {
        }
    }
    public class DisableShowInstructionsEvent : CompositePresentationEvent<DisableShowInstructionsData> { }

    public class EnableResetData
    {
        public EnableResetData()
        {
        }
    }
    public class EnableResetEvent : CompositePresentationEvent<EnableResetData> { }

    public class DisableResetData
    {
        public DisableResetData()
        {
        }
    }
    public class DisableResetEvent : CompositePresentationEvent<DisableResetData> { }
}

