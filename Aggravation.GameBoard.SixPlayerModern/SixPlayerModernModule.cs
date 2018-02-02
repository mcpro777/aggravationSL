using System;
using Aggravation.Infrastructure.Models;
using Microsoft.Practices.Prism.Modularity;

using Microsoft.Practices.Prism.Regions;

namespace Aggravation.GameBoard.SixPlayerModern
{
    public class SixPlayerModernModule : ModuleBase
    {
        private readonly IRegionManager regionManager;

        public SixPlayerModernModule(IRegionManager regionManager)
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
