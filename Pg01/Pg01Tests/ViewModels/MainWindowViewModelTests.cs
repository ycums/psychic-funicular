#region

using System.Windows.Forms;
using System.Windows.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pg01.Models;
using Pg01.Models.Util;
using Pg01.ViewModels;
using Pg01.Views.Behaviors.Util;
using Pg01Tests.Properties;

#endregion

namespace Pg01Tests.ViewModels
{
    [TestClass]
    public class MainWindowViewModelTests
    {
        [TestMethod]
        public void InitializeTest()
        {
            var configA = ConfigUtil.Deserialize(Resources.TestConfig05);
            var configB = ConfigUtil.Deserialize(Resources.TestConfig06);

            var model = new Model(configA, new DummySendKeyCode());
            var vm = new MainWindowViewModel(model);
            vm.Initialize();
            model.WindowInfo = new WindowInfo("ClipStudioPaint.exe",
                "新規ファイル.clip - CLIP STUDIO PAINT");
            vm.ApplicationGroupName.Is("CLIP STUDIO PAINT");
            vm.BankName.Is("(default)");
            vm.Buttons.Count.Is(5);
            vm.Buttons[0].Key.Is("NumPad9");
            vm.Buttons[0].LabelText.Is("前景");
            vm.Buttons[0].ActionItem.ActionType.Is(ActionType.Send);
            vm.Buttons[0].ActionItem.ActionValue.Is(" ");

            // Config リロード機能
            model.Config = configB;

            model.ApplicationGroup.Name.Is("CLIP STUDIO PAINT B");

            vm.ApplicationGroupName.Is("CLIP STUDIO PAINT B");
            vm.BankName.Is("(default)");
            vm.Buttons.Count.Is(4);
            vm.Buttons[0].Key.Is("NumPad9");
            vm.Buttons[0].LabelText.Is("前景B");
            vm.Buttons[0].ActionItem.ActionType.Is(ActionType.Send);
            vm.Buttons[0].ActionItem.ActionValue.Is(" B");

            // Config リロード 時 Bankリセット機能
            vm.Buttons[0].ActionItem.NextBank.Is("曲線");
            model.ProcAction(vm.Buttons[0].ActionItem,
                NativeMethods.KeyboardUpDown.Up);
            vm.BankName.Is("曲線");

            model.Config = configA;
            vm.ApplicationGroupName.Is("CLIP STUDIO PAINT");
            vm.BankName.Is("(default)");
            vm.Buttons.Count.Is(5);
            vm.Buttons[0].Key.Is("NumPad9");
            vm.Buttons[0].LabelText.Is("前景");
            vm.Buttons[0].ActionItem.ActionType.Is(ActionType.Send);
            vm.Buttons[0].ActionItem.ActionValue.Is(" ");
        }

        [TestMethod]
        [Description("メニューを表示してから Config を再読み込みするとメニューボタンに設定値が反映されてこない不具合の修正")]
        public void ConfigReloadTest01()
        {
            var configA = ConfigUtil.Deserialize(Resources.TestConfig05);
            var configB = ConfigUtil.Deserialize(Resources.TestConfig06);

            var model = new Model(configA, new DummySendKeyCode());
            var vm = new MainWindowViewModel(model);
            vm.Initialize();
            model.WindowInfo = new WindowInfo("ClipStudioPaint.exe",
                "新規ファイル.clip - CLIP STUDIO PAINT");
            vm.ApplicationGroupName.Is("CLIP STUDIO PAINT");
            vm.BankName.Is("(default)");
            vm.Buttons.Count.Is(5);
            vm.Buttons[2].Key.Is("NumPad3");
            vm.Buttons[2].LabelText.Is("メニュー");
            vm.Buttons[2].ActionItem.ActionType.Is(ActionType.Menu);
            vm.Buttons[2].ActionItem.ActionValue.Is("menu01");

            model.ProcAction(vm.Buttons[2].ActionItem,
                NativeMethods.KeyboardUpDown.Up);
            model.IsMenuVisible.Is(true);
            // vm は MenuViewModel のインスタンスを公開していないのでとりあえずモデルだけ操作してみる
            model.Menu.Name.Is("menu01");
            model.Menu.MenuItem.Count.Is(2);
            model.Menu.MenuItem[0].LabelText.Is("閉じる");
            model.ProcAction(model.Menu.MenuItem[0].ActionItem,
                NativeMethods.KeyboardUpDown.Up);
            model.IsMenuVisible.Is(false);


            model.Config = configB;

            model.ApplicationGroup.Name.Is("CLIP STUDIO PAINT B");

            vm.ApplicationGroupName.Is("CLIP STUDIO PAINT B");
            vm.BankName.Is("(default)");
            vm.Buttons.Count.Is(4);
            vm.Buttons[0].Key.Is("NumPad9");
            vm.Buttons[0].LabelText.Is("前景B");
            vm.Buttons[0].ActionItem.ActionType.Is(ActionType.Send);
            vm.Buttons[0].ActionItem.ActionValue.Is(" B");
        }

