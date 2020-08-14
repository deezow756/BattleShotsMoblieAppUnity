using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitAnimationManager : MonoBehaviour
{
    Animator animator;
    private void OnEnable()
    {
        animator = GetComponent<Animator>();

        animator.SetBool("animate", true);

        Invoke("Disable", 4);
    }

    void Disable()
    {
        animator.SetBool("animate", false);
        this.gameObject.SetActive(false);
    }
}
