using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pg01.Models;
using Pg01Tests.Properties;

namespace Pg01Tests.Models
{
    [TestClass]
    public class ConfigUtilTests
    {
        [TestMethod]
        public void ApplicationGroupsTest()
        {
            var config = ConfigUtil.Deserialize(Resources.TestConfig01);
            var applicationGroup = config.ApplicationGroups[0];
            applicationGroup.Name.Is("CLIP STUDIO PAINT");
            var menuGroup = applicationGroup.Banks[0];
            menuGroup.Name.Is(string.Empty);
        }

        [TestMethod]
        [Description("DPGest #143 設定GUI から Key に 半角スペースを設定できない不具合の修正")]
        public void SerializeTest()
        {
            var config = ConfigUtil.Deserialize(Resources.TestConfig02);
            var applicationGroup = config.ApplicationGroups[0];
            applicationGroup.Name.Is("CLIP STUDIO PAINT");
            var bank = applicationGroup.Banks[0];
            bank.Name.Is(string.Empty);
            var mi = bank.Entries[0];
            mi.Trigger.Is("Num9");
            var miActionItem = mi.ActionItem;
            miActionItem.ActionType.Is(ActionType.Send);
            miActionItem.ActionValue.Is(" ");

            using (var stream = new MemoryStream())
            {
                ConfigUtil.Serialize(config, stream);
                var uni = Encoding.GetEncoding("UTF-8").GetString(stream.ToArray());
                uni.Contains("ActionValue=\" \"").IsTrue();
            }
        }

        [TestMethod]
        public void SerializeTest02()
        {
            var config = GetSerializeTest02Param();
            using (var stream = new MemoryStream())
            {
                ConfigUtil.Serialize(config, stream);
                var uni = Encoding.GetEncoding("UTF-8").GetString(stream.ToArray());
                new Regex("<Entries").Matches(uni).Count.Is(0);
                new Regex("<Entry").Matches(uni).Count.Is(2);
                new Regex("<MenuItem").Matches(uni).Count.Is(2);
                uni.Is(Resources.TestConfig03);
            }
        }

        private static Config GetSerializeTest02Param()
        {
            return new Config
            {
                Basic = new Basic
                {
                    Title = "Title01",
                    WindowLocation = new Location {X = 99999, Y = 0},
                    Buttons = new List<ButtonItem>
                    {
                        new ButtonItem {X = 0, Y = 0, Key = "Num9"},
                        new ButtonItem {X = 0, Y = 1, Key = "Num6"},
                        new ButtonItem {X = 0, Y = 2, Key = "Num3"},
                        new ButtonItem {X = 0, Y = 3, Key = "Num5"},
                        new ButtonItem {X = 8, Y = 8, Key = "Num8"}
                    }
                },
                ApplicationGroups = new List<ApplicationGroup>
                {
                    new ApplicationGroup
                    {
                        Name = "CLIP STUDIO PAINT",
                        MatchingRoule = new MatchingRoule
                        {
                            ExeName = "ClipStudioPaint.exe",
                            WindowTitlePatterns = new List<string>
                            {
                                "*CLIP STUDIO PAINT",
                                "ショートカット設定"
                            }
                        },
                        Banks = new List<Bank>
                        {
                            new Bank
                            {
                                Name = "",
                                Entries = new List<Entry>
                                {
                                    new Entry
                                    {
                                        Trigger = "Num9",
                                        LabelText = "前景",
                                        BackColor = Color.FromRgb(0, 0, 255),
                                        ActionItem = new ActionItem
                                        {
                                            ActionType = ActionType.Send,
                                            ActionValue = " ",
                                            NextBank = "曲線"
                                        }
                                    },
                                    new Entry
                                    {
                                        Trigger = "Num9",
                                        LabelText = "前景",
                                        BackColor = Color.FromRgb(0, 0, 255),
                                        ActionItem = new ActionItem
                                        {
                                            ActionType = ActionType.Send,
                                            ActionValue = " ",
                                            NextBank = "曲線"
                                        }
                                    }
                                }
                            }
                        },
                        Menus = new List<Menu>
                        {
                            new Menu
                            {
                                Name = "menu01",
                                MenuItem = new List<MenuItem>
                                {
                                    new MenuItem
                                    {
                                        LabelText = "前景",
                                        BackColor = Color.FromRgb(0, 0, 255),
                                        X = 0,
                                        Y = 0,
                                        Action = new ActionItem
                                        {
                                            ActionType = ActionType.None
                                        }
                                    },
                                    new MenuItem
                                    {
                                        LabelText = "前景",
                                        BackColor = Color.FromRgb(0, 0, 255),
                                        X = 0,
                                        Y = 0,
                                        Action = new ActionItem
                                        {
                                            ActionType = ActionType.None
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}