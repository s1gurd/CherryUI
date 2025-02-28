﻿using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CherryUI.InteractiveElements.Presenters
{
    public abstract class PresenterErrorBase : PresenterBase, IPopUp
    {
        [Title("Error Screen Settings")]
        [SerializeField] private TMP_Text errorTitle;
        [SerializeField] private TMP_Text errorMsg;
        [SerializeField] private Button backButton;

        private void Start()
        {
            backButton.onClick.AddListener(ViewService.Back);
        }

        public void SetError(string title, string message)
        {
            errorTitle.text = title;
            errorMsg.text = message;
        }

    }
}