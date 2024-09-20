using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cup : MonoBehaviour
{
    [SerializeField] CupType type;
    [SerializeField] List<GameObject> listCupType;
    [SerializeField] GameObject lid;
    [SerializeField] Animator lidAnimator;

    public CupType Type { get => type; set => type = value; }

    private void Awake()
    {
        foreach (var cup in listCupType) { cup.SetActive(false); }
    }

    public void ActiveCupType()
    {
        listCupType[(int)Type].gameObject.SetActive(true);
        lid = listCupType[(int)Type].transform.GetChild(0).gameObject;
        lidAnimator = lid.GetComponent<Animator>();
    }

    public void LidAnimation()
    {
        lid.gameObject.SetActive(true);
        lidAnimator.Play("LidAnimation");
    }
}
