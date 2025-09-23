using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Redeem
{
    public class RejectedPanel : MonoBehaviour
    {
        [SerializeField] private Text textDes;
        [SerializeField] private Button btnAgain;

        private Action onAgain;

        public void Init(Action againCallBack)
        {
            onAgain = againCallBack;
        }

        public void Show(string desStr)
        {
            textDes.text = desStr;
        }

        private void OnEnable()
        {
            btnAgain.onClick.AddListener(ClickAgain);
        }

        private void OnDisable()
        {
            btnAgain.onClick.RemoveListener(ClickAgain);
        }

        private void ClickAgain()
        {
            onAgain?.Invoke();
        }
    }
}
