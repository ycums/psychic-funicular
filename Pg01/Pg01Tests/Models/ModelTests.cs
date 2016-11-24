#region

using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pg01.Models;
using Pg01.Models.Util;
using Pg01.Views.Behaviors.Util;
using Pg01Tests.Properties;
using Pg01Tests.ViewModels;

#endregion

namespace Pg01Tests.Models
{
    [TestClass]
    public class ModelTests
    {
        [TestMethod]
        [Description("ActionItem を空にすると落ちる #61")]
        public void NullActionItemTest()
        {
            var config = ConfigUtil.Deserialize(Resources.TestConfig08);
            var model = new Model(config, new DummySendKeyCode());
            model.Basic.Title.Is("Title01");
            model.ApplicationGroups.IsNotNull();
            model.ApplicationGroup.Name.Is("");
            model.Bank.Name.Is("");

            // 該当あり
            model.WindowInfo = new WindowInfo("ClipStudioPaint.exe",
                "新規ファイル.clip - CLIP STUDIO PAINT");
            model.ApplicationGroup.IsNotNull();
            model.ApplicationGroup.Name.Is("CLIP STUDIO PAINT B");
            model.Bank.Name.Is("");
            model.IsMenuVisible.IsFalse();

            model.Bank.Entries.Count.Is(3);

            model.Bank.Entries[0].LabelText.Is("#F000");
            model.Bank.Entries[0].Background.ToString()
                .Is(Util.ConvertFromString<Brush>("#F000").ToString());
            model.Bank.Entries[0].Trigger.Is("NumPad9");
            model.Bank.Entries[0].ActionItem.IsNull();

            // Entry ボタンプレス時
            model.ProcAction(model.Bank.Entries[0].ActionItem,
                NativeMethods.KeyboardUpDown.Down);
            model.ProcAction(model.Bank.Entries[0].ActionItem,
                NativeMethods.KeyboardUpDown.Up);

            // Entry キープレス時
            var state = new NativeMethods.KeyboardState
            {
                KeyCode = Keys.NumPad5
            };
            model.SetEvent(
                new KeyboardHookedEventArgs(
                    NativeMethods.KeyboardMessage.KeyDown, ref state));
            model.SetEvent(
                new KeyboardHookedEventArgs(
                    NativeMethods.KeyboardMessage.KeyUp, ref state));

            var menus = model.ApplicationGroup.Menus;
            menus.Count.Is(1);
            menus[0].Name.Is("menu01");
            menus[0].MenuItem.Count.Is(2);

            var items = menus[0].MenuItem;
            items[0].LabelText.Is("#F0F0");
            items[0].Background.ToString()
                .Is(Util.ConvertFromString<Brush>("#F0F0").ToString());
            items[0].ActionItem.IsNull();

            // MenuItem ボタンプレス時 (キープレスは今のところありえない）
            model.ProcAction(items[0].ActionItem,
                NativeMethods.KeyboardUpDown.Down);
            model.ProcAction(items[0].ActionItem,
                NativeMethods.KeyboardUpDown.Up);
        }

        [TestMethod]
        public void DefaultConfigFileLoadTest()
        {
            var path = ConfigUtil.GetConfigFilePath();
            File.Delete(path);

            var model = new Model();
            model.Basic.Title.Is("Sample01");
            model.ApplicationGroups.IsNotNull();
            model.ApplicationGroup.Name.Is("");
            model.Bank.Name.Is("");

            model.WindowInfo = new WindowInfo("ClipStudioPaint.exe",
                "新規ファイル.clip - CLIP STUDIO PAINT");
            model.ApplicationGroup.IsNotNull();
            model.ApplicationGroup.Name = "CLIP STUDIO PAINT";
            model.Bank.IsNotNull();
            model.Bank.Name.Is("");
            model.Bank.Entries.Count.Is(10);

            model.IsMenuVisible.IsFalse();
        }

        [TestMethod]
        [Description("該当する ApplicationGroup がない場合に落ちないことの検証")]
        public void NoMatchingApplicationGroupTest()
        {
            var config = ConfigUtil.Deserialize(Resources.TestConfig07);
            var model = new Model(config, new DummySendKeyCode());
            model.Basic.Title.Is("Title01");
            model.ApplicationGroups.IsNotNull();
            model.ApplicationGroup.Name.Is("");
            model.Bank.Name.Is("");

            // 該当あり
            model.WindowInfo = new WindowInfo("ClipStudioPaint.exe",
                "新規ファイル.clip - CLIP STUDIO PAINT");
            model.ApplicationGroup.IsNotNull();
            model.ApplicationGroup.Name.Is("CLIP STUDIO PAINT B");
            model.Bank.Name.Is("");
            model.IsMenuVisible.IsFalse();

            // 該当なし
            model.WindowInfo = new WindowInfo("booboo.exe", "BOOBOO PAINT");
            model.ApplicationGroups.IsNotNull();
            model.ApplicationGroup.Name.Is("");
            model.IsMenuVisible.IsFalse();

            var state = new NativeMethods.KeyboardState
            {
                KeyCode = Keys.NumPad5
            };
            var e =
                new KeyboardHookedEventArgs(
                    NativeMethods.KeyboardMessage.KeyUp, ref state);
            model.SetEvent(e);
        }

