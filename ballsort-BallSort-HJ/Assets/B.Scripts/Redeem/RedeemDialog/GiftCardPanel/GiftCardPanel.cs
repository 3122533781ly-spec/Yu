using System.Collections;
using System.Collections.Generic;
using ProjectSpace.Lei31Utils.Scripts.Framework.App;
using UnityEngine;

namespace Redeem
{
    public class GiftCardPanel : MonoBehaviour
    {
        [SerializeField] private GiftCardEntry giftCardEntry;

        private List<GiftCardEntry> giftCardEntries = new List<GiftCardEntry>();

        private void OnEnable()
        {
            CreateAllEntries();
            Game.Instance.GetSystem<RedeemSystem>().onNewGiftCardEntryUnlock += HandleNewGiftCardEntryUnlock;
        }

        private void OnDisable()
        {
            DestroyAllEntries();
            Game.Instance.GetSystem<RedeemSystem>().onNewGiftCardEntryUnlock -= HandleNewGiftCardEntryUnlock;
        }

        private void CreateAllEntries()
        {
            giftCardEntry.gameObject.SetActive(false);

            foreach (var item in Game.Instance.GetSystem<RedeemSystem>().Model.giftCardEntryDatas)
            {
                CreateEntry(item);
            }
        }

        private void CreateEntry(GiftCardEntryData data)
        {
            GiftCardEntry newEntry = Instantiate(giftCardEntry, giftCardEntry.transform.parent);
            newEntry.Show(data);
            newEntry.gameObject.SetActive(true);
            giftCardEntries.Add(newEntry);
        }

        private void DestroyAllEntries()
        {
            foreach (var item in giftCardEntries)
            {
                Destroy(item.gameObject);
            }
            giftCardEntries.Clear();
        }

        private void HandleNewGiftCardEntryUnlock(GiftCardEntryData data)
        {
            CreateEntry(data);
        }
    }
}
