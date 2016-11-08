﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Media;
using Livet;
using Pg01.Behaviors.Util;

namespace Pg01.Models
{
    [Serializable]
    public class Config : NotificationObject
    {
        /*
         * NotificationObjectはプロパティ変更通知の仕組みを実装したオブジェクトです。
         */

        #region Initialize & Finalize

        public Config()
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
                                Name = "base",
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
                                            ActionValue = "wd",
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
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }

        #endregion

        #region Functions

        public bool LoadFile(string path)
        {
            try
            {
                var tmp = ConfigUtil.LoadConfigFile(path);
                Basic = tmp.Basic;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

        public bool SaveFile(string path)
        {
            try
            {
                ConfigUtil.SaveConfigFile(this, path);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

        public void SetEvent(KeyboardHookedEventArgs e)
        {
            Debug.WriteLine($"{e.KeyCode} {e.UpDown}");
        }

        #endregion

        #region Properties

        #region Basic変更通知プロパティ

        private Basic _Basic;

        public Basic Basic
        {
            get { return _Basic; }
            set
            {
                if (_Basic == value)
                    return;
                _Basic = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #endregion
    }
}