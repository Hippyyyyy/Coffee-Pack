using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GridItem : MonoBehaviour
{
    Node node;
    [SerializeField] SpriteRenderer spr;
    [SerializeField] List<GridItem> neighbors;
    [SerializeField] Tray tray;
    [SerializeField] Box box;
    [SerializeField] ParticleSystem par;
    [SerializeField] ParticleSystem parBox;
    [SerializeField] GridItemType gridItemType;

    public Node Node { get => node; set => node = value; }
    public List<GridItem> Neighbors { get => neighbors; set => neighbors = value; }
    public Tray Tray { get => tray; set => tray = value; }
    public Box Box { get => box; set => box = value; }
    public GridItemType GridItemType { get => gridItemType; set => gridItemType = value; }

    public void FadeIn()
    {
        spr.DOFade(0.7f, 0.1f).SetEase(Ease.OutQuad);
    }
    public void FadeOut()
    {
        spr.DOFade(0f, 0.1f).SetEase(Ease.InQuad);
    }
    public void PlayParticle()
    {
        if (!par) return;
        par.Play();
    }
    public void PlayParticleBox()
    {
        if (!parBox) return;
        parBox.Play();
    }
    public void ActiveBox(GridItemType type)
    {
        if (type == GridItemType.Empty) return;
        if (node == null) return;

        box.gameObject.SetActive(true);
        GridItemType = type;
    }
}
