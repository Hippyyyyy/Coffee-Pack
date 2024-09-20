using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    [SerializeField] ParticleSystem parDone;

    public void Done(Action done = null)
    {
        parDone.Play();
        transform.DOScale(0f, 0.2f).SetEase(Ease.OutBack).OnComplete(()=> done());
    }
}
