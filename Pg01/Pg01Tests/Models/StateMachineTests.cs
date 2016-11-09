using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pg01.Behaviors.Util;
using Pg01.Models;
using Pg01.Models.Util;
using Pg01Tests.Properties;

namespace Pg01Tests.Models
{
    [TestClass()]
    public class StateMachineTests
    {
        [TestMethod, TestCategory("Ctrl+Shift+S")]
        [Description("Ctrl+Shift+S として有効な順序でキーを操作した場合、DPGest はキー操作に介入しない")]
        public void ExecCtrlShiftSTest01()
        {
            var eia = new StateMachine(new DummySendKey());
            var applicationGroups = ConfigUtil.Deserialize(Resources.TestConfig04);
            var menuItems = applicationGroups.ApplicationGroups[0].Banks[0].Entries;
            var r1 = eia.Exec(menuItems, Keys.LShiftKey, NativeMethods.KeyboardUpDown.Down);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = eia.Exec(menuItems, Keys.LControlKey, NativeMethods.KeyboardUpDown.Down);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = eia.Exec(menuItems, Keys.S, NativeMethods.KeyboardUpDown.Down);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = eia.Exec(menuItems, Keys.S, NativeMethods.KeyboardUpDown.Up);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = eia.Exec(menuItems, Keys.LControlKey, NativeMethods.KeyboardUpDown.Up);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = eia.Exec(menuItems, Keys.LShiftKey, NativeMethods.KeyboardUpDown.Up);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);
        }

        [TestMethod, TestCategory("Ctrl+Shift+S")]
        [Description("Ctrl+Shift+S として無効な順序でキーを操作した場合、DPGest の機能が優先される")]
        public void ExecCtrlShiftSTest02()
        {
            var eia = new StateMachine(new DummySendKey());
            var config = ConfigUtil.Deserialize(Resources.TestConfig04);
            var menuItems = config.ApplicationGroups[0].Banks[0].Entries;
            var r1 = eia.Exec(menuItems, Keys.S, NativeMethods.KeyboardUpDown.Down);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.LoadGroup);
            r1.NextBank.Is("曲線");
            eia.ClearInternalStatuses();

            var menuGroup = config.ApplicationGroups[0].Banks[3];
            menuGroup.Name.Is(r1.NextBank);
            menuItems = menuGroup.Entries;

            r1 = eia.Exec(menuItems, Keys.LShiftKey, NativeMethods.KeyboardUpDown.Down);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = eia.Exec(menuItems, Keys.LControlKey, NativeMethods.KeyboardUpDown.Down);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = eia.Exec(menuItems, Keys.S, NativeMethods.KeyboardUpDown.Up);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = eia.Exec(menuItems, Keys.LControlKey, NativeMethods.KeyboardUpDown.Up);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = eia.Exec(menuItems, Keys.LShiftKey, NativeMethods.KeyboardUpDown.Up);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = eia.Exec(menuItems, Keys.D, NativeMethods.KeyboardUpDown.Down);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.LoadGroup);
            r1.NextBank.IsNull();
            eia.ClearInternalStatuses();
        }

        [TestMethod]
        [Description("MenuItem.Type==Key の場合で、理想的なキーの入力順序の一例")]
        public void ExecMenuItemTypeKey01()
        {
            var eia = new StateMachine(new DummySendKey());
            var config = ConfigUtil.Deserialize(Resources.TestConfig04);
            var applicationGroup = config.ApplicationGroups[0];
            var menuGroup = applicationGroup.Banks[0];
            var menuItems = menuGroup.Entries;
            var r1 = eia.Exec(menuItems, Keys.D4, NativeMethods.KeyboardUpDown.Down);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.None);

            r1 = eia.Exec(menuItems, Keys.LShiftKey, NativeMethods.KeyboardUpDown.Down);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = eia.Exec(menuItems, Keys.LShiftKey, NativeMethods.KeyboardUpDown.Up);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = eia.Exec(menuItems, Keys.D4, NativeMethods.KeyboardUpDown.Up);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.LoadGroup);
            r1.NextBank.IsNull();
        }

        [TestMethod]
        [Description("MenuItem.Type==Key の場合で、複雑なキーの入力順序の一例")]
        public void ExecMenuItemTypeKey02()
        {
            var eia = new StateMachine(new DummySendKey());
            var applicationGroups = ConfigUtil.Deserialize(Resources.TestConfig04);
            var menuItems = applicationGroups.ApplicationGroups[0].Banks[0].Entries;
            var r1 = eia.Exec(menuItems, Keys.D4, NativeMethods.KeyboardUpDown.Down);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.None);

            r1 = eia.Exec(menuItems, Keys.LShiftKey, NativeMethods.KeyboardUpDown.Down);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = eia.Exec(menuItems, Keys.D4, NativeMethods.KeyboardUpDown.Up);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.LoadGroup);
            r1.NextBank.IsNull();
            eia.ClearInternalStatuses();

            r1 = eia.Exec(menuItems, Keys.LShiftKey, NativeMethods.KeyboardUpDown.Up);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);
        }

        [TestMethod]
        [Description("shift(down) -> 4(down) -> 4(up) -> shift(up) とすると、4(up) が解釈されない不具合の修正")]
        public void ExecMenuItemTypeKey03()
        {
            var eia = new StateMachine(new DummySendKey());
            var applicationGroups = ConfigUtil.Deserialize(Resources.TestConfig04);
            var menuItems = applicationGroups.ApplicationGroups[0].Banks[0].Entries;
            var r1 = eia.Exec(menuItems, Keys.LShiftKey, NativeMethods.KeyboardUpDown.Down);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = eia.Exec(menuItems, Keys.D4, NativeMethods.KeyboardUpDown.Down);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.None);

            r1 = eia.Exec(menuItems, Keys.D4, NativeMethods.KeyboardUpDown.Up);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.LoadGroup);
            r1.NextBank.IsNull();
            eia.ClearInternalStatuses();

            r1 = eia.Exec(menuItems, Keys.LShiftKey, NativeMethods.KeyboardUpDown.Up);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

        }

        [TestMethod]
        [Description("Win+4 で Win+(スペース) に置き換えられることの確認")]
        public void ExecMenuItemTypeKey04()
        {
            var eia = new StateMachine(new DummySendKey());
            var applicationGroups = ConfigUtil.Deserialize(Resources.TestConfig04);
            var menuItems = applicationGroups.ApplicationGroups[0].Banks[0].Entries;
            var r1 = eia.Exec(menuItems, Keys.LWin, NativeMethods.KeyboardUpDown.Down);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = eia.Exec(menuItems, Keys.D4, NativeMethods.KeyboardUpDown.Down);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.None);

            r1 = eia.Exec(menuItems, Keys.D4, NativeMethods.KeyboardUpDown.Up);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.LoadGroup);
            r1.NextBank.IsNull();
            eia.ClearInternalStatuses();

            r1 = eia.Exec(menuItems, Keys.LWin, NativeMethods.KeyboardUpDown.Up);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

        }
    }

    public class DummySendKey : ISendKeyCode
    {
        public void SendKey(string data, NativeMethods.KeyboardUpDown kud)
        {
        }

        public void SendWait(string data)
        {
        }
    }
}