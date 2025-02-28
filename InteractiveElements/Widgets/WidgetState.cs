using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace CherryUI.InteractiveElements.Widgets
{
    [Serializable]
    public class WidgetState
    {
        public string stateName = "";
        public List<WidgetElement> stateElements = new();
        public List<Button> activationButtons = new();
    }
}