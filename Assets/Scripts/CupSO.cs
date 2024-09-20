using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "CupSO", menuName = "Game/CupSO")]
public class CupSO : ScriptableObject
{
    [SerializeField] List<Sprite> cupSpr;

    public List<Sprite> CupSpr { get => cupSpr; set => cupSpr = value; }
}
