using BTDToolbox.Extensions;
using BTDToolbox.Lib.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BTDToolbox.Lib.UI
{
    public class PopupAction
    {
        public PopupButtons ButtonsToShow { get; set; }
        public List<Action> OnOkayClicked { get; set; }
        public List<Action> OnYesClicked { get; set; }
        public List<Action> OnNoClicked { get; set; }
        public List<Action> OnAnyButtonPressed { get; set; }

        public PopupAction(PopupButtons buttonsToShow = PopupButtons.Okay)
        {
            ButtonsToShow = buttonsToShow;
            OnAnyButtonPressed = new List<Action>();
            OnOkayClicked = new List<Action>() { () => OnAnyButtonPressed?.InvokeAll() };
            OnYesClicked = new List<Action>() { () => OnAnyButtonPressed?.InvokeAll() };
            OnNoClicked = new List<Action>() { () => OnAnyButtonPressed?.InvokeAll() };
        }


        #region Okay button

        public PopupAction(Action onOkayClicked)
        {
            ButtonsToShow = PopupButtons.Okay;
            OnAnyButtonPressed = new List<Action>();
            OnOkayClicked = new List<Action>() { onOkayClicked, () => OnAnyButtonPressed?.InvokeAll() };
            OnYesClicked = new List<Action>() { () => OnAnyButtonPressed?.InvokeAll() };
            OnNoClicked = new List<Action>() { () => OnAnyButtonPressed?.InvokeAll() };
        }
        public PopupAction(List<Action> onOkayClicked)
        {
            ButtonsToShow = PopupButtons.Okay;

            OnAnyButtonPressed = new List<Action>();
            OnOkayClicked = onOkayClicked;
            OnOkayClicked.Add(() => OnAnyButtonPressed?.InvokeAll());

            OnYesClicked = new List<Action>() { () => OnAnyButtonPressed?.InvokeAll() };
            OnNoClicked = new List<Action>() { () => OnAnyButtonPressed?.InvokeAll() };
        }

        #endregion



        #region Yes/No buttons

        public PopupAction(Action onYesClicked, Action onNoClicked)
        {
            ButtonsToShow = PopupButtons.Yes_No;
            OnAnyButtonPressed = new List<Action>();
            OnOkayClicked = null;
            OnYesClicked = new List<Action>() { onYesClicked, () => OnAnyButtonPressed?.InvokeAll() }; ;
            OnNoClicked = new List<Action>() { onNoClicked, () => OnAnyButtonPressed?.InvokeAll() }; ;
        }
        public PopupAction(List<Action> onYesClicked, List<Action> onNoClicked)
        {
            ButtonsToShow = PopupButtons.Yes_No;
            OnAnyButtonPressed = new List<Action>();
            OnOkayClicked = null;

            OnYesClicked = onYesClicked;
            OnYesClicked.Add(() => OnAnyButtonPressed?.InvokeAll());

            OnNoClicked = onNoClicked;
            OnNoClicked.Add(() => OnAnyButtonPressed?.InvokeAll());
        }

        #endregion

    }
}
