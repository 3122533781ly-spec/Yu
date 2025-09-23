using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using ProjectSpace.Lei31Utils.Scripts.Utils2.Dialog;
using Spine;
using UnityEngine;
using UnityEngine.UI;

public class HIntText : MonoBehaviour
{
    [SerializeField] private Text _HintNumber;

    public void Refresh(int id)
    {
        // 修正变量引用并使用传入的id参数
        _HintNumber.text = $"{id}";
    }
}