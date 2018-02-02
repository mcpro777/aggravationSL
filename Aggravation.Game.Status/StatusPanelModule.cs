using System;
using Aggravation.Infrastructure.Models;
using Microsoft.Practices.Prism.Modularity;

using Microsoft.Practices.Prism.Regions;

namespace Aggravation.Game.Status
{
    public class StatusPanelModule : ModuleBase
    {
        private readonly IRegionManager regionManager;

        public StatusPanelModule(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
        }

        public override void Initialize()
        {
            base.Initialize();

            regionManager.RegisterViewWithRegion("StatusRegion", typeof(Views.StatusPanel));
        }
    }
}
