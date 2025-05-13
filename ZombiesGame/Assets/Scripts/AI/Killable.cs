using System.Collections;
using UnityEngine;

public class Killable : MonoBehaviour
{
    private Animator animator;

    public void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void Die()
    {
        StartCoroutine(DieCoroutine());
    }

    IEnumerator DieCoroutine()
    {
        // Play death animation
        animator.SetTrigger("Death");
        // Disable any components that should not be active after death
        // Wait for a few seconds before destroying the object
        yield return new WaitForSeconds(4f);
        Destroy(gameObject);
    }
}
