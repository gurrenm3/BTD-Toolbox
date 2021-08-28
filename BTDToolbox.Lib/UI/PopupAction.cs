using BTDToolbox.Lib.Enums;
using System;
using System.Collections.Generic;

namespace BTDToolbox.Lib.UI
{
    public struct PopupAction
    {
        public PopupButtons ButtonsToShow { get; set; }
        public List<Action> OnOkayClicked { get; set; }
        public List<Action> OnYesClicked { get; set; }
        public List<Action> OnNoClicked { get; set; }

        public PopupAction(PopupButtons buttonsToShow = PopupButtons.Okay)
        {
            ButtonsToShow = buttonsToShow;
            OnOkayClicked = null;
            OnYesClicked = null;
            OnNoClicked = null;
        }

        #region Okay button
        public PopupAction(Action onOkayClicked)
        {
            ButtonsToShow = PopupButtons.Okay;
            OnOkayClicked = new List<Action>() { onOkayClicked };
            OnYesClicked = null;
            OnNoClicked = null;
        }
        public PopupAction(List<Action> onOkayClicked)
        {
            ButtonsToShow = PopupButtons.Okay;
            OnOkayClicked = onOkayClicked;
            OnYesClicked = null;
            OnNoClicked = null;
        }
        #endregion


        #region Yes/No buttons
        public PopupAction(Action onYesClicked, Action onNoClicked)
        {
            ButtonsToShow = PopupButtons.Yes_No;
            OnOkayClicked = null;
            OnYesClicked = new List<Action>() { onYesClicked }; ;
            OnNoClicked = new List<Action>() { onNoClicked }; ;
        }
        public PopupAction(List<Action> onYesClicked, List<Action> onNoClicked)
        {
            ButtonsToShow = PopupButtons.Yes_No;
            OnOkayClicked = null;
            OnYesClicked = onYesClicked;
            OnNoClicked = onNoClicked;
        }
        #endregion
    }
}
