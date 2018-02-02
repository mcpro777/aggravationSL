using System;
using Aggravation.Infrastructure.Models;
using Microsoft.Practices.Prism.Modularity;

using Microsoft.Practices.Prism.Regions;

namespace Aggravation.Game.PlayerPanels
{
    public class PlayerPanelsModule : ModuleBase
    {
        private readonly IRegionManager regionManager;

        public PlayerPanelsModule(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
        }

        public override void Initialize()
        {
            base.Initialize();

            regionManager.RegisterViewWithRegion("GamePanelsRegion", typeof(Views.PlayerPanels));
        }
    }
}
