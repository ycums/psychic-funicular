#region

using System;
using System.Collections.Generic;
using System.Linq;
using Livet;

#endregion

namespace GcTest.ViewModels
{
    internal static class ViewModelManager
    {
        private static readonly Dictionary<Type, List<ViewModel>>
            ViewModels = new Dictionary<Type, List<ViewModel>>();

        /// <summary>
        ///     ビューモデルのインスタンスを登録します。
        ///     初めて登録されるビューモデルの場合には、自動的に登録するビューモデルのエントリを作成します。
        /// </summary>
        /// <param name="viewModel"></param>
        public static void AddEntryViewModel(ViewModel viewModel)
        {
            var type = viewModel.GetType();
            if (!ViewModels.ContainsKey(type))
            {
                var list = new List<ViewModel>();
                ViewModels.Add(type, list);
            }
            ViewModels[type].Add(viewModel);
        }

        /// <summary>
        ///     指定されたビューモデルのインスタンスの数を返します。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int Count(Type type)
        {
            return !ViewModels.ContainsKey(type) ? 0 : ViewModels[type].Count;
        }

        /// <summary>
        ///     ビューモデルのインスタンスの登録を解除します。
        ///     ウィンドウの Closed イベントが発生した際に呼び出されるようにしてください。
        /// </summary>
        /// <param name="viewModel"></param>
        public static void RemoveEntryViewModel(ViewModel viewModel)
        {
            var type = viewModel.GetType();
            if (!ViewModels.ContainsKey(type)) return;
            ViewModels[type].Remove(viewModel);
        }

        /// <summary>
        ///     指定されたビューモデルのインスタンスの IRaiseCloseMessage インターフェイス の
        ///     RaiseCloseMessage メソッドを実行します。
        /// </summary>
        /// <returns></returns>
        public static void CloseViewModels(Type type)

        {
            if (!ViewModels.ContainsKey(type) ||
                (ViewModels[type].Count == 0)) return;

            if (!(ViewModels[type].First() is IRaiseCloseMessage))
            {
                throw new InvalidCastException(
                    "オブジェクトは IRaiseCloseMessage インターフェイスを実装していません。: " + type);
            }

            var list = new List<ViewModel>();
            ViewModels[type].ForEach(n => list.Add(n));
            list.ForEach(n =>
            {
                var raiseCloseMessage = n as IRaiseCloseMessage;
                raiseCloseMessage?.RaiseCloseMessage();
            });
        }
    }
}