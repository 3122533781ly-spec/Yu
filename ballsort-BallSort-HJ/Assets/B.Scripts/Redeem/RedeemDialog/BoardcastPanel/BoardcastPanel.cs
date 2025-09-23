using DG.Tweening;
using Lei31.Localizetion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Redeem
{
    public class BoardcastPanel : MonoBehaviour
    {
        [SerializeField] private List<CashOutPlayerHeadItem> headItems;
        [SerializeField] private Text textMsg;
        [SerializeField] private Transform beginPoint;
        [SerializeField] private Transform midPoint;
        [SerializeField] private Transform endPoint;
        [SerializeField] private RectTransform headContent;
        public float Startx;
        public float Starty;
        public float Endx;
        public float Endy;

        public float moveSpeed = 100f; // 控制移动速度，可以在 Unity 编辑器中调整
        private Vector2 moveDirection = new Vector2(-1, 0); // 移动方向，可以在 Unity 编辑器中修改

        private DateTime lastRefresh;
        private List<HeadIconData> showHeadDatas = new List<HeadIconData>();

        private string[] msgStrs = new string[]
        {
            "1", "2", "3", "4", "5", "6", "7", "8", "9",
            "a", "b", "c", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v",
            "w", "x", "y", "z"
        };

        private void Start()
        {
            headContent.localPosition = new Vector3(Startx, Starty, 0);

            showHeadDatas.Clear();
            for (int i = 0; i < headItems.Count; i++)
            {
                HeadIconData headData = HeadIconConfig.Instance.RandomGet();
                headItems[i].Show(headData.Icon, GetRandomMoney());
                showHeadDatas.Add(headData);
            }

            HandleShowMsg();
        }

        private void OnEnable()
        {
        }

        private void OnDisable()
        {
        }

        private void Update()
        {
            MoveHeadContent();
        }

        private void MoveHeadContent()
        {
            // 按照 moveDirection 方向移动
            headContent.localPosition += new Vector3(moveDirection.x, moveDirection.y, 0) * moveSpeed * Time.deltaTime;
            if (headContent.localPosition.x < Endx)
            {
                showHeadDatas.Clear();
                for (int i = 0; i < headItems.Count; i++)
                {
                    HeadIconData headData = HeadIconConfig.Instance.RandomGet();
                    headItems[i].Show(headData.Icon, GetRandomMoney());
                    showHeadDatas.Add(headData);
                }
                headContent.localPosition = new Vector3(6339, Starty, 0);
            }
        }

        private void HandleShowMsg()
        {
            textMsg.text =
                $"{GetRandomAccount()} {LocalizationManager.Instance.GetTextByTag(LocalizationConst.JUST_CASHED_OUT)} ${GetRandomMoney()}";

            textMsg.transform.DOKill();
            textMsg.transform.position = beginPoint.position;
            textMsg.transform.DOMove(midPoint.position, 5f).OnComplete(() =>
            {
                textMsg.transform.DOMove(endPoint.position, 5f).SetDelay(5f).OnComplete(() => { HandleShowMsg(); });
            });
        }

        private int GetRandomMoney()
        {
            int randomIndex = UnityEngine.Random.Range(0, MoneyRedeemConfig.Instance.All.Count);
            return 100; //MoneyRedeemConfig.Instance.All[randomIndex].money
        }

        private string GetRandomAccount()
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < 12; i++)
            {
                if (i == 0 || i > 9)
                {
                    stringBuilder.Append(GetRandomMsg());
                }
                else
                {
                    stringBuilder.Append("*");
                }
            }

            stringBuilder.Append("@gmail.com");
            return stringBuilder.ToString();
        }

        private string GetRandomMsg()
        {
            int randomIndex = UnityEngine.Random.Range(0, msgStrs.Length);
            return msgStrs[randomIndex];
        }
    }
}