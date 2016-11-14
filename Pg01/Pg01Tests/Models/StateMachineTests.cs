#region

using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pg01.Models;
using Pg01.Views.Behaviors.Util;
using Pg01Tests.Properties;

#endregion

namespace Pg01Tests.Models
{
    [TestClass]
    public class StateMachineTests
    {
        public TestContext TestContext { get; set; }

        #region Test Exec

        [TestMethod]
        [TestCategory("Ctrl+Shift+S")]
        [Description("Ctrl+Shift+S として有効な順序でキーを操作した場合、DPGest はキー操作に介入しない")]
        public void ExecCtrlShiftSTest01()
        {
            var eia = new StateMachine();
            var config = ConfigUtil.Deserialize(Resources.TestConfig04);
            var menuItems = config.ApplicationGroups[0].Banks[0].Entries;
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

        [TestMethod]
        [TestCategory("Ctrl+Shift+S")]
        [Description("Ctrl+Shift+S として無効な順序でキーを操作した場合、DPGest の機能が優先される")]
        [SuppressMessage("ReSharper", "RedundantAssignment")]
        public void ExecCtrlShiftSTest02()
        {
            var eia = new StateMachine();
            var config = ConfigUtil.Deserialize(Resources.TestConfig04);
            var menuItems = config.ApplicationGroups[0].Banks[0].Entries;
            ExecResult r1 = null;
            r1 = eia.Exec(menuItems, Keys.S, NativeMethods.KeyboardUpDown.Down);
            r1 = eia.Exec(menuItems, Keys.S, NativeMethods.KeyboardUpDown.Up);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.LoadBank);
            r1.NextBank.Is("曲線");
            eia.ClearInternalStatuses();

            var menuGroup = config.ApplicationGroups[0].Banks[3];
            menuGroup.Name.Is(r1.NextBank);
            menuItems = menuGroup.Entries;

            r1 = eia.Exec(menuItems, Keys.S, NativeMethods.KeyboardUpDown.Down);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.None);

            r1 = eia.Exec(menuItems, Keys.LShiftKey, NativeMethods.KeyboardUpDown.Down);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = eia.Exec(menuItems, Keys.LControlKey, NativeMethods.KeyboardUpDown.Down);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = eia.Exec(menuItems, Keys.S, NativeMethods.KeyboardUpDown.Up);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.LoadBank);

