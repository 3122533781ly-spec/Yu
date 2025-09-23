using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineBeeInGameAudio : MonoSingleton<LineBeeInGameAudio>
{
    public void PlayTapPoint()
    {
        AudioManager.Instance.PlaySoundEffect(_clipTapPoint);
    }

    public void PlayWinGame()
    {
        AudioManager.Instance.PlaySoundEffect(_clipWinGame);
    }

    public void PlayLoseGame()
    {
        AudioManager.Instance.PlaySoundEffect(_clipLoseGame);
    }

    public void PlayRewardGet()
    {
        AudioManager.Instance.PlaySoundEffect(_clipRewardGet);
    }

    public void PlayGetCoin()
    {
        AudioManager.Instance.PlaySoundEffect(_clipGetCoin);
    }

    [SerializeField] private AudioClip _clipTapPoint;
    [SerializeField] private AudioClip _clipWinGame;
    [SerializeField] private AudioClip _clipLoseGame;
    [SerializeField] private AudioClip _clipRewardGet;
    [SerializeField] private AudioClip _clipGetCoin;
}