        [TestMethod]
        [Description("リセットキーでBankをリセットする機能の検証")]
        public void BankResetTest()
        {
            var config = ConfigUtil.Deserialize(Resources.TestConfig07);
            var model = new Model(config, new DummySendKeyCode());
            var vm = new MainWindowViewModel(model);
            vm.Initialize();
            model.WindowInfo = new WindowInfo("ClipStudioPaint.exe",
                "新規ファイル.clip - CLIP STUDIO PAINT");
            model.ApplicationGroup.Name.Is("CLIP STUDIO PAINT B");
            vm.ApplicationGroupName.Is("CLIP STUDIO PAINT B");
            vm.BankName.Is("(default)");
            vm.Buttons.Count.Is(3);
            vm.Buttons[0].Key.Is("NumPad9");
            vm.Buttons[0].LabelText.Is("前景B");
            vm.Buttons[0].ActionItem.ActionType.Is(ActionType.Send);
            vm.Buttons[0].ActionItem.ActionValue.Is(" B");
            vm.Buttons[0].ActionItem.NextBank.Is("曲線");
            model.ProcAction(vm.Buttons[0].ActionItem,
                NativeMethods.KeyboardUpDown.Up);

            // "曲線" Bank の Entries[2] に
            // システムコマンド Cancel が設定されていることを確認
            vm.BankName.Is("曲線");
            model.Bank.Entries.Count.Is(3);
            model.Bank.Entries[2].LabelText.Is("キャンセル");
            model.Bank.Entries[2].Trigger.Is("NumPad5");
            model.Bank.Entries[2].ActionItem.ActionType.Is(ActionType.System);
            model.Bank.Entries[2].ActionItem.ActionValue.Is("Cancel");
            model.Bank.Entries[2].ActionItem.NextBank.IsNull();

            // Cancel を呼び出す
            var state = new NativeMethods.KeyboardState
            {
                KeyCode = Keys.NumPad5
            };
            vm.Event =
                new KeyboardHookedEventArgs(
                    NativeMethods.KeyboardMessage.KeyUp, ref state);

            // Bank がリセットされていればOK
            vm.BankName.Is("(default)");
        }

        [TestMethod]
        [Description("リセットキーでメニューを閉じる機能の検証")]
        public void MenuResetTest()
        {
            var config = ConfigUtil.Deserialize(Resources.TestConfig07);

            var model = new Model(config, new DummySendKeyCode());
            var vm = new MainWindowViewModel(model);
            vm.Initialize();
            model.WindowInfo = new WindowInfo("ClipStudioPaint.exe",
                "新規ファイル.clip - CLIP STUDIO PAINT");
            model.ApplicationGroup.Name.Is("CLIP STUDIO PAINT B");
            vm.ApplicationGroupName.Is("CLIP STUDIO PAINT B");
            vm.BankName.Is("(default)");
            vm.Buttons.Count.Is(3);
            vm.Buttons[2].Key.Is("NumPad3");
            vm.Buttons[2].LabelText.Is("メニューB");
            vm.Buttons[2].ActionItem.ActionType.Is(ActionType.Menu);
            vm.Buttons[2].ActionItem.ActionValue.Is("menu01");
            vm.Buttons[2].ActionItem.NextBank.IsNull();

            model.ProcAction(vm.Buttons[2].ActionItem,
                NativeMethods.KeyboardUpDown.Up);

            model.IsMenuVisible.Is(true);
            model.Menu.Name.Is("menu01");
            model.Menu.MenuItem.Count.Is(2);

            vm.BankName.Is("(default)");
            model.Bank.Name.Is("");
            model.Bank.Entries.Count.Is(4);
            model.Bank.Entries[3].LabelText.Is("キャンセル");
            model.Bank.Entries[3].Trigger.Is("NumPad5");
            model.Bank.Entries[3].ActionItem.ActionType.Is(ActionType.System);
            model.Bank.Entries[3].ActionItem.ActionValue.Is("Cancel");

            var state = new NativeMethods.KeyboardState
            {
                KeyCode = Keys.NumPad5
            };
            vm.Event =
                new KeyboardHookedEventArgs(
                    NativeMethods.KeyboardMessage.KeyUp, ref state);
            vm.BankName.Is("(default)");
            model.IsMenuVisible.Is(false);
        }

        [TestMethod]
        [Description("リセットキーでメニューを閉じるときに Bank のリセットがかからないようにする")]
        public void MenuResetTest02()
        {
            var config = ConfigUtil.Deserialize(Resources.TestConfig07);

            var model = new Model(config, new DummySendKeyCode());
            var vm = new MainWindowViewModel(model);
            vm.Initialize();
            model.WindowInfo = new WindowInfo("ClipStudioPaint.exe",
                "新規ファイル.clip - CLIP STUDIO PAINT");
            model.ApplicationGroup.Name.Is("CLIP STUDIO PAINT B");
            vm.ApplicationGroupName.Is("CLIP STUDIO PAINT B");
            vm.BankName.Is("(default)");
            vm.Buttons.Count.Is(3);

            vm.Buttons[1].Key.Is("NumPad6");
            vm.Buttons[1].LabelText.Is("Bank2");
            vm.Buttons[1].ActionItem.ActionType.Is(ActionType.None);
            vm.Buttons[1].ActionItem.NextBank.Is("Bank2");

            model.ProcAction(vm.Buttons[1].ActionItem,
                NativeMethods.KeyboardUpDown.Down);
            model.ProcAction(vm.Buttons[1].ActionItem,
                NativeMethods.KeyboardUpDown.Up);

            vm.BankName.Is("Bank2");
            vm.Buttons.Count.Is(3);

            vm.Buttons[2].Key.Is("NumPad3");
            vm.Buttons[2].LabelText.Is("メニューB");
            vm.Buttons[2].ActionItem.ActionType.Is(ActionType.Menu);
            vm.Buttons[2].ActionItem.ActionValue.Is("menu01");
            vm.Buttons[2].ActionItem.NextBank.IsNull();

            model.ProcAction(vm.Buttons[2].ActionItem,
                NativeMethods.KeyboardUpDown.Down);
            model.ProcAction(vm.Buttons[2].ActionItem,
                NativeMethods.KeyboardUpDown.Up);

            // menu01 表示後の状態の確認
            model.IsMenuVisible.Is(true);
            model.Menu.Name.Is("menu01");
            model.Menu.MenuItem.Count.Is(2);
            model.Bank.Name.Is("Bank2");
            model.Bank.Entries.Count.Is(3);
            model.Bank.Entries[2].LabelText.Is("キャンセル");
            model.Bank.Entries[2].Trigger.Is("NumPad5");
            model.Bank.Entries[2].ActionItem.ActionType.Is(ActionType.System);
            model.Bank.Entries[2].ActionItem.ActionValue.Is("Cancel");

            // キャンセル機能を呼び出す
            var state = new NativeMethods.KeyboardState
            {
                KeyCode = Keys.NumPad5
            };
            vm.Event =
                new KeyboardHookedEventArgs(
                    NativeMethods.KeyboardMessage.KeyDown, ref state);
            vm.Event =
                new KeyboardHookedEventArgs(
                    NativeMethods.KeyboardMessage.KeyUp, ref state);

            // Bank がリセットされず、メニューが非表示ならOK
            vm.BankName.Is("Bank2");
            model.IsMenuVisible.Is(false);
        }

