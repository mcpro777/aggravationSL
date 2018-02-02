using System;
using Aggravation.Infrastructure.Models;
using Microsoft.Practices.Prism.Modularity;

using Microsoft.Practices.Prism.Regions;

namespace Aggravation.Game.GameControls
{
    public class GameControlsModule : ModuleBase
    {
        private readonly IRegionManager regionManager;

        public GameControlsModule(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
        }

        public override void Initialize()
        {
            base.Initialize();

            regionManager.RegisterViewWithRegion("ControlsRegion", typeof(Views.GameControls));
        }
    }
}
