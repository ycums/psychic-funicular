#region

using System.Collections.Generic;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pg01.Models;
using Pg01.ViewModels;

#endregion

namespace Pg01Tests.ViewModels
{
    [TestClass]
    public class MenuViewModelTests
    {
        [TestMethod]
        [Description("メニューの表示位置がなんだかずれている気がする #60")]
        public void MenuPositionTest()
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

            var pos = new Point(800, 400);
            var expectedWidth = (8*2 + 1)*ConstValues.ButtonWidth;
            var expectedHeight = (9*2 + 1)*ConstValues.ButtonHeight;
            var expectedX = pos.X - expectedWidth/2;
            var expectedY = pos.Y - expectedHeight/2;

            var vm = new MenuViewModel(model);
            vm.UpdateMenu(model);
            vm.IsMenuVisibleChanged(true, pos);
            vm.ButtonsOrigin.X.Is(-8);
            vm.ButtonsOrigin.Y.Is(-9);

            vm.ButtonsContainerWidth.Is(expectedWidth);
            vm.ButtonsContainerHeight.Is(expectedHeight);
            vm.Buttons[2].X.Is(0);
            vm.Buttons[2].Y.Is(9*ConstValues.ButtonHeight);
            vm.X.Is(expectedX);
            vm.Y.Is(expectedY);
        }
    }
}