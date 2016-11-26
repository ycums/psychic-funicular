#region

using System;
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

        public ExecResult Exec(List<Entry> entries, Keys keyCode,
            NativeMethods.KeyboardUpDown upDown, bool isMenuVisible = false)
        {
            if (entries == null) return new ExecResult(false);

            var modifilers = new[]
            {
                Keys.LControlKey, Keys.LShiftKey, Keys.LMenu,
                Keys.RControlKey, Keys.RShiftKey, Keys.RMenu,
                Keys.Control, Keys.Shift, Keys.Alt,
                Keys.ControlKey, Keys.ShiftKey
            };

            if (!modifilers.Contains(keyCode))
            {
                var keyStr = SendKeyCode.Conv(keyCode);
                var query = from mi in entries
                    where mi.Trigger == keyStr
                    select mi;
                var mi1 = query.FirstOrDefault();

                if (mi1?.Trigger == null)
                    return new ExecResult(false);

                if (mi1.ActionItem == null)
                    return new ExecResult(false);

                switch (mi1.ActionItem.ActionType)
                {
                    case ActionType.Send:
                        switch (upDown)
                        {
                            case NativeMethods.KeyboardUpDown.Down:
                                if (0 != _downedModifierKeys.Count)
                                    return new ExecResult(false);

                                _keySending++;
                                if (1 != _keySending)
                                    return new ExecResult(false);
                                return ExecCore(mi1.ActionItem, upDown);

                            case NativeMethods.KeyboardUpDown.Up:
                                _keySending--;
                                if (0 != _keySending)
                                    return new ExecResult(false);
                                return ExecCore(mi1.ActionItem, upDown);

                            case NativeMethods.KeyboardUpDown.None:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException(
                                    nameof(upDown), upDown, null);
                        }
                        break;

                    case ActionType.None:
                        return _downedModifierKeys.Count == 0
                            ? ExecCore(mi1.ActionItem, upDown)
                            : new ExecResult(false);
                }
                return ExecCore(mi1.ActionItem, upDown);
            }

            if (upDown != NativeMethods.KeyboardUpDown.Down)
                _downedModifierKeys.Remove(keyCode);
            else
                _downedModifierKeys.Add(keyCode);

            return new ExecResult(false);
        }

        public ExecResult ExecCore(ActionItem item,
            NativeMethods.KeyboardUpDown kud)
        {
            if (item == null) return new ExecResult(true);

            switch (item.ActionType)
            {
                case ActionType.Key:
                    switch (kud)
                    {
                        case NativeMethods.KeyboardUpDown.Up:
                            return new ExecResult(true, ExecStatus.None, "",
                                ActionType.Key, item.ActionValue, kud);
                        case NativeMethods.KeyboardUpDown.Down:
                            return new ExecResult(true, ExecStatus.LoadBank,
                                item.NextBank, ActionType.Key,
                                item.ActionValue, kud);
                        default:
                            return new ExecResult(true);
                    }
                default:
                    switch (kud)
                    {
                        case NativeMethods.KeyboardUpDown.Down:
                            return new ExecResult(true, ExecStatus.LoadBank,
                                item.NextBank,
                                item.ActionType, item.ActionValue, kud);
                        default:
                            return new ExecResult(true);
                    }
            }
        }

        #endregion
    }
}