        [TestMethod]
        [Description("背景色がデフォルトで設定されないと見づらいので機能追加 #59")]
        public void DefaultMenuItemBackgrowndColor()
        {
            var config = ConfigUtil.Deserialize(Resources.TestConfig08);
            var model = new Model(config, new DummySendKeyCode());
            model.Basic.Title.Is("Title01");
            model.ApplicationGroups.IsNotNull();
            model.ApplicationGroup.Name.Is("");
            model.Bank.Name.Is("");

            // 該当あり
            model.WindowInfo = new WindowInfo("ClipStudioPaint.exe",
                "新規ファイル.clip - CLIP STUDIO PAINT");
            model.ApplicationGroup.IsNotNull();
            model.ApplicationGroup.Name.Is("CLIP STUDIO PAINT B");
            model.Bank.Name.Is("");
            model.IsMenuVisible.IsFalse();

            model.Bank.Entries.Count.Is(3);

            model.Bank.Entries[0].LabelText.Is("#F000");
            model.Bank.Entries[0].Background.ToString()
                .Is(Util.ConvertFromString<Brush>("#F000").ToString());

            model.Bank.Entries[1].LabelText.Is("No BackColor");
            model.Bank.Entries[1].Background.IsNotNull();
            model.Bank.Entries[1].Background.ToString()
                .Is(Util.DefaultBrush.ToString());

            var menus = model.ApplicationGroup.Menus;
            menus.Count.Is(1);
            menus[0].Name.Is("menu01");
            menus[0].MenuItem.Count.Is(2);

            var items = menus[0].MenuItem;
            items[0].LabelText.Is("#F0F0");
            items[0].Background.ToString()
                .Is(Util.ConvertFromString<Brush>("#F0F0").ToString());

            items[1].LabelText.Is("No BackColor");
            items[1].Background.IsNotNull();
            items[1].Background.ToString().Is(Util.DefaultBrush.ToString());
        }

        [TestMethod]
        [Description("システムコマンド「状態のキャンセル」を追加 #73")]
        public void SystemCommandCancelTest()
        {
            var config = ConfigUtil.Deserialize(Resources.TestConfig09);
            var model = new Model(config, new DummySendKeyCode());
            model.Basic.Title.Is("Sample01");
            model.Basic.Buttons.Count.Is(4);
            model.ApplicationGroups.IsNotNull();
            model.ApplicationGroup.Name.Is("");
            model.Bank.Name.Is("");

            // 該当あり
            model.WindowInfo = new WindowInfo("ClipStudioPaint.exe",
                "新規ファイル.clip - CLIP STUDIO PAINT");
            model.ApplicationGroup.IsNotNull();
            model.ApplicationGroup.Name.Is("CLIP STUDIO PAINT");
            model.Bank.Name.Is("");
            model.IsMenuVisible.IsFalse();

            model.Bank.Entries.Count.Is(4);

            model.Bank.Entries[3].LabelText.Is("キャンセル");
            model.Bank.Entries[3].Trigger.Is("NumPad5");
            model.Bank.Entries[3].ActionItem.ActionType.Is(ActionType.System);
            model.Bank.Entries[3].ActionItem.ActionValue.Is("Cancel");


            //
            // とりあえずエラーが出ないことを確認
            //

            // Entry ボタンプレス時
            model.ProcAction(
                model.Bank.Entries[3].ActionItem,
                NativeMethods.KeyboardUpDown.Down);
            model.ProcAction(
                model.Bank.Entries[3].ActionItem,
                NativeMethods.KeyboardUpDown.Up);

            // Entry キープレス時
            var state = new NativeMethods.KeyboardState
            {
                KeyCode = Keys.NumPad5
            };
            model.SetEvent(
                new KeyboardHookedEventArgs(
                    NativeMethods.KeyboardMessage.KeyDown, ref state));
            model.SetEvent(
                new KeyboardHookedEventArgs(
                    NativeMethods.KeyboardMessage.KeyUp, ref state));

            //
            // メニューのリセットの確認
            //
            model.Bank.Entries[2].LabelText.Is("メニュー");
            model.Bank.Entries[2].Trigger.Is("NumPad3");
            model.Bank.Entries[2].ActionItem.ActionType.Is(ActionType.Menu);
            model.Bank.Entries[2].ActionItem.ActionValue.Is("menu01");

            // menu01 を読み込ませる
            model.ProcAction(
                model.Bank.Entries[2].ActionItem,
                NativeMethods.KeyboardUpDown.Down);
            model.ProcAction(
                model.Bank.Entries[2].ActionItem,
                NativeMethods.KeyboardUpDown.Up);
            model.IsMenuVisible.Is(true);
            model.Menu.Name.Is("menu01");

            // キャンセル処理させる
            model.ProcAction(
                model.Bank.Entries[3].ActionItem,
                NativeMethods.KeyboardUpDown.Down);
            model.ProcAction(
                model.Bank.Entries[3].ActionItem,
                NativeMethods.KeyboardUpDown.Up);

            model.IsMenuVisible.Is(false);
            model.Bank.Name.Is("");
        }

