#region

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

        private int _cntModifiersDown;
        private int _keySending;

        #endregion

        #region Initialize & Finalize

        public StateMachine()
        {
            ClearInternalStatuses();
        }


        public void ClearInternalStatuses()
        {
            _keySending = 0;
            _cntModifiersDown = 0;
        }

        #endregion

        #region Functions

        public ExecResult Exec(List<Entry> entries, Keys keyCode, NativeMethods.KeyboardUpDown upDown)
        {
            if (entries == null) return new ExecResult(false);

            var modifilers = new[]
            {
                Keys.LControlKey, Keys.LShiftKey, Keys.LMenu,
                Keys.RControlKey, Keys.RShiftKey, Keys.RMenu,
                Keys.Control, Keys.Shift, Keys.Alt,
                Keys.ControlKey, Keys.ShiftKey
            };

            if (keyCode == Keys.Escape)
                return new ExecResult(false, ExecStatus.LoadGroup, string.Empty);

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
                    if (0 != _cntModifiersDown)
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
                _cntModifiersDown--;
            else
                _cntModifiersDown++;
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
                            return new ExecResult(true, ExecStatus.LoadGroup, item.NextBank, ActionType.Send,
                                item.ActionValue, kud);
                        default:
                            return new ExecResult(true);
                    }
                case ActionType.Send:
                    switch (kud)
                    {
                        case NativeMethods.KeyboardUpDown.Up:
                            return new ExecResult(true, ExecStatus.LoadGroup, item.NextBank,
                                ActionType.Send, item.ActionValue, kud);
                        default:
                            return new ExecResult(true);
                    }
                default:
                    return new ExecResult(true, ExecStatus.None, "",
                        ActionType.Menu, item.ActionValue, kud);
            }
        }

        #endregion
    }
}