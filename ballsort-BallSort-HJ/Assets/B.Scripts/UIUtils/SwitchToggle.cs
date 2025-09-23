using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SwitchToggle : MonoBehaviour
{
    public Action<bool> OnValueChange = delegate { };


    [SerializeField] public bool IsOn;

    private void OnEnable()
    {
        _btnToggle.onClick.AddListener(ClickButton);
    }

    private void OnDisable()
    {
        _btnToggle.onClick.RemoveListener(ClickButton);
    }

    private void Start()
    {
        
    }

    public void Init(bool isOn)
    {
        IsOn = isOn;

        _toggleToy.DOAnchorPos(new Vector2(IsOn ? _onX : _offX, _toggleToy.anchoredPosition.y), _moveDuration)
            .OnComplete(() =>
            {
                SetImageState();
            });
    }

    private void ClickButton()
    {
        IsOn = !IsOn;
        _toggleToy.DOAnchorPos(new Vector2(IsOn ? _onX : _offX, _toggleToy.anchoredPosition.y), _moveDuration)
            .OnComplete(() =>
            {
                SetImageState();
                OnValueChange.Invoke(IsOn);
                // Debug.Log(IsOn);
            });
    }

    private void SetImageState()
    {
        _onImage.gameObject.SetActive(IsOn);
        _offImage.gameObject.SetActive(!IsOn);
    }

    [SerializeField] private Button _btnToggle = null;
    [SerializeField] private RectTransform _toggleToy = null;
    [SerializeField] private Image _onImage = null;
    [SerializeField] private Image _offImage = null;
    [SerializeField] private float _onX = -1;
    [SerializeField] private float _offX = 1;
    [SerializeField] private float _moveDuration = 0.5f;
}