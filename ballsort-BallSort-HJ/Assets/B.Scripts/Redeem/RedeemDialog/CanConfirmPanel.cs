using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Redeem
{
    public class CanConfirmPanel : MonoBehaviour
    {
        [SerializeField] private Button btnConfirm;

        private Action onConfirm;

        public void Init(Action confirmCallBack)
        {
            onConfirm = confirmCallBack;
        }


        private void OnEnable()
        {
            btnConfirm.onClick.AddListener(ClickConfirm);
        }

        private void OnDisable()
        {
            btnConfirm.onClick.RemoveListener(ClickConfirm);
        }

        private void ClickConfirm()
        {
            onConfirm?.Invoke();
        }
    }
}
