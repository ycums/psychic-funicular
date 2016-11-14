using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pg01.Models;
using Pg01.ViewModels;

namespace Pg01Tests.ViewModels
{
    [TestClass]
    public class MenuViewModelTests
    {
        [TestMethod]
        public void UpdateBasicTest()
        {
            var model = new Model
            {
                Menu = new Menu
                {
                    MenuItem = new List<MenuItem>
                    {
                        new MenuItem {X = -1, Y = -1},
                        new MenuItem {X = 0, Y = 0},
                        new MenuItem {X = -8, Y = 0},
                        new MenuItem {X = 0, Y = 9}
                    }
                }
            };
            var vm = new MenuViewModel(model);
            vm.Initialize();
            vm.ButtonsOrigin.X.Is(-8);
            vm.ButtonsOrigin.Y.Is(-9);
            vm.ButtonsContainerWidth.Is((8*2 + 1)*ConstValues.ButtonWidth);
            vm.ButtonsContainerHeight.Is((9*2 + 1)*ConstValues.ButtonHeight);
            vm.Buttons[2].X.Is(0);
            vm.Buttons[2].Y.Is(9*ConstValues.ButtonHeight);
        }
    }
}