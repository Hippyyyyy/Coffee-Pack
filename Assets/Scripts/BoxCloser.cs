using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxCloser : MonoBehaviour
{
    [SerializeField] Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Play()
    {
        animator.Play("BoxCloserAnim");
    }
}