        [TestMethod]
        [Description("システムコマンド「ファイルの再読み込み」を追加 #74")]
        public void SystemCommandReloadConfigTest()
        {
            var configB = ConfigUtil.Deserialize(Resources.TestConfig12);
            var windowInfo = new WindowInfo("ClipStudioPaint.exe",
                "新規ファイル.clip - CLIP STUDIO PAINT");

            var model = new Model();
            var defaultConfigTitle = model.Basic.Title;
            var configBTitle = "Sample12XXXXXXXX";

            // 念のため確認
            defaultConfigTitle.IsNot(configBTitle);

            model.Config = configB;
            model.Basic.Title.Is(configBTitle);
            model.WindowInfo = windowInfo;
            model.ApplicationGroup.Name.Is("CLIP STUDIO PAINT");
            model.Bank.Name.Is("");
            model.Bank.Entries.Count.Is(4);
            model.Bank.Entries[3].LabelText.Is("再読み込み");
            model.Bank.Entries[3].Trigger.Is("NumPad5");
            model.Bank.Entries[3].ActionItem.ActionType.Is(ActionType.System);
            model.Bank.Entries[3].ActionItem.ActionValue.Is("ReloadConfig");

            // Entry ボタンプレス時
            model.ProcAction(
                model.Bank.Entries[3].ActionItem,
                NativeMethods.KeyboardUpDown.Down);
            model.ProcAction(
                model.Bank.Entries[3].ActionItem,
                NativeMethods.KeyboardUpDown.Up);

            // デフォルトの config.xml が読み込まれていればOK
            model.Basic.Title.Is(defaultConfigTitle);

            // 再度 configB を読み込み
            model.Config = configB;
            model.Basic.Title.Is(configBTitle);
            model.WindowInfo = windowInfo;
            model.ApplicationGroup.Name.Is("CLIP STUDIO PAINT");
            model.Bank.Name.Is("");
            model.Bank.Entries.Count.Is(4);
            model.Bank.Entries[3].LabelText.Is("再読み込み");
            model.Bank.Entries[3].Trigger.Is("NumPad5");
            model.Bank.Entries[3].ActionItem.ActionType.Is(ActionType.System);
            model.Bank.Entries[3].ActionItem.ActionValue.Is("ReloadConfig");

            // Entry キープレス時
            var state = new NativeMethods.KeyboardState
            {
                KeyCode = Keys.NumPad5
            };
            model.SetEvent(
                new KeyboardHookedEventArgs(
                    NativeMethods.KeyboardMessage.KeyDown, ref state));
            model.SetEvent(
                new KeyboardHookedEventArgs(
                    NativeMethods.KeyboardMessage.KeyUp, ref state));

            // デフォルトの config.xml が読み込まれていればOK
            model.Basic.Title.Is(defaultConfigTitle);
        }

        [TestMethod]
        [Description("マウスオーバーでの自動非表示機能を追加する #49")]
        public void AutoHideTest01()
        {
            var config = ConfigUtil.Deserialize(Resources.TestConfig12);
            var model = new Model(config, new DummySendKeyCode());

            // 初期化チェック
            model.AutoHide.Is(true); 
            model.MainWindowVisibility.Is(Visibility.Visible);

            // マウスカーソルが入った
            model.OnMouse = true;
            model.MainWindowVisibility.Is(Visibility.Hidden);

            model.OnMouse = false;
            model.MainWindowVisibility.Is(Visibility.Visible);

            model.OnMouse = true;
            model.MainWindowVisibility.Is(Visibility.Hidden);

            // OnMouse==true の時に AutoHide が false になったら 
            // MainWindowVisibility==Visible にならなければならない
            model.AutoHide = false;
            model.MainWindowVisibility.Is(Visibility.Visible);

            model.AutoHide = true;
            model.MainWindowVisibility.Is(Visibility.Hidden);

            model.OnMouse = false;
            model.MainWindowVisibility.Is(Visibility.Visible);
        }

    }
}