        [TestMethod]
        [Description("メニューボタンを押してメニューが閉じた後にBankがリセットされないことを検証")]
        public void MenuResetTest03()
        {
            var config = ConfigUtil.Deserialize(Resources.TestConfig07);

            var model = new Model(config, new DummySendKeyCode());
            var vm = new MainWindowViewModel(model);
            vm.Initialize();
            model.WindowInfo = new WindowInfo("ClipStudioPaint.exe",
                "新規ファイル.clip - CLIP STUDIO PAINT");
            model.ApplicationGroup.Name.Is("CLIP STUDIO PAINT B");
            vm.ApplicationGroupName.Is("CLIP STUDIO PAINT B");
            vm.BankName.Is("(default)");
            vm.Buttons.Count.Is(3);

            vm.Buttons[1].Key.Is("NumPad6");
            vm.Buttons[1].LabelText.Is("Bank2");
            vm.Buttons[1].ActionItem.ActionType.Is(ActionType.None);
            vm.Buttons[1].ActionItem.NextBank.Is("Bank2");

            model.ProcAction(vm.Buttons[1].ActionItem,
                NativeMethods.KeyboardUpDown.Down);
            model.ProcAction(vm.Buttons[1].ActionItem,
                NativeMethods.KeyboardUpDown.Up);

            vm.BankName.Is("Bank2");
            vm.Buttons.Count.Is(3);

            vm.Buttons[2].Key.Is("NumPad3");
            vm.Buttons[2].LabelText.Is("メニューB");
            vm.Buttons[2].ActionItem.ActionType.Is(ActionType.Menu);
            vm.Buttons[2].ActionItem.ActionValue.Is("menu01");
            vm.Buttons[2].ActionItem.NextBank.IsNull();

            model.ProcAction(vm.Buttons[2].ActionItem,
                NativeMethods.KeyboardUpDown.Down);
            model.ProcAction(vm.Buttons[2].ActionItem,
                NativeMethods.KeyboardUpDown.Up);

            model.IsMenuVisible.Is(true);
            model.Menu.Name.Is("menu01");
            model.Menu.MenuItem.Count.Is(2);
            model.Menu.MenuItem[0].LabelText.Is("閉じる");
            model.Menu.MenuItem[0].ActionItem.ActionType.Is(ActionType.None);
            model.Menu.MenuItem[0].ActionItem.NextBank.IsNull();
            model.Bank.Name.Is("Bank2");

            model.ProcAction(model.Menu.MenuItem[0].ActionItem,
                NativeMethods.KeyboardUpDown.Down);
            model.ProcAction(model.Menu.MenuItem[0].ActionItem,
                NativeMethods.KeyboardUpDown.Up);

            vm.BankName.Is("Bank2");
            model.IsMenuVisible.Is(false);
        }

