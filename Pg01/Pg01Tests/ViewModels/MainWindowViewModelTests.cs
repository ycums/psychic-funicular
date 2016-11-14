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

    }
}