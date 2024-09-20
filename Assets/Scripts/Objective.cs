using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Objective : MonoBehaviour
{
    [SerializeField] Image img;
    [SerializeField] CupType type;
    [SerializeField] Transform tick;
    [SerializeField] Button btn;
    [SerializeField] TextMeshProUGUI text;
    public Image Img { get => img; set => img = value; }
    public CupType Type { get => type; set => type = value; }

    public void SetImg(Sprite spr)
    {
        if(!spr) return;
        img.sprite = spr;
    }

    public void Done()
    {
        btn.interactable = false;
        tick.gameObject.SetActive(true);
        tick.transform.DOScale(1.3f, 0.4f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            tick.transform.DOScale(1f, 0.3f);
        });
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!img) img = transform.GetChild(1).GetComponent<Image>();
    }
#endif
}
