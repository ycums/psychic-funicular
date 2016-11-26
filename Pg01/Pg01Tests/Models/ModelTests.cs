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
        [Description("自動非表示機能の有効・無効の切り替え #78")]
        public void SystemCommandToggleAutoHideTest()
        {
            var configB = ConfigUtil.Deserialize(Resources.TestConfig12);
            var windowInfo = new WindowInfo("ClipStudioPaint.exe",
                "新規ファイル.clip - CLIP STUDIO PAINT");

            var model = new Model(configB, new DummySendKeyCode());
            var configBTitle = "Sample12XXXXXXXX";

            model.AutoHide.Is(true);
            model.Basic.Title.Is(configBTitle);
            model.WindowInfo = windowInfo;
            model.ApplicationGroup.Name.Is("CLIP STUDIO PAINT");
            model.Bank.Name.Is("");
            model.Bank.Entries.Count.Is(4);
            model.Bank.Entries[0].LabelText.Is("自動非表示切り替え");
            model.Bank.Entries[0].Trigger.Is("NumPad9");
            model.Bank.Entries[0].ActionItem.ActionType.Is(ActionType.System);
            model.Bank.Entries[0].ActionItem.ActionValue.Is(
                ConstValues.SystemCommandToggleAutoHide);

            // Entry ボタンプレス時
            model.ProcAction(
                model.Bank.Entries[0].ActionItem,
                NativeMethods.KeyboardUpDown.Down);
            model.ProcAction(
                model.Bank.Entries[0].ActionItem,
                NativeMethods.KeyboardUpDown.Up);

            model.AutoHide.Is(false);

            // Entry キープレス時
            var state = new NativeMethods.KeyboardState
            {
                KeyCode = Keys.NumPad9
            };
            model.SetEvent(
                new KeyboardHookedEventArgs(
                    NativeMethods.KeyboardMessage.KeyDown, ref state));
            model.SetEvent(
                new KeyboardHookedEventArgs(
                    NativeMethods.KeyboardMessage.KeyUp, ref state));

            model.AutoHide.Is(true);
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

        [TestMethod]
        [Description("#81 自分で出したイベントを自分でキャンセルしてしまう不具合の修正")]
        public void SelfEventPathThroughTest()
        {
            var config = ConfigUtil.Deserialize(Resources.TestConfig13);
            var dsc = new DummySendKeyCode();
            var model = new Model(config, dsc);
            var windowInfo = new WindowInfo("ClipStudioPaint.exe",
                "新規ファイル.clip - CLIP STUDIO PAINT");
            model.WindowInfo = windowInfo;
            model.ApplicationGroup.Name.Is("CLIP STUDIO PAINT");
            model.Bank.Name.Is("");
            model.Bank.Entries.Count.Is(3);
            model.Bank.Entries[0].Trigger.Is("F");
            model.Bank.Entries[0].ActionItem.ActionType.Is(ActionType.Send);
            model.Bank.Entries[0].ActionItem.ActionValue.Is("g");
            model.Bank.Entries[1].Trigger.Is("G");
            model.Bank.Entries[1].ActionItem.ActionType.Is(ActionType.Send);
            model.Bank.Entries[1].ActionItem.ActionValue.Is("r");

            var ksF = new NativeMethods.KeyboardState { KeyCode = Keys.F };
            var ksG = new NativeMethods.KeyboardState { KeyCode = Keys.G };

            // Fを押す
            model.SetEvent(new KeyboardHookedEventArgs(
                NativeMethods.KeyboardMessage.KeyDown, ref ksF));

            // g を送出する
            dsc.EventLog.Count.Is(1);
            dsc.EventLog[0].Type.Is(DummySendKeyCode.EventType.SendWait);
            dsc.EventLog[0].Value.Is("g");

            // g を自分で受け取ってしまう
            model.SetEvent(new KeyboardHookedEventArgs(
                NativeMethods.KeyboardMessage.KeyDown, ref ksG));
            model.SetEvent(new KeyboardHookedEventArgs(
                NativeMethods.KeyboardMessage.KeyUp, ref ksG));

            // g がキャンセルされて、r が送出されてしまっているとまずい。
            // ただし、g が本当にキャンセルされているかどうかはこの層ではわからない。
            // ここでは代替的に r が送出されていないことを確認する
            dsc.EventLog.Count.Is(1);

            model.SetEvent(new KeyboardHookedEventArgs(
                NativeMethods.KeyboardMessage.KeyUp, ref ksF));
            dsc.EventLog.Count.Is(1);
        }

        [TestMethod]
        [Description("SendWait() がちゃんと呼ばれていることの確認")]
        public void SendWaitCallTest()
        {
            var config = ConfigUtil.Deserialize(Resources.TestConfig13);
            var dsc = new DummySendKeyCode();
            var model = new Model(config, dsc);
            var windowInfo = new WindowInfo("ClipStudioPaint.exe",
                "新規ファイル.clip - CLIP STUDIO PAINT");
            model.WindowInfo = windowInfo;
            model.ApplicationGroup.Name.Is("CLIP STUDIO PAINT");
            model.Bank.Name.Is("");
            model.Bank.Entries.Count.Is(3);
            model.Bank.Entries[2].Trigger.Is("H");
            model.Bank.Entries[2].ActionItem.ActionType.Is(ActionType.Send);
            model.Bank.Entries[2].ActionItem.ActionValue.Is("12345");

            var state = new NativeMethods.KeyboardState
            {
                KeyCode = Keys.H
            };
            model.SetEvent(new KeyboardHookedEventArgs(
                NativeMethods.KeyboardMessage.KeyDown, ref state));
            model.SetEvent(new KeyboardHookedEventArgs(
                NativeMethods.KeyboardMessage.KeyUp, ref state));

            dsc.EventLog.Count.Is(1);
            dsc.EventLog[0].Type.Is(DummySendKeyCode.EventType.SendWait);
            dsc.EventLog[0].Value.Is("12345");
        }

        [TestMethod]
        [Description("LoadBank 直後の Send の不発を解決 #82")]
        public void SendAfterLoadBankTest()
        {
            var config = ConfigUtil.Deserialize(Resources.TestConfig14);
            var dsc = new DummySendKeyCode();
            var model = new Model(config, dsc);
            var windowInfo = new WindowInfo("ClipStudioPaint.exe",
                "新規ファイル.clip - CLIP STUDIO PAINT");
            model.WindowInfo = windowInfo;
            model.ApplicationGroup.Name.Is("CLIP STUDIO PAINT");
            model.Bank.Name.Is("");
            model.Bank.Entries.Count.Is(2);
            model.Bank.Entries[0].Trigger.Is("F");
            model.Bank.Entries[0].ActionItem.ActionType.Is(ActionType.None);
            model.Bank.Entries[0].ActionItem.ActionValue.IsNull();
            model.Bank.Entries[0].ActionItem.NextBank.Is("Bank01");
            model.Bank.Entries[1].Trigger.Is("R");
            model.Bank.Entries[1].ActionItem.ActionType.Is(ActionType.Send);
            model.Bank.Entries[1].ActionItem.ActionValue.Is("a");
            model.Bank.Entries[1].ActionItem.NextBank.Is("");

            var ksF = new NativeMethods.KeyboardState { KeyCode = Keys.F };
            var ksG = new NativeMethods.KeyboardState { KeyCode = Keys.G };
            var ksR = new NativeMethods.KeyboardState { KeyCode = Keys.R };

            // Fを押す
            model.SetEvent(new KeyboardHookedEventArgs(
                NativeMethods.KeyboardMessage.KeyDown, ref ksF));
            dsc.EventLog.Count.Is(0);

            // この時点で LoadBank が完了していなければならない。
            model.Bank.Name.Is("Bank01");
            model.Bank.Entries.Count.Is(2);
            model.Bank.Entries[0].Trigger.Is("G");
            model.Bank.Entries[0].ActionItem.ActionType.Is(ActionType.Send);
            model.Bank.Entries[0].ActionItem.ActionValue.Is("r");
            model.Bank.Entries[0].ActionItem.NextBank.Is("");
            model.Bank.Entries[1].Trigger.Is("R");
            model.Bank.Entries[1].ActionItem.ActionType.Is(ActionType.Send);
            model.Bank.Entries[1].ActionItem.ActionValue.Is("b");
            model.Bank.Entries[1].ActionItem.NextBank.Is("");

            // f(down)のまま g(down) を送出する
            model.SetEvent(new KeyboardHookedEventArgs(
                NativeMethods.KeyboardMessage.KeyDown, ref ksG));

            // この時に SendWait("r") が実行済みでなければならない。
            dsc.EventLog.Count.Is(1);
            dsc.EventLog[0].Type.Is(DummySendKeyCode.EventType.SendWait);
            dsc.EventLog[0].Value.Is("r");

            // 当然、LoadBank も完了済みでなければならない。
            model.Bank.Name.Is("");
            model.Bank.Entries.Count.Is(2);

            // 自分で出した r を受け取ってしまう。
            model.SetEvent(new KeyboardHookedEventArgs(
                NativeMethods.KeyboardMessage.KeyDown, ref ksR));
            dsc.EventLog.Count.Is(1);

            model.SetEvent(new KeyboardHookedEventArgs(
                NativeMethods.KeyboardMessage.KeyUp, ref ksR));
            dsc.EventLog.Count.Is(1);

            // g(up) と f(up) はどこかのタイミングで発生する。
            // これらのいずれもキャンセルされなければならない。
            // （ただし、ここではキャンセルされたかどうかを正確に把握できない）
            model.SetEvent(new KeyboardHookedEventArgs(
                NativeMethods.KeyboardMessage.KeyUp, ref ksG));
            dsc.EventLog.Count.Is(1);

            model.SetEvent(new KeyboardHookedEventArgs(
                NativeMethods.KeyboardMessage.KeyUp, ref ksF));
            dsc.EventLog.Count.Is(1);
        }
    }
}