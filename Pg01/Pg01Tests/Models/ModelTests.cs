using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pg01.Models;

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

            model.LoadApplicationGroup(
                "ClipStudioPaint.exe",
                "新規ファイル.clip - CLIP STUDIO PAINT");
            model.ApplicationGroup.IsNotNull();
            model.ApplicationGroup.Name = "CLIP STUDIO PAINT";
            model.Bank.IsNotNull();
            model.Bank.Name.Is("");
            model.Bank.Entries.Count.Is(3);

            model.IsMenuVisible.IsFalse();
        }
    }
}