        [TestMethod]
        [Description("現在のBankに対応する項目が存在しないときに前のBankの項目名が残ってしまう不具合の修正 #58")]
        public void NullMenuItemTest()
        {
            var config = ConfigUtil.Deserialize(Resources.TestConfig10);

            var model = new Model(config, new DummySendKeyCode());
            var vm = new MainWindowViewModel(model);
            vm.Initialize();
            model.WindowInfo = new WindowInfo("ClipStudioPaint.exe",
                "新規ファイル.clip - CLIP STUDIO PAINT");
            model.ApplicationGroup.Name.Is("CLIP STUDIO PAINT");
            vm.ApplicationGroupName.Is("CLIP STUDIO PAINT");
            vm.BankName.Is("(default)");
            model.Bank.Entries.Count.Is(4);
            vm.Buttons.Count.Is(4);

            vm.Buttons[0].Key.Is("NumPad9");
            vm.Buttons[0].LabelText.Is("前景");
            vm.Buttons[0].ActionItem.ActionType.Is(ActionType.None);
            vm.Buttons[0].ActionItem.NextBank.Is("EmptyBank");
            vm.Buttons[0].Background.ToString()
                .Is(Util.ConvertFromString<Brush>("#F0F0").ToString());

            model.ProcAction(vm.Buttons[0].ActionItem,
                NativeMethods.KeyboardUpDown.Down);
            model.ProcAction(vm.Buttons[0].ActionItem,
                NativeMethods.KeyboardUpDown.Up);

            vm.BankName.Is("EmptyBank");
            model.Bank.Entries.Count.Is(0);
            vm.Buttons.Count.Is(4);

            vm.Buttons[0].Key.Is("NumPad9");
            vm.Buttons[0].LabelText.Is("");
            vm.Buttons[0].ActionItem.ActionType.Is(ActionType.None);
            vm.Buttons[0].ActionItem.ActionValue.Is("");
            vm.Buttons[0].ActionItem.NextBank.IsNull();
            vm.Buttons[0].Background.ToString()
                .Is(Util.DefaultBrush.ToString());
        }

        [TestMethod]
        public void CtrlSTest01()
        {
            var config = ConfigUtil.Deserialize(Resources.TestConfig04);
            var model = new Model(config, new DummySendKeyCode());
            var vm = new MainWindowViewModel(model);
            vm.Initialize();
            model.TimerEnabled = false; // 停止させないとステップ実行させたりするとおかしくなる
            model.WindowInfo = new WindowInfo("ClipStudioPaint.exe",
                "新規ファイル.clip - CLIP STUDIO PAINT");
            model.ApplicationGroup.Name.Is("CLIP STUDIO PAINT");
            vm.ApplicationGroupName.Is("CLIP STUDIO PAINT");
            vm.BankName.Is("(default)");
            model.Bank.Entries.Count.Is(2);
            vm.Buttons.Count.Is(5);
            model.Bank.Entries[0].Trigger.Is("S");
            model.Bank.Entries[0].ActionItem.NextBank.Is("曲線");

            var ksLControl = new NativeMethods.KeyboardState
            {
                KeyCode = Keys.LControlKey
            };
            var ksS = new NativeMethods.KeyboardState
            {
                KeyCode = Keys.S
            };

            vm.Event =
                new KeyboardHookedEventArgs(
                    NativeMethods.KeyboardMessage.KeyDown, ref ksLControl);
            model.Bank.Entries.Count.Is(2);
            model.Bank.Name.IsNot("曲線");
            model.Bank.Name.Is("");

            vm.Event=
                new KeyboardHookedEventArgs(
                    NativeMethods.KeyboardMessage.KeyDown, ref ksS);
            model.Bank.Entries.Count.Is(2);
            model.Bank.Name.IsNot("曲線");
            model.Bank.Name.Is("");

            vm.Event=
                new KeyboardHookedEventArgs(
                    NativeMethods.KeyboardMessage.KeyUp, ref ksS);
            model.Bank.Entries.Count.Is(2);
            model.Bank.Name.IsNot("曲線");
            model.Bank.Name.Is("");

            vm.Event =
                new KeyboardHookedEventArgs(
                    NativeMethods.KeyboardMessage.KeyUp, ref ksLControl);
            // これで Bank が "曲線" だとおかしい
            model.Bank.Entries.Count.Is(2);
            model.Bank.Name.IsNot("曲線");
            model.Bank.Name.Is("");

        }
    }
}