            r1 = eia.Exec(menuItems, Keys.LControlKey, NativeMethods.KeyboardUpDown.Up);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = eia.Exec(menuItems, Keys.LShiftKey, NativeMethods.KeyboardUpDown.Up);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = eia.Exec(menuItems, Keys.S, NativeMethods.KeyboardUpDown.Down);
            r1 = eia.Exec(menuItems, Keys.S, NativeMethods.KeyboardUpDown.Up);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.LoadBank);
            r1.NextBank.IsNull();
            eia.ClearInternalStatuses();
        }

        [TestMethod]
        [Description("MenuItem.Type==Key の場合で、理想的なキーの入力順序の一例")]
        public void ExecMenuItemTypeKey01()
        {
            var eia = new StateMachine();
            var config = ConfigUtil.Deserialize(Resources.TestConfig04);
            var menuItems = config.ApplicationGroups[0].Banks[0].Entries;
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
            r1.Status.Is(ExecStatus.LoadBank);
            r1.NextBank.IsNull();
        }

        [TestMethod]
        [Description("MenuItem.Type==Key の場合で、複雑なキーの入力順序の一例")]
        public void ExecMenuItemTypeKey02()
        {
            var eia = new StateMachine();
            var config = ConfigUtil.Deserialize(Resources.TestConfig04);
            var menuItems = config.ApplicationGroups[0].Banks[0].Entries;
            var r1 = eia.Exec(menuItems, Keys.D4, NativeMethods.KeyboardUpDown.Down);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.None);

            r1 = eia.Exec(menuItems, Keys.LShiftKey, NativeMethods.KeyboardUpDown.Down);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = eia.Exec(menuItems, Keys.D4, NativeMethods.KeyboardUpDown.Up);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.LoadBank);
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
            var eia = new StateMachine();
            var config = ConfigUtil.Deserialize(Resources.TestConfig04);
            var menuItems = config.ApplicationGroups[0].Banks[0].Entries;
            var r1 = eia.Exec(menuItems, Keys.LShiftKey, NativeMethods.KeyboardUpDown.Down);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = eia.Exec(menuItems, Keys.D4, NativeMethods.KeyboardUpDown.Down);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.None);

            r1 = eia.Exec(menuItems, Keys.D4, NativeMethods.KeyboardUpDown.Up);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.LoadBank);
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
            var eia = new StateMachine();
            var config = ConfigUtil.Deserialize(Resources.TestConfig04);
            var menuItems = config.ApplicationGroups[0].Banks[0].Entries;
            var r1 = eia.Exec(menuItems, Keys.LWin, NativeMethods.KeyboardUpDown.Down);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = eia.Exec(menuItems, Keys.D4, NativeMethods.KeyboardUpDown.Down);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.None);

            r1 = eia.Exec(menuItems, Keys.D4, NativeMethods.KeyboardUpDown.Up);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.LoadBank);
            r1.NextBank.IsNull();
            eia.ClearInternalStatuses();

            r1 = eia.Exec(menuItems, Keys.LWin, NativeMethods.KeyboardUpDown.Up);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);
        }

        [TestMethod]
        [Description("モディファイアキーを長時間Downしているとそのあとの入力を受け付けなくなる不具合の修正 #52")]
        public void ExecMdifireKey01()
        {
            var eia = new StateMachine();
            var config = ConfigUtil.Deserialize(Resources.TestConfig04);
            var menuItems = config.ApplicationGroups[0].Banks[0].Entries;
            var r1 = eia.Exec(menuItems, Keys.LControlKey, NativeMethods.KeyboardUpDown.Down);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = eia.Exec(menuItems, Keys.LControlKey, NativeMethods.KeyboardUpDown.Down);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = eia.Exec(menuItems, Keys.LControlKey, NativeMethods.KeyboardUpDown.Down);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None); 

            // 長期間同じキーをDownしていると Down イベントは複数回発生するが、
            // Up イベントは1度しか発生しない
            r1 = eia.Exec(menuItems, Keys.LControlKey, NativeMethods.KeyboardUpDown.Up);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = eia.Exec(menuItems, Keys.S, NativeMethods.KeyboardUpDown.Down);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.None);

            r1 = eia.Exec(menuItems, Keys.S, NativeMethods.KeyboardUpDown.Up);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.LoadBank);
        }

        #endregion

        #region Test ExecCore

        public static object[] ToaruSource =
        {
            new object[]
            {
                "Key Down",
                new ActionItem {ActionType = ActionType.Key, ActionValue = "", NextBank = ""},
                NativeMethods.KeyboardUpDown.Down,
                new ExecResult(true, ExecStatus.None, "", ActionType.Key, "", NativeMethods.KeyboardUpDown.Down)
            },
            new object[]
            {
                "Send Down",
                new ActionItem {ActionType = ActionType.Send, ActionValue = "Val1", NextBank = "Bank1"},
                NativeMethods.KeyboardUpDown.Down,
                new ExecResult(true)
            },
            new object[]
            {
                "Send Up",
                new ActionItem {ActionType = ActionType.Send, ActionValue = "Val1", NextBank = "Bank1"},
                NativeMethods.KeyboardUpDown.Up,
                new ExecResult(true, ExecStatus.LoadBank, "Bank1", ActionType.Send, "Val1",
                    NativeMethods.KeyboardUpDown.Up)
            },
            new object[]
            {
                "Menu Down",
                new ActionItem {ActionType = ActionType.Menu, ActionValue = "Menu1", NextBank = ""},
                NativeMethods.KeyboardUpDown.Down,
                new ExecResult(true)
            },
            new object[]
            {
                "Menu Up",
                new ActionItem {ActionType = ActionType.Menu, ActionValue = "Menu1", NextBank = ""},
                NativeMethods.KeyboardUpDown.Up,
                new ExecResult(true, ExecStatus.LoadBank, "", ActionType.Menu, "Menu1", NativeMethods.KeyboardUpDown.Up)
            },
            new object[]
            {
                "None Down",
                new ActionItem {ActionType = ActionType.None, ActionValue = "Menu1", NextBank = ""},
                NativeMethods.KeyboardUpDown.Down,
                new ExecResult(true)
            },
            new object[]
            {
                "None Up",
                new ActionItem {ActionType = ActionType.None, ActionValue = "Menu1", NextBank = ""},
                NativeMethods.KeyboardUpDown.Up,
                new ExecResult(true, ExecStatus.LoadBank, "", ActionType.None, "Menu1", NativeMethods.KeyboardUpDown.Up)
            },
        };

        [TestMethod]
        [TestCaseSource(nameof(ToaruSource))]
        public void TestTestCaseSource()
        {
            var sm = new StateMachine();
            TestContext.Run((
                    string caseName,
                    ActionItem actionItem,
                    NativeMethods.KeyboardUpDown upDown,
                    ExecResult expected) =>
                {
                    var actual = sm.ExecCore(actionItem, upDown);
                    actual.ActionType.Is(expected.ActionType, caseName);
                    actual.UpDown.Is(expected.UpDown, caseName);
                    actual.Status.Is(expected.Status, caseName);
                });
        }

        #endregion
    }
}