﻿using System;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Modularity;

namespace Aggravation.Infrastructure.Events
{
    public class ModuleChangedEvent : CompositePresentationEvent<ModuleInfo> { }
}
