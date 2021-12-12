using BTDToolbox.Extensions;
using BTDToolbox.Lib.Enums;
using BTDToolbox.Lib.UI;
using BTDToolbox.Wpf.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BTDToolbox.Wpf
{
    /// <summary>
    /// Interaction logic for Popup.xaml
    /// </summary>
    public partial class Popup : UserControl
    {
        private static Queue<Popup> queuedPopups = new Queue<Popup>();
        private PopupAction popupAction;
        private bool popupButtonPressed = false;

        public Popup()
        {
            InitializeComponent();
        }

        public static void ClosePopup() => GetPopupPanel().Content = null;
        public void ShowPopup() => GetPopupPanel().Content = this;

        public void Init(string message, string title, PopupAction popupAction)
        {
            popupGrid.MaxHeight = 300;
            popupGrid.MaxWidth = 450;
            popupBody.Text = message;
            popupTitle.Text = title;
            this.popupAction = popupAction;

            if (popupAction.ButtonsToShow == PopupButtons.Okay)
                okayButton.Visibility = Visibility.Visible;
            else if (popupAction.ButtonsToShow == PopupButtons.Yes_No)
                yes_noButtons.Visibility = Visibility.Visible;
        }

        private void okayButton_Click(object sender, RoutedEventArgs e)
        {
            InvokeActionsIfAny(popupAction.OnOkayClicked);
            RunQueuedPopups();
        }

        private void yesButton_Click(object sender, RoutedEventArgs e)
        {
            InvokeActionsIfAny(popupAction.OnYesClicked);
            RunQueuedPopups();
        }

        private void noButton_Click(object sender, RoutedEventArgs e)
        {
            InvokeActionsIfAny(popupAction.OnNoClicked);
            RunQueuedPopups();
        }

        private void InvokeActionsIfAny(List<Action> buttonClickActions)
        {
            if (buttonClickActions != null && buttonClickActions.Any())
                buttonClickActions.InvokeAll();
        }

        private async Task WaitForClose()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    if (popupButtonPressed)
                        break;

                    Thread.Sleep(350);
                }
            });
        }

        private static ContentControl GetPopupPanel()
        {
            if (MainWindow.Instance != null)
                return MainWindow.Instance.popupPanel;
            else if (StartupWindow.Instance != null)
                return StartupWindow.Instance.popupPanel;

            return null;
        }


        public static async Task ShowWarning(string message) => await ShowWarning(message, new PopupAction());
        public static async Task ShowWarning(string message, PopupAction popupAction)
        {
            await Show(message, "Warning", popupAction);
        }


        public static async Task ShowError(string message) => await ShowError(message, new PopupAction());
        public static async Task ShowError(string message, PopupAction popupAction)
        {
            await Show(message, "Error", popupAction);
        }

        public static async Task Show(string message) => await Show(message, "", new PopupAction());
        public static async Task Show(string message, string title) => await Show(message, title, new PopupAction());
        public static async Task Show(string message, string title, Action okayClicked) => await Show(message, title, new PopupAction(okayClicked));
        public static async Task Show(string message, string title, Action yesClicked, Action noClicked) => await Show(message, title, new PopupAction(yesClicked, noClicked));
        public static async Task Show(string message, string title, PopupAction popupAction)
        {
            var popup = new Popup();
            popup.Init(message, title, popupAction);
            queuedPopups.Enqueue(popup);
            if (!runningPopups)
                await RunQueuedPopups();
        }

        static bool runningPopups = false;
        private static async Task RunQueuedPopups()
        {
            if (queuedPopups.Count == 0)
            {
                ClosePopup();
                runningPopups = false;
                return;
            }

            runningPopups = true;
            var popup = queuedPopups.Dequeue();
            popup.ShowPopup();
            popup.popupAction.OnAnyButtonPressed.Add(() => popup.popupButtonPressed = true);
            await popup.WaitForClose();
        }
    }
}
