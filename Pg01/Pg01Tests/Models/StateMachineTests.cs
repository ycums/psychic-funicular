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
            var machine = new StateMachine();
            var config = ConfigUtil.Deserialize(Resources.TestConfig04);
            var menuItems = config.ApplicationGroups[0].Banks[0].Entries;
            var r1 = machine.Exec(menuItems, Keys.LShiftKey, NativeMethods.KeyboardUpDown.Down);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = machine.Exec(menuItems, Keys.LControlKey, NativeMethods.KeyboardUpDown.Down);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = machine.Exec(menuItems, Keys.S, NativeMethods.KeyboardUpDown.Down);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = machine.Exec(menuItems, Keys.S, NativeMethods.KeyboardUpDown.Up);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = machine.Exec(menuItems, Keys.LControlKey, NativeMethods.KeyboardUpDown.Up);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = machine.Exec(menuItems, Keys.LShiftKey, NativeMethods.KeyboardUpDown.Up);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);
        }

        [TestMethod]
        [TestCategory("Ctrl+Shift+S")]
        [Description("Ctrl+Shift+S として無効な順序でキーを操作した場合、DPGest の機能が優先される")]
        [SuppressMessage("ReSharper", "RedundantAssignment")]
        public void ExecCtrlShiftSTest02()
        {
            var machine = new StateMachine();
            var config = ConfigUtil.Deserialize(Resources.TestConfig04);
            var menuItems = config.ApplicationGroups[0].Banks[0].Entries;
            ExecResult r1 = null;
            r1 = machine.Exec(menuItems, Keys.S, NativeMethods.KeyboardUpDown.Down);
            r1 = machine.Exec(menuItems, Keys.S, NativeMethods.KeyboardUpDown.Up);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.LoadBank);
            r1.NextBank.Is("曲線");
            machine.ClearInternalStatuses();

            var menuGroup = config.ApplicationGroups[0].Banks[3];
            menuGroup.Name.Is(r1.NextBank);
            menuItems = menuGroup.Entries;

            r1 = machine.Exec(menuItems, Keys.S, NativeMethods.KeyboardUpDown.Down);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.None);

            r1 = machine.Exec(menuItems, Keys.LShiftKey, NativeMethods.KeyboardUpDown.Down);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = machine.Exec(menuItems, Keys.LControlKey, NativeMethods.KeyboardUpDown.Down);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = machine.Exec(menuItems, Keys.S, NativeMethods.KeyboardUpDown.Up);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.LoadBank);

            r1 = machine.Exec(menuItems, Keys.LControlKey, NativeMethods.KeyboardUpDown.Up);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = machine.Exec(menuItems, Keys.LShiftKey, NativeMethods.KeyboardUpDown.Up);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = machine.Exec(menuItems, Keys.S, NativeMethods.KeyboardUpDown.Down);
            r1 = machine.Exec(menuItems, Keys.S, NativeMethods.KeyboardUpDown.Up);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.LoadBank);
            r1.NextBank.IsNull();
            machine.ClearInternalStatuses();
        }

        [TestMethod]
        [Description("MenuItem.Type==Key の場合で、理想的なキーの入力順序の例")]
        public void ExecMenuItemTypeKey01()
        {
            var machine = new StateMachine();
            var config = ConfigUtil.Deserialize(Resources.TestConfig04);
            var menuItems = config.ApplicationGroups[0].Banks[0].Entries;
            var r1 = machine.Exec(menuItems, Keys.D4, NativeMethods.KeyboardUpDown.Down);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.None);

            r1 = machine.Exec(menuItems, Keys.LShiftKey, NativeMethods.KeyboardUpDown.Down);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = machine.Exec(menuItems, Keys.LShiftKey, NativeMethods.KeyboardUpDown.Up);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = machine.Exec(menuItems, Keys.D4, NativeMethods.KeyboardUpDown.Up);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.LoadBank);
            r1.NextBank.IsNull();
        }

        [TestMethod]
        [Description("MenuItem.Type==Key の場合で、複雑なキーの入力順序の例")]
        public void ExecMenuItemTypeKey02()
        {
            var machine = new StateMachine();
            var config = ConfigUtil.Deserialize(Resources.TestConfig04);
            var menuItems = config.ApplicationGroups[0].Banks[0].Entries;
            var r1 = machine.Exec(menuItems, Keys.D4, NativeMethods.KeyboardUpDown.Down);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.None);

            r1 = machine.Exec(menuItems, Keys.LShiftKey, NativeMethods.KeyboardUpDown.Down);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = machine.Exec(menuItems, Keys.D4, NativeMethods.KeyboardUpDown.Up);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.LoadBank);
            r1.NextBank.IsNull();
            machine.ClearInternalStatuses();

            r1 = machine.Exec(menuItems, Keys.LShiftKey, NativeMethods.KeyboardUpDown.Up);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);
        }

        [TestMethod]
        [Description("shift(down) -> 4(down) -> 4(up) -> shift(up) とすると、4(up) が解釈されない不具合の修正")]
        public void ExecMenuItemTypeKey03()
        {
            var machine = new StateMachine();
            var config = ConfigUtil.Deserialize(Resources.TestConfig04);
            var menuItems = config.ApplicationGroups[0].Banks[0].Entries;
            var r1 = machine.Exec(menuItems, Keys.LShiftKey, NativeMethods.KeyboardUpDown.Down);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = machine.Exec(menuItems, Keys.D4, NativeMethods.KeyboardUpDown.Down);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.None);

            r1 = machine.Exec(menuItems, Keys.D4, NativeMethods.KeyboardUpDown.Up);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.LoadBank);
            r1.NextBank.IsNull();
            machine.ClearInternalStatuses();

            r1 = machine.Exec(menuItems, Keys.LShiftKey, NativeMethods.KeyboardUpDown.Up);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);
        }

        [TestMethod]
        [Description("Win+4 で Win+(スペース) に置き換えられることの確認")]
        public void ExecMenuItemTypeKey04()
        {
            var machine = new StateMachine();
            var config = ConfigUtil.Deserialize(Resources.TestConfig04);
            var menuItems = config.ApplicationGroups[0].Banks[0].Entries;
            var r1 = machine.Exec(menuItems, Keys.LWin, NativeMethods.KeyboardUpDown.Down);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = machine.Exec(menuItems, Keys.D4, NativeMethods.KeyboardUpDown.Down);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.None);

            r1 = machine.Exec(menuItems, Keys.D4, NativeMethods.KeyboardUpDown.Up);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.LoadBank);
            r1.NextBank.IsNull();
            machine.ClearInternalStatuses();

            r1 = machine.Exec(menuItems, Keys.LWin, NativeMethods.KeyboardUpDown.Up);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);
        }

        [TestMethod]
        [Description("モディファイアキーを長時間Downしているとそのあとの入力を受け付けなくなる不具合の修正 #52")]
        public void ExecMdifireKey01()
        {
            var machine = new StateMachine();
            var config = ConfigUtil.Deserialize(Resources.TestConfig04);
            var menuItems = config.ApplicationGroups[0].Banks[0].Entries;
            var r1 = machine.Exec(menuItems, Keys.LControlKey, NativeMethods.KeyboardUpDown.Down);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = machine.Exec(menuItems, Keys.LControlKey, NativeMethods.KeyboardUpDown.Down);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = machine.Exec(menuItems, Keys.LControlKey, NativeMethods.KeyboardUpDown.Down);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            // 長期間同じキーをDownしていると Down イベントは複数回発生するが、
            // Up イベントは1度しか発生しない
            r1 = machine.Exec(menuItems, Keys.LControlKey, NativeMethods.KeyboardUpDown.Up);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = machine.Exec(menuItems, Keys.S, NativeMethods.KeyboardUpDown.Down);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.None);

            r1 = machine.Exec(menuItems, Keys.S, NativeMethods.KeyboardUpDown.Up);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.LoadBank);
        }

        [TestMethod]
        [Description("リセットキーに対する動作の検証")]
        public void ExecResetMenu()
        {
            var machine = new StateMachine();
            var config = ConfigUtil.Deserialize(Resources.TestConfig07);
            var bank = config.ApplicationGroups[0].Banks[4];
            bank.Name.Is("Bank2");

            var entries = bank.Entries;
            entries.Count.Is(1);

            config.Basic.ResetKey.Is("NumPad5");


            //
            // Menu 非表示中
            //
            // ReSharper disable RedundantArgumentDefaultValue
            var r1 = machine.Exec(entries, Keys.NumPad5, NativeMethods.KeyboardUpDown.Down,
                config.Basic.ResetKey,
                false);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.None);

            r1 = machine.Exec(entries, Keys.NumPad5, NativeMethods.KeyboardUpDown.Up,
                config.Basic.ResetKey,
                false);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.LoadBank);
            // ReSharper restore RedundantArgumentDefaultValue

            //
            // Menu 表示中
            //
            r1 = machine.Exec(entries, Keys.NumPad5, NativeMethods.KeyboardUpDown.Down,
                config.Basic.ResetKey,
                true);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.None);

            r1 = machine.Exec(entries, Keys.NumPad5, NativeMethods.KeyboardUpDown.Up,
                config.Basic.ResetKey,
                true);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.CloseMenu);
        }

        [TestMethod]
        [Description("ActionItem を空にすると落ちる #61")]
        public void ExecNullActionItem()
        {
            var machine = new StateMachine();
            var config = ConfigUtil.Deserialize(Resources.TestConfig08);
            var bank = config.ApplicationGroups[0].Banks[0];
            bank.Name.Is("");

            var entries = bank.Entries;
            entries.Count.Is(3);
            entries[0].Trigger.Is("NumPad9");
            entries[0].ActionItem.IsNull();
            machine.Exec(entries, Keys.NumPad9, NativeMethods.KeyboardUpDown.Down);
            machine.Exec(entries, Keys.NumPad9, NativeMethods.KeyboardUpDown.Up);
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
                new ActionItem {ActionType = ActionType.None, ActionValue = "Menu1", NextBank = "Bank1"},
                NativeMethods.KeyboardUpDown.Down,
                new ExecResult(true)
            },
            new object[]
            {
                "None Up",
                new ActionItem {ActionType = ActionType.None, ActionValue = "Menu1", NextBank = "Bank1"},
                NativeMethods.KeyboardUpDown.Up,
                new ExecResult(true, ExecStatus.LoadBank, "Bank1", ActionType.None, "Menu1",
                    NativeMethods.KeyboardUpDown.Up)
            },
            new object[]
            {
                "Null ActionItem Up",
                null,
                NativeMethods.KeyboardUpDown.Up,
                new ExecResult(true)
            }
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
                    actual.ShouldCancel.Is(expected.ShouldCancel, caseName);
                    actual.NextBank.Is(expected.NextBank, caseName);
                });
        }

        #endregion
    }
}