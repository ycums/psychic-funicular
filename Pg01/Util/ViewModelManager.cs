#region

using System;
using System.Collections.Generic;
using Livet;

#endregion

namespace Pg01Util
{
    public static class ViewModelManager
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
    }
}