using System;
using UnityEngine;

public class CoinFlyUnit : MonoBehaviour
{
    public void Play( int num, Action onCompleted)
    {
        Vector2 formScreen = UICamera.Instance.Camera.WorldToScreenPoint(_form.position);
        Vector2 toScreen = UICamera.Instance.Camera.WorldToScreenPoint(_to.position);

        CoinFlyAnim.Instance.Play(num, formScreen, toScreen, onCompleted);
    }

    [SerializeField] private Transform _form;
    [SerializeField] private Transform _to;
}