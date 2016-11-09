using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pg01.Models;
using Pg01Tests.Properties;

namespace Pg01Tests.Models
{
    [TestClass]
    public class ConfigTests
    {
        [TestMethod]
        public void SetEventTest()
        {
            var config = ConfigUtil.Deserialize(Resources.TestConfig02);
            var applicationGroup = config.ApplicationGroups[0];

        }
    }
}