using BTDToolbox.Extensions;
using BTDToolbox.Lib.Enums;
using BTDToolbox.Lib.UI;
using BTDToolbox.Wpf.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private static ContentControl GetPopupPanel()
        {
            if (MainWindow.Instance != null)
                return MainWindow.Instance.popupPanel;
            else if (StartupWindow.Instance != null)
                return StartupWindow.Instance.popupPanel;

            return null;
        }


        public static void ShowWarning(string message) => ShowWarning(message, new PopupAction());
        public static void ShowWarning(string message, PopupAction popupAction)
        {
            Show(message, popupAction, "Warning");
        }


        public static void ShowError(string message) => ShowError(message, new PopupAction());
        public static void ShowError(string message, PopupAction popupAction)
        {
            Show(message, popupAction, "Error");
        }

        public static void Show(string message, string title = "Popup") => Show(message, new PopupAction(), title);
        public static void Show(string message, Action okayClicked, string title = "Popup") => Show(message, new PopupAction(okayClicked), title);
        public static void Show(string message, Action yesClicked, Action noClicked, string title = "Popup") => Show(message, new PopupAction(yesClicked, noClicked), title);
        public static void Show(string message, PopupAction popupAction, string title = "Popup")
        {
            var popup = new Popup();
            popup.Init(message, title, popupAction);
            queuedPopups.Enqueue(popup);
            if (!runningPopups)
                RunQueuedPopups();
        }

        static bool runningPopups = false;
        private static void RunQueuedPopups()
        {
            if (queuedPopups.Count == 0)
            {
                ClosePopup();
                runningPopups = false;
                return;
            }

            var popup = queuedPopups.Dequeue();
            popup.ShowPopup();
            runningPopups = true;
        }
    }
}
