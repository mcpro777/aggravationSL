using System;
using Aggravation.Infrastructure.Models;
using Microsoft.Practices.Prism.Modularity;

using Microsoft.Practices.Prism.Regions;

namespace Aggravation.GameBoard.SixPlayerTraditional
{
    public class SixPlayerTraditionalModule : ModuleBase
    {
        private readonly IRegionManager regionManager;

        public SixPlayerTraditionalModule(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
        }

        public override void Initialize()
        {
            base.Initialize();

            regionManager.RegisterViewWithRegion("GameBoardRegion", typeof(Views.GameBoard));
        }
    }
}
