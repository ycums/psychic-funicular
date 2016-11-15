using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pg01.Models;
using Pg01.ViewModels;
using Pg01.Views.Behaviors.Util;
using Pg01Tests.Properties;

namespace Pg01Tests.ViewModels
{
    [TestClass()]
    public class MainWindowViewModelTests
    {
        [TestMethod()]
        public void InitializeTest()
        {
            var configA = ConfigUtil.Deserialize(Resources.TestConfig05);
            var configB = ConfigUtil.Deserialize(Resources.TestConfig06);

            var model = new Model(configA);
            var vm = new MainWindowViewModel(model);
            vm.Initialize();
            model.WindowInfo = new WindowInfo("ClipStudioPaint.exe", "新規ファイル.clip - CLIP STUDIO PAINT");
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
            model.ProcAction(vm.Buttons[0].ActionItem, NativeMethods.KeyboardUpDown.Up);
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

            var model = new Model(configA);
            var vm = new MainWindowViewModel(model);
            vm.Initialize();
            model.WindowInfo = new WindowInfo("ClipStudioPaint.exe", "新規ファイル.clip - CLIP STUDIO PAINT");
            vm.ApplicationGroupName.Is("CLIP STUDIO PAINT");
            vm.BankName.Is("(default)");
            vm.Buttons.Count.Is(5);
            vm.Buttons[2].Key.Is("NumPad3");
            vm.Buttons[2].LabelText.Is("メニュー");
            vm.Buttons[2].ActionItem.ActionType.Is(ActionType.Menu);
            vm.Buttons[2].ActionItem.ActionValue.Is("menu01");

            model.ProcAction(vm.Buttons[2].ActionItem, NativeMethods.KeyboardUpDown.Up);
            model.IsMenuVisible.Is(true);
            // vm は MenuViewModel のインスタンスを公開していないのでとりあえずモデルだけ操作してみる
            model.Menu.Name.Is("menu01");
            model.Menu.MenuItem.Count.Is(2);
            model.Menu.MenuItem[0].LabelText.Is("閉じる");
            model.ProcAction(model.Menu.MenuItem[0].ActionItem, NativeMethods.KeyboardUpDown.Up);
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

            var model = new Model(config);
            var vm = new MainWindowViewModel(model);
            vm.Initialize();
            model.WindowInfo = new WindowInfo("ClipStudioPaint.exe", "新規ファイル.clip - CLIP STUDIO PAINT");
            model.ApplicationGroup.Name.Is("CLIP STUDIO PAINT B");
            vm.ApplicationGroupName.Is("CLIP STUDIO PAINT B");
            vm.BankName.Is("(default)");
            vm.Buttons.Count.Is(3);
            vm.Buttons[0].Key.Is("NumPad9");
            vm.Buttons[0].LabelText.Is("前景B");
            vm.Buttons[0].ActionItem.ActionType.Is(ActionType.Send);
            vm.Buttons[0].ActionItem.ActionValue.Is(" B");
            vm.Buttons[0].ActionItem.NextBank.Is("曲線");
            model.ProcAction(vm.Buttons[0].ActionItem, NativeMethods.KeyboardUpDown.Up);
            vm.BankName.Is("曲線");

            model.Basic.ResetKey.Is("NumPad5");

            var state = new NativeMethods.KeyboardState {KeyCode = Keys.NumPad5};
            vm.Event = new KeyboardHookedEventArgs(NativeMethods.KeyboardMessage.KeyDown, ref state);
            vm.BankName.Is("(default)");
        }


        [TestMethod]
        [Description("リセットキーでメニューを閉じる機能の検証")]
        public void MenuResetTest()
        {
            var config = ConfigUtil.Deserialize(Resources.TestConfig07);

            var model = new Model(config);
            var vm = new MainWindowViewModel(model);
            vm.Initialize();
            model.WindowInfo = new WindowInfo("ClipStudioPaint.exe", "新規ファイル.clip - CLIP STUDIO PAINT");
            model.ApplicationGroup.Name.Is("CLIP STUDIO PAINT B");
            vm.ApplicationGroupName.Is("CLIP STUDIO PAINT B");
            vm.BankName.Is("(default)");
            vm.Buttons.Count.Is(3);
            vm.Buttons[2].Key.Is("NumPad3");
            vm.Buttons[2].LabelText.Is("メニューB");
            vm.Buttons[2].ActionItem.ActionType.Is(ActionType.Menu);
            vm.Buttons[2].ActionItem.ActionValue.Is("menu01");
            vm.Buttons[2].ActionItem.NextBank.IsNull();

            model.ProcAction(vm.Buttons[2].ActionItem, NativeMethods.KeyboardUpDown.Up);

            model.IsMenuVisible.Is(true);
            model.Menu.Name.Is("menu01");
            model.Menu.MenuItem.Count.Is(2);
            model.Menu.MenuItem[0].LabelText.Is("閉じる");

            model.Basic.ResetKey.Is("NumPad5");

            var state = new NativeMethods.KeyboardState {KeyCode = Keys.NumPad5};
            vm.Event = new KeyboardHookedEventArgs(NativeMethods.KeyboardMessage.KeyDown, ref state);
            vm.BankName.Is("(default)");
            model.IsMenuVisible.Is(false);
        }

        [TestMethod]
        [Description("リセットキーでメニューを閉じるときに Bank のリセットがかからないようにする")]
        public void MenuResetTest02()
        {
            var config = ConfigUtil.Deserialize(Resources.TestConfig07);

            var model = new Model(config);
            var vm = new MainWindowViewModel(model);
            vm.Initialize();
            model.WindowInfo = new WindowInfo("ClipStudioPaint.exe", "新規ファイル.clip - CLIP STUDIO PAINT");
            model.ApplicationGroup.Name.Is("CLIP STUDIO PAINT B");
            vm.ApplicationGroupName.Is("CLIP STUDIO PAINT B");
            vm.BankName.Is("(default)");
            vm.Buttons.Count.Is(3);

            vm.Buttons[1].Key.Is("NumPad6");
            vm.Buttons[1].LabelText.Is("Bank2");
            vm.Buttons[1].ActionItem.ActionType.Is(ActionType.None);
            vm.Buttons[1].ActionItem.NextBank.Is("Bank2");

            model.ProcAction(vm.Buttons[1].ActionItem, NativeMethods.KeyboardUpDown.Up);

            vm.BankName.Is("Bank2");
            vm.Buttons.Count.Is(1);

            vm.Buttons[2].Key.Is("NumPad3");
            vm.Buttons[2].LabelText.Is("メニューB");
            vm.Buttons[2].ActionItem.ActionType.Is(ActionType.Menu);
            vm.Buttons[2].ActionItem.ActionValue.Is("menu01");
            vm.Buttons[2].ActionItem.NextBank.IsNull();

            model.ProcAction(vm.Buttons[2].ActionItem, NativeMethods.KeyboardUpDown.Up);

            model.IsMenuVisible.Is(true);
            model.Menu.Name.Is("menu01");
            model.Menu.MenuItem.Count.Is(2);
            model.Menu.MenuItem[0].LabelText.Is("閉じる");

            model.Basic.ResetKey.Is("NumPad5");

            var state = new NativeMethods.KeyboardState {KeyCode = Keys.NumPad5};
            vm.Event = new KeyboardHookedEventArgs(NativeMethods.KeyboardMessage.KeyDown, ref state);
            vm.BankName.Is("Bank2");
            model.IsMenuVisible.Is(false);
        }
    }
}