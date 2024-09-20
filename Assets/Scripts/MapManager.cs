using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    [SerializeField] Button btnPlay;


    private void Start()
    {
        btnPlay.onClick.AddListener(Play);
    }

    void Play()
    {

    }

}
