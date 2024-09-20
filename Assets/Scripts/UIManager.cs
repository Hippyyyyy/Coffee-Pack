using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject MainMenu;
    [SerializeField] GameObject InGame;
    [SerializeField] GameObject CompletePanel;
    [SerializeField] Image imgbgComplete;
    [SerializeField] Transform popUp;
    [SerializeField] Button btnPlay;
    [SerializeField] Image img;

    private void Start()
    {
        MainMenu.SetActive(true);
        btnPlay.onClick.AddListener(Play);
    }

    void Play()
    {
        Fade(() =>
        {
            MainMenu.SetActive(false);
            InGame.SetActive(true);
        });
    }

    void Fade(Action onDone = null)
    {
        img.DOFade(1f, 0.5f).OnComplete(() =>
        {
            onDone?.Invoke();
            img.DOFade(0f, 0.5f);
        });
    }
    
    public void CompleteGame()
    {
        CompletePanel.gameObject.SetActive(true);
        imgbgComplete.DOFade(0.7f, 1.5f).OnComplete(() =>
        {
            popUp.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
        });
    }
}
