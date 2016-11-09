using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Pg01.Behaviors.Util;
using Pg01.Models.Util;

namespace Pg01.Models
{
    public class StateMachine
    {
        #region Initialize & Finalize

        public StateMachine() : this(new SendKeyCode())
        {
        }

        public StateMachine(ISendKeyCode skc)
        {
            _skc = skc;
            ClearInternalStatuses();
        }

        public void ClearInternalStatuses()
        {
            _keyControlDown = false;
            _keyShiftDown = false;
            _keyAltDown = false;
            _keyDownCount = 0;
        }

        #endregion

        #region Functions

        public ExecResult Exec(List<Entry> entries, Keys keyCode, NativeMethods.KeyboardUpDown upDown)
        {
            if (entries == null) return new ExecResult(false);
            switch (keyCode)
            {
                case Keys.LControlKey:
                    if (upDown == NativeMethods.KeyboardUpDown.Down)
                    {
                        _keyControlDown = true;
                        _keyDownCount = 0;
                    }
                    else
                    {
                        _keyControlDown = false;
                    }
                    break;
                case Keys.LShiftKey:
                    _keyShiftDown = upDown == NativeMethods.KeyboardUpDown.Down;
                    break;
                case Keys.LMenu:
                    _keyAltDown = upDown == NativeMethods.KeyboardUpDown.Down;
                    break;
                case Keys.Escape:
                    return new ExecResult(false, ExecStatus.LoadGroup, string.Empty);
                default:
                    var keyStr = SendKeyCode.Conv(keyCode);
                    var query = from mi in entries
                        where mi.Trigger == keyStr
                        select mi;
                    var mi1 = query.FirstOrDefault();

                    if ((mi1 != null) && (mi1.ActionItem.ActionType == ActionType.Key) && (mi1.Trigger != null) &&
                        (0 == _keySending))
                        return ExecCore(mi1.ActionItem, upDown);
                    if (_keyControlDown || _keyShiftDown || _keyAltDown)
                    {
                        switch (upDown)
                        {
                            case NativeMethods.KeyboardUpDown.Down:
                                _keyDownCount++;
                                break;
                            case NativeMethods.KeyboardUpDown.Up:
                                _keyDownCount--;
                                break;
                        }
                    }
                    else if (0 < _keyDownCount)
                    {
                        switch (upDown)
                        {
                            case NativeMethods.KeyboardUpDown.Up:
                                _keyDownCount--;
                                break;
                        }
                    }
                    else
                    {
                        if (mi1?.Trigger != null)
                            if (0 == _keySending)
                                return ExecCore(mi1.ActionItem, upDown);
                    }
                    break;
            }
            return new ExecResult(false);
        }

        public ExecResult ExecCore(ActionItem item, NativeMethods.KeyboardUpDown kud)
        {
            switch (kud)
            {
                case NativeMethods.KeyboardUpDown.Down:
                    switch (item.ActionType)
                    {
                        case ActionType.Key:
                            _keySending++;
                            _skc.SendKey(item.ActionValue, kud);
                            _keySending--;

                            break;

                        default:
                            if (0 < item.ActionValue.Length)
                                try
                                {
                                    _keySending++;
                                    _skc.SendWait(item.ActionValue);
                                }
                                catch (ArgumentException aex)
                                {
                                    return new ExecResult(true, ExecStatus.Error,
                                        string.Empty,
                                        $"{aex.Message}\n設定を確認してください。");
                                }
                                finally
                                {
                                    _keySending--;
                                }
                            return new ExecResult(true, ExecStatus.LoadGroup, item.NextBank);
                    }
                    break;
                case NativeMethods.KeyboardUpDown.Up:
                    switch (item.ActionType)
                    {
                        case ActionType.Key:
                            _keySending++;
                            _skc.SendKey(item.ActionValue, kud);
                            _keySending--;

                            return new ExecResult(true, ExecStatus.LoadGroup, item.NextBank);
                    }
                    break;
            }
            return new ExecResult(true);
        }

        #endregion

        #region Fields

        private readonly ISendKeyCode _skc;

        private bool _keyAltDown;
        private bool _keyControlDown;
        private int _keyDownCount;
        private int _keySending;
        private bool _keyShiftDown;

        #endregion
    }
}