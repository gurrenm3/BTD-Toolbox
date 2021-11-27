using System.Windows;
using BTDToolbox.Wpf.ViewModels;

namespace BTDToolbox.Extensions
{
	public static class WindowExtensions
	{
		public static void ChangeView<T>(this Window window) where T : IViewModel, new()
		{
			window.DataContext = new T();
		}
	}
}