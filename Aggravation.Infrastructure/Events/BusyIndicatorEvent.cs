using System;
using Microsoft.Practices.Prism.Events;

namespace Aggravation.Infrastructure.Events
{
    public class BusyIndicatorData
    {
        public Boolean IsBusy { get; private set; }
        public String Message { get; private set; }

        public BusyIndicatorData(bool isBusy)
            : this(isBusy, String.Empty)
        {
        }

        public BusyIndicatorData(Boolean isBusy, String message)
        {
            this.IsBusy = isBusy;
            this.Message = message;
        }
    }

    public class BusyIndicatorEvent : CompositePresentationEvent<BusyIndicatorData> { }
}
