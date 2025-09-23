using System.Collections;
using System.Collections.Generic;
using ProjectSpace.Lei31Utils.Scripts.Framework.App;
using UnityEngine;

namespace Redeem
{
    public class PayPalPanel : MonoBehaviour
    {
        [SerializeField] private PayPalEntry payPalEntry;

        private List<PayPalEntry> payPalEntries = new List<PayPalEntry>();

        private void OnEnable()
        {
            CreateAllEntries();
            Game.Instance.GetSystem<RedeemSystem>().onNewPayPalEntryUnlock += HandleNewPayPalEntryUnlock;
        }

        private void OnDisable()
        {
            DestroyAllEntries();
            Game.Instance.GetSystem<RedeemSystem>().onNewPayPalEntryUnlock -= HandleNewPayPalEntryUnlock;
        }

        private void CreateAllEntries()
        {
            //   payPalEntry.gameObject.SetActive(false);

            foreach (var item in Game.Instance.GetSystem<RedeemSystem>().Model.payPalEntryDatas)
            {
                //    CreateEntry(item);
            }
        }

        private void CreateEntry(PayPalEntryData data)
        {
            PayPalEntry newEntry = Instantiate(payPalEntry, payPalEntry.transform.parent);
            newEntry.Show(data);
            newEntry.gameObject.SetActive(true);
            payPalEntries.Add(newEntry);
        }

        private void DestroyAllEntries()
        {
            foreach (var item in payPalEntries)
            {
                Destroy(item.gameObject);
            }
            payPalEntries.Clear();
        }

        private void HandleNewPayPalEntryUnlock(PayPalEntryData data)
        {
            //    CreateEntry(data);
        }
    }
}