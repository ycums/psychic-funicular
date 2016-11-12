﻿using System;
using System.Windows.Media;
using JetBrains.Annotations;
using Livet;
using Livet.Commands;
using Pg01.Behaviors.Util;
using Pg01.Models;

namespace Pg01.ViewModels
{
    public class ButtonItemViewModel : ViewModel
    {
        #region Fields

        [UsedImplicitly] private readonly ButtonItem _item;
        private MainWindowViewModel _parent;

        #endregion

        #region Initialize & Finalize

        public ButtonItemViewModel()
        {
            Key = "";
        }

        public ButtonItemViewModel(MainWindowViewModel parent, ButtonItem buttonItem)
        {
            _parent = parent;
            _item = buttonItem;
            Width = ConstValues.ButtonWidth;
            Height = ConstValues.ButtonHeight;

            X = _item.X*Width;
            Y = _item.Y*Height;
            Key = _item.Key;
        }

        public void Initialize()
        {
        }

        #endregion

        #region Properties

        #region Enabled変更通知プロパティ

        private bool _Enabled;

        public bool Enabled
        {
            get { return _Enabled; }
            set
            {
                if (_Enabled == value)
                    return;
                _Enabled = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Background変更通知プロパティ

        private Brush _background;

        public Brush Background
        {
            get { return _background; }
            set
            {
                if (Equals(_background, value))
                    return;
                _background = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region LabelText変更通知プロパティ

        private string _LabelText;

        public string LabelText
        {
            get { return _LabelText; }
            set
            {
                if (_LabelText == value)
                    return;
                _LabelText = value;
                RaisePropertyChanged();
            }
        }

        #endregion


        #region ActionItem変更通知プロパティ
        private ActionItem _ActionItem;

        public ActionItem ActionItem
        {
            get
            { return _ActionItem; }
            set
            { 
                if (_ActionItem == value)
                    return;
                _ActionItem = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        #region X変更通知プロパティ

        private double _X;

        public double X
        {
            get { return _X; }
            set
            {
                if (Math.Abs(_X - value) < ConstValues.TOLERANCE)
                    return;
                _X = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Y変更通知プロパティ

        private double _Y;

        public double Y
        {
            get { return _Y; }
            set
            {
                if (Math.Abs(_Y - value) < ConstValues.TOLERANCE)
                    return;
                _Y = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Key変更通知プロパティ

        private string _Key;

        public string Key
        {
            get { return _Key; }
            set
            {
                if (_Key == value)
                    return;
                _Key = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Width変更通知プロパティ

        private double _Width;

        public double Width
        {
            get { return _Width; }
            set
            {
                if (Math.Abs(_Width - value) < ConstValues.TOLERANCE)
                    return;
                _Width = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Height変更通知プロパティ

        private double _Height;

        public double Height
        {
            get { return _Height; }
            set
            {
                if (Math.Abs(_Height - value) < ConstValues.TOLERANCE)
                    return;
                _Height = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Commands

        #region ButtonCommand

        private ViewModelCommand _ButtonCommand;

        public ViewModelCommand ButtonCommand => _ButtonCommand ?? (_ButtonCommand = new ViewModelCommand(Button));

        public void Button()
        {
            _parent.DoAction(ActionItem, NativeMethods.KeyboardUpDown.Down);
            _parent.DoAction(ActionItem, NativeMethods.KeyboardUpDown.Up);
        }

        #endregion

        #endregion
    }
}