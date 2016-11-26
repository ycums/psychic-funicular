#region

using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pg01.Models;
using Pg01Tests.Properties;
using static Pg01.Views.Behaviors.Util.NativeMethods;

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
            var r1 = machine.Exec(menuItems, Keys.LShiftKey,
                KeyboardUpDown.Down);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = machine.Exec(menuItems, Keys.LControlKey,
                KeyboardUpDown.Down);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = machine.Exec(menuItems, Keys.S,
                KeyboardUpDown.Down);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = machine.Exec(menuItems, Keys.S,
                KeyboardUpDown.Up);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = machine.Exec(menuItems, Keys.LControlKey,
                KeyboardUpDown.Up);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = machine.Exec(menuItems, Keys.LShiftKey,
                KeyboardUpDown.Up);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);
        }

        [TestMethod]
        [TestCategory("Ctrl+S")]
        [Description("Ctrl+S として有効な順序でキーを操作した場合、DPGest はキー操作に介入しない")]
        public void ExecCtrlSTest01()
        {
            var machine = new StateMachine();
            var config = ConfigUtil.Deserialize(Resources.TestConfig04);
            var menuItems = config.ApplicationGroups[0].Banks[0].Entries;

            var r1 = machine.Exec(menuItems, Keys.LControlKey,
                KeyboardUpDown.Down);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = machine.Exec(menuItems, Keys.S,
                KeyboardUpDown.Down);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = machine.Exec(menuItems, Keys.S,
                KeyboardUpDown.Up);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = machine.Exec(menuItems, Keys.LControlKey,
                KeyboardUpDown.Up);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);
        }

        [TestMethod]
        [TestCategory("Ctrl+S")]
        [Description("Ctrl+S 用のブロックがすり抜けてしまう不具合の修正")]
        public void ExecCtrlSTest02()
        {
            var machine = new StateMachine();
            var config = ConfigUtil.Deserialize(Resources.TestConfig11);
            var entries = config.ApplicationGroups[0].Banks[0].Entries;
            entries[0].ActionItem.ActionType.Is(ActionType.None);
            entries[0].ActionItem.ActionValue.IsNull();

            var r1 = machine.Exec(entries, Keys.LControlKey,
                KeyboardUpDown.Down);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = machine.Exec(entries, Keys.S,
                KeyboardUpDown.Down);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = machine.Exec(entries, Keys.S,
                KeyboardUpDown.Up);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = machine.Exec(entries, Keys.LControlKey,
                KeyboardUpDown.Up);
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
            r1 = machine.Exec(menuItems, Keys.S,
                KeyboardUpDown.Down);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.LoadBank);
            r1.NextBank.Is("曲線");
            r1 = machine.Exec(menuItems, Keys.S,
                KeyboardUpDown.Up);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.None);

            machine.ClearInternalStatuses();

            var menuGroup = config.ApplicationGroups[0].Banks[3];
            menuGroup.Name.Is("曲線");
            menuItems = menuGroup.Entries;

            r1 = machine.Exec(menuItems, Keys.S,
                KeyboardUpDown.Down);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.LoadBank);

            r1 = machine.Exec(menuItems, Keys.LShiftKey,
                KeyboardUpDown.Down);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = machine.Exec(menuItems, Keys.LControlKey,
                KeyboardUpDown.Down);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = machine.Exec(menuItems, Keys.S,
                KeyboardUpDown.Up);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.None);

            r1 = machine.Exec(menuItems, Keys.LControlKey,
                KeyboardUpDown.Up);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = machine.Exec(menuItems, Keys.LShiftKey,
                KeyboardUpDown.Up);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = machine.Exec(menuItems, Keys.S,
                KeyboardUpDown.Down);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.LoadBank);
            r1.NextBank.IsNull();
            r1 = machine.Exec(menuItems, Keys.S,
                KeyboardUpDown.Up);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.None);
            machine.ClearInternalStatuses();
        }

        [TestMethod]
        [Description("MenuItem.Type==Key の場合で、理想的なキーの入力順序の例")]
        public void ExecMenuItemTypeKey01()
        {
            var machine = new StateMachine();
            var config = ConfigUtil.Deserialize(Resources.TestConfig04);
            var menuItems = config.ApplicationGroups[0].Banks[0].Entries;
            var r1 = machine.Exec(menuItems, Keys.D4,
                KeyboardUpDown.Down);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.LoadBank);
            r1.NextBank.IsNull();

            r1 = machine.Exec(menuItems, Keys.LShiftKey,
                KeyboardUpDown.Down);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = machine.Exec(menuItems, Keys.LShiftKey,
                KeyboardUpDown.Up);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = machine.Exec(menuItems, Keys.D4,
                KeyboardUpDown.Up);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.None);
        }

        [TestMethod]
        [Description("MenuItem.Type==Key の場合で、複雑なキーの入力順序の例")]
        public void ExecMenuItemTypeKey02()
        {
            var machine = new StateMachine();
            var config = ConfigUtil.Deserialize(Resources.TestConfig04);
            var menuItems = config.ApplicationGroups[0].Banks[0].Entries;
            var r1 = machine.Exec(menuItems, Keys.D4,
                KeyboardUpDown.Down);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.LoadBank);
            r1.NextBank.IsNull();

            r1 = machine.Exec(menuItems, Keys.LShiftKey,
                KeyboardUpDown.Down);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = machine.Exec(menuItems, Keys.D4,
                KeyboardUpDown.Up);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.None);
            machine.ClearInternalStatuses();

            r1 = machine.Exec(menuItems, Keys.LShiftKey,
                KeyboardUpDown.Up);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);
        }

        [TestMethod]
        [Description(
             "shift(down) -> 4(down) -> 4(up) -> shift(up) とすると、4(up) が解釈されない不具合の修正"
         )]
        public void ExecMenuItemTypeKey03()
        {
            var machine = new StateMachine();
            var config = ConfigUtil.Deserialize(Resources.TestConfig04);
            var menuItems = config.ApplicationGroups[0].Banks[0].Entries;
            var r1 = machine.Exec(menuItems, Keys.LShiftKey,
                KeyboardUpDown.Down);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = machine.Exec(menuItems, Keys.D4,
                KeyboardUpDown.Down);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.LoadBank);
            r1.NextBank.IsNull();

            r1 = machine.Exec(menuItems, Keys.D4,
                KeyboardUpDown.Up);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.None);
            machine.ClearInternalStatuses();

            r1 = machine.Exec(menuItems, Keys.LShiftKey,
                KeyboardUpDown.Up);
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
            var r1 = machine.Exec(menuItems, Keys.LWin,
                KeyboardUpDown.Down);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = machine.Exec(menuItems, Keys.D4,
                KeyboardUpDown.Down);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.LoadBank);
            r1.NextBank.IsNull();

            r1 = machine.Exec(menuItems, Keys.D4,
                KeyboardUpDown.Up);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.None);
            r1.NextBank.Is("");
            machine.ClearInternalStatuses();

            r1 = machine.Exec(menuItems, Keys.LWin,
                KeyboardUpDown.Up);
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
            var r1 = machine.Exec(menuItems, Keys.LControlKey,
                KeyboardUpDown.Down);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = machine.Exec(menuItems, Keys.LControlKey,
                KeyboardUpDown.Down);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = machine.Exec(menuItems, Keys.LControlKey,
                KeyboardUpDown.Down);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            // 長期間同じキーをDownしていると Down イベントは複数回発生するが、
            // Up イベントは1度しか発生しない
            r1 = machine.Exec(menuItems, Keys.LControlKey,
                KeyboardUpDown.Up);
            r1.ShouldCancel.Is(false);
            r1.Status.Is(ExecStatus.None);

            r1 = machine.Exec(menuItems, Keys.S,
                KeyboardUpDown.Down);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.LoadBank);

            r1 = machine.Exec(menuItems, Keys.S,
                KeyboardUpDown.Up);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.None);
        }

        [TestMethod]
        [Description("リセットキーに対する動作の検証")]
        public void ExecResetMenu()
        {
            var machine = new StateMachine();
            var config = ConfigUtil.Deserialize(Resources.TestConfig09);
            var bank = config.ApplicationGroups[0].Banks[4];
            bank.Name.Is("UndoRedo");

            var entries = bank.Entries;
            entries.Count.Is(4);
            entries[3].LabelText.Is("キャンセル");
            entries[3].Trigger.Is("NumPad5");
            entries[3].ActionItem.ActionType.Is(ActionType.System);
            entries[3].ActionItem.ActionValue.Is("Cancel");

            //
            // Menu 非表示中
            //
            // ReSharper disable RedundantArgumentDefaultValue
            var r1 = machine.Exec(entries, Keys.NumPad5,
                KeyboardUpDown.Down,
                false);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.LoadBank);

            r1 = machine.Exec(entries, Keys.NumPad5,
                KeyboardUpDown.Up,
                false);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.None);
            // ReSharper restore RedundantArgumentDefaultValue

            //
            // Menu 表示中
            //
            r1 = machine.Exec(entries, Keys.NumPad5,
                KeyboardUpDown.Down,
                true);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.LoadBank);

            r1 = machine.Exec(entries, Keys.NumPad5,
                KeyboardUpDown.Up,
                true);
            r1.ShouldCancel.Is(true);
            r1.Status.Is(ExecStatus.None);
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
            machine.Exec(entries, Keys.NumPad9, KeyboardUpDown.Down);
            machine.Exec(entries, Keys.NumPad9, KeyboardUpDown.Up);
        }

        [TestMethod]
        [Description("#81 自分で出したイベントを自分でキャンセルしてしまう不具合の修正")]
        [SuppressMessage("ReSharper", "RedundantAssignment")]
        [SuppressMessage("ReSharper", "JoinDeclarationAndInitializer")]
        public void SelfEventCancelTest()
        {
            ExecResult r1;

            var machine = new StateMachine();
            var config = ConfigUtil.Deserialize(Resources.TestConfig13);
            var bank = config.ApplicationGroups[0].Banks[0];
            bank.Name.Is("");

            var entries = bank.Entries;
            entries.Count.Is(3);
            entries[0].Trigger.Is("F");
            entries[0].ActionItem.ActionType.Is(ActionType.Send);
            entries[0].ActionItem.ActionValue.Is("g");
            entries[1].Trigger.Is("G");
            entries[1].ActionItem.ActionType.Is(ActionType.Send);
            entries[1].ActionItem.ActionValue.Is("r");

            // F を Down する
            r1 = machine.Exec(entries, Keys.F, KeyboardUpDown.Down);

            // g が発生して、これを受け取ってしまう。
            r1 = machine.Exec(entries, Keys.G, KeyboardUpDown.Down);

            // g はキャンセルしてはいけない
            r1.ShouldCancel.Is(false);
            r1.ActionType.Is(ActionType.None);
            r1.Status.Is(ExecStatus.None);

            // 同上
            r1 = machine.Exec(entries, Keys.G, KeyboardUpDown.Up);
            r1.ShouldCancel.Is(false);
            r1.ActionType.Is(ActionType.None);
            r1.Status.Is(ExecStatus.None);

            // こっちはキャンセルしないといけない
            r1 = machine.Exec(entries, Keys.F, KeyboardUpDown.Up);
            r1.ShouldCancel.Is(true);
            r1.ActionType.Is(ActionType.None);
        }

        #endregion

        #region Test ExecCore

        public static object[] ToaruSource =
        {
            new object[]
            {
                "Key Up",
                new ActionItem
                {
                    ActionType = ActionType.Key,
                    ActionValue = "",
                    NextBank = ""
                },
                KeyboardUpDown.Up,
                new ExecResult(true, ExecStatus.None, "", ActionType.Key, "",
                    KeyboardUpDown.Up)
            },
            new object[]
            {
                "Send Up",
                new ActionItem
                {
                    ActionType = ActionType.Send,
                    ActionValue = "Val1",
                    NextBank = "Bank1"
                },
                KeyboardUpDown.Up,
                new ExecResult(true)
            },
            new object[]
            {
                "Send Down",
                new ActionItem
                {
                    ActionType = ActionType.Send,
                    ActionValue = "Val1",
                    NextBank = "Bank1"
                },
                KeyboardUpDown.Down,
                new ExecResult(true, ExecStatus.LoadBank, "Bank1",
                    ActionType.Send, "Val1",
                    KeyboardUpDown.Down)
            },
            new object[]
            {
                "Menu Up",
                new ActionItem
                {
                    ActionType = ActionType.Menu,
                    ActionValue = "Menu1",
                    NextBank = ""
                },
                KeyboardUpDown.Up,
                new ExecResult(true)
            },
            new object[]
            {
                "Menu Down",
                new ActionItem
                {
                    ActionType = ActionType.Menu,
                    ActionValue = "Menu1",
                    NextBank = ""
                },
                KeyboardUpDown.Down,
                new ExecResult(true, ExecStatus.LoadBank, "", ActionType.Menu,
                    "Menu1", KeyboardUpDown.Down)
            },
            new object[]
            {
                "None Up",
                new ActionItem
                {
                    ActionType = ActionType.None,
                    ActionValue = "Menu1",
                    NextBank = "Bank1"
                },
                KeyboardUpDown.Up,
                new ExecResult(true)
            },
            new object[]
            {
                "None Down",
                new ActionItem
                {
                    ActionType = ActionType.None,
                    ActionValue = "Menu1",
                    NextBank = "Bank1"
                },
                KeyboardUpDown.Down,
                new ExecResult(true, ExecStatus.LoadBank, "Bank1",
                    ActionType.None, "Menu1",
                    KeyboardUpDown.Down)
            },
            new object[]
            {
                "Null ActionItem Down",
                null,
                KeyboardUpDown.Down,
                new ExecResult(true)
            },
            new object[]
            {
                "System Down",
                new ActionItem
                {
                    ActionType = ActionType.System,
                    ActionValue = "Cancel",
                    NextBank = null
                },
                KeyboardUpDown.Down,
                new ExecResult(true, ExecStatus.LoadBank, null,
                    ActionType.System, "Cancel",
                    KeyboardUpDown.Down)
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
                    KeyboardUpDown upDown,
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