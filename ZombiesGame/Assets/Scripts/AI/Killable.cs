using System.Collections;
using UnityEngine;

public class Killable : MonoBehaviour
{
    private Animator animator;
    private ZombieAI zombieAI;
    private HumanAI humanAI;

    public void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        zombieAI = GetComponent<ZombieAI>();
        humanAI = GetComponent<HumanAI>();
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
        if (zombieAI != null)
        {
            zombieAI.enabled = false;
        }
        if (humanAI != null)
        {
            humanAI.enabled = false;
        }
        // Wait for a few seconds before destroying the object
        yield return new WaitForSeconds(4f);
        Destroy(gameObject);
    }
}
