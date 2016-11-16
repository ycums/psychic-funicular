﻿#region

using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Pg01.Models.Util;
using Pg01.Views.Behaviors.Util;

#endregion

namespace Pg01.Models
{
    public class StateMachine
    {
        #region Fields

        private int _keySending;
        private HashSet<Keys> _downedModifierKeys;

        #endregion

        #region Initialize & Finalize

        public StateMachine()
        {
            ClearInternalStatuses();
        }


        public void ClearInternalStatuses()
        {
            _keySending = 0;
            _downedModifierKeys = new HashSet<Keys>();
        }

        #endregion

        #region Functions

        public ExecResult Exec(List<Entry> entries, Keys keyCode, NativeMethods.KeyboardUpDown upDown,
            string resetKey = "", bool isMenuVisible = false)
        {
            if (entries == null) return new ExecResult(false);

            var modifilers = new[]
            {
                Keys.LControlKey, Keys.LShiftKey, Keys.LMenu,
                Keys.RControlKey, Keys.RShiftKey, Keys.RMenu,
                Keys.Control, Keys.Shift, Keys.Alt,
                Keys.ControlKey, Keys.ShiftKey
            };

            if (SendKeyCode.Conv(keyCode) == resetKey)
            {
                if (upDown == NativeMethods.KeyboardUpDown.Down)
                    return new ExecResult(true);
                return isMenuVisible
                    ? new ExecResult(true, ExecStatus.CloseMenu, string.Empty, ActionType.None, string.Empty, upDown)
                    : new ExecResult(true, ExecStatus.LoadBank, string.Empty, ActionType.None, string.Empty, upDown);
            }

            if (!modifilers.Contains(keyCode))
            {
                var keyStr = SendKeyCode.Conv(keyCode);
                var query = from mi in entries
                    where mi.Trigger == keyStr
                    select mi;
                var mi1 = query.FirstOrDefault();

                if (mi1?.Trigger == null)
                    return new ExecResult(false);

                if (mi1.ActionItem.ActionType != ActionType.Send)
                    return ExecCore(mi1.ActionItem, upDown);

                if (upDown == NativeMethods.KeyboardUpDown.Down)
                {
                    if (0 != _downedModifierKeys.Count)
                        return new ExecResult(false);

                    _keySending++;
                    return ExecCore(mi1.ActionItem, upDown);
                }

                if (0 == _keySending)
                    return new ExecResult(false);

                _keySending--;
                return ExecCore(mi1.ActionItem, upDown);
            }

            if (upDown != NativeMethods.KeyboardUpDown.Down)
                _downedModifierKeys.Remove(keyCode);
            else
                _downedModifierKeys.Add(keyCode);

            return new ExecResult(false);
        }

        public ExecResult ExecCore(ActionItem item, NativeMethods.KeyboardUpDown kud)
        {
            switch (item.ActionType)
            {
                case ActionType.Key:
                    switch (kud)
                    {
                        case NativeMethods.KeyboardUpDown.Down:
                            return new ExecResult(true, ExecStatus.None, "", ActionType.Key, item.ActionValue, kud);
                        case NativeMethods.KeyboardUpDown.Up:
                            return new ExecResult(true, ExecStatus.LoadBank, item.NextBank, ActionType.Send,
                                item.ActionValue, kud);
                        default:
                            return new ExecResult(true);
                    }
                case ActionType.Send:
                    switch (kud)
                    {
                        case NativeMethods.KeyboardUpDown.Up:
                            return new ExecResult(true, ExecStatus.LoadBank, item.NextBank,
                                ActionType.Send, item.ActionValue, kud);
                        default:
                            return new ExecResult(true);
                    }
                case ActionType.Menu:
                    switch (kud)
                    {
                        case NativeMethods.KeyboardUpDown.Up:
                            return new ExecResult(true, ExecStatus.LoadBank, "",
                                ActionType.Menu, item.ActionValue, kud);
                        default:
                            return new ExecResult(true);
                    }
                case ActionType.None:
                    switch (kud)
                    {
                        case NativeMethods.KeyboardUpDown.Up:
                            return new ExecResult(true, ExecStatus.LoadBank, item.NextBank,
                                ActionType.None, item.ActionValue, kud);
                        default:
                            return new ExecResult(true);
                    }
                default:
                    return new ExecResult(true);
            }
        }

        #endregion
    }
}