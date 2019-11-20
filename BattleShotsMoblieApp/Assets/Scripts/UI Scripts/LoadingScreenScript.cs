using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreenScript : MonoBehaviour
{
    Animator animator;

    private void OnEnable()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("Loading", true);
    }

    private void OnDisable()
    {
        animator.SetBool("Loading", false);
    }
}
