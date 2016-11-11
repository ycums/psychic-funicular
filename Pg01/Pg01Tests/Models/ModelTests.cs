using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pg01.Models;
using Pg01Tests.Properties;

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

            var config = new Model();
            config.ApplicationGroup.IsNull();
            config.Basic.Title.Is("Sample01");
        }

        [TestMethod]
        public void SetEventTest()
        {
            var config = ConfigUtil.Deserialize(Resources.TestConfig02);
        }
    }
}