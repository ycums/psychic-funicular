#region

using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pg01.Models;
using Pg01Tests.Properties;
using Pg01Tests.ViewModels;

#endregion

namespace Pg01Tests.Models
{
    [TestClass]
    public class ModelTests
    {
        [TestMethod]
        public void DefaultConfigFileLoadTest()
        {
            var path = ConfigUtil.GetConfigFilePath();
            File.Delete(path);

            var model = new Model();
            model.Basic.Title.Is("Sample01");
            model.ApplicationGroups.IsNotNull();
            model.ApplicationGroup.IsNull();
            model.Bank.IsNull();

            model.WindowInfo = new WindowInfo("ClipStudioPaint.exe", "新規ファイル.clip - CLIP STUDIO PAINT");
            model.ApplicationGroup.IsNotNull();
            model.ApplicationGroup.Name = "CLIP STUDIO PAINT";
            model.Bank.IsNotNull();
            model.Bank.Name.Is("");
            model.Bank.Entries.Count.Is(3);

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
            model.ApplicationGroup.IsNull();
            model.Bank.IsNull();

            // 該当あり
            model.WindowInfo = new WindowInfo("ClipStudioPaint.exe", "新規ファイル.clip - CLIP STUDIO PAINT");
            model.ApplicationGroup.IsNotNull();
            model.ApplicationGroup.Name = "CLIP STUDIO PAINT";
            model.Bank.IsNull();
            model.IsMenuVisible.IsFalse();

            // 該当なし
            model.WindowInfo = new WindowInfo("booboo.exe", "BOOBOO PAINT");
            model.ApplicationGroup.IsNull();
            model.Bank.IsNull();
            model.IsMenuVisible.IsFalse();
        }
    }
}