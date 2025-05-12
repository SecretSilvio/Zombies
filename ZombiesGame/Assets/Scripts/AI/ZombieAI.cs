using System;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    public float wanderRadius = 10f;
    public float sightRange = 15f;
    public float attackRange = 2f;
    public float timeBetweenWanders = 5f;
    public float attackCooldown = 2f;

    [Range(0, 180)]
    public float fieldOfView = 110f; // degrees

    private NavMeshAgent agent;
    private Transform target;
    private bool targetDeath = false;
    private float wanderTimer;
    private float attackTimer;

    private Animator animator;

    private enum ZombieState { Wandering, Chasing, Attacking }
    private ZombieState currentState = ZombieState.Wandering;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        wanderTimer = timeBetweenWanders;
        attackTimer = 0f;
    }

    void Update()
    {
        attackTimer += Time.deltaTime;
        
        switch (currentState)
        {

            case ZombieState.Wandering:
                Wander();
                LookForTarget();
                break;

            case ZombieState.Chasing:
                Chase();
                break;

            case ZombieState.Attacking:
                Attack();
                break;
        }
    }

    void Wander()
    {
        wanderTimer += Time.deltaTime;
        if (wanderTimer >= timeBetweenWanders)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius);
            agent.SetDestination(newPos);
            wanderTimer = 0f;
        }
    }

    void LookForTarget()
    {
        GameObject[] humans = GameObject.FindGameObjectsWithTag("Human");
        foreach (GameObject human in humans)
        {
            if (CanSeeTarget(human.transform))
            {
                target = human.transform;
                currentState = ZombieState.Chasing;
                return;
            }
        }
    }

    void Chase()
    {
        targetDeath = false;
        if (target == null || !CanSeeTarget(target))
        {
            target = null;
            currentState = ZombieState.Wandering;
            return;
        }

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= attackRange)
        {
            agent.isStopped = true;
            currentState = ZombieState.Attacking;
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
        }
    }

    void Attack()
    {
        if (target == null || Vector3.Distance(transform.position, target.position) > attackRange || !CanSeeTarget(target))
        {
            currentState = ZombieState.Chasing;
            agent.isStopped = false;
            return;
        }

        if (targetDeath)
        {
            currentState = ZombieState.Wandering;
            target = null;
            agent.isStopped = false;
            return;
        }

        if (attackTimer >= attackCooldown)
        {
            if (animator != null)
                animator.SetTrigger("Attack");

            Killable killable = target.GetComponent<Killable>();
            if (killable != null)
            {
                killable.Die();
                targetDeath = true;
            }

            attackTimer = 0f;
        }
    }

    bool CanSeeTarget(Transform t)
    {
        Vector3 directionToTarget = (t.position - transform.position).normalized;
        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

        if (angleToTarget < fieldOfView / 2f && Vector3.Distance(transform.position, t.position) <= sightRange)
        {
            RaycastHit hit;
            Vector3 origin = transform.position + Vector3.up * 0.5f; // Eye height
            Vector3 destination = t.position + Vector3.up * 0.5f;

            if (Physics.Raycast(origin, (destination - origin).normalized, out hit, sightRange))
            {
                if (hit.transform == t)
                    return true; // Line of sight confirmed
            }
        }

        return false;
    }

    Vector3 RandomNavSphere(Vector3 origin, float dist)
    {
        Vector3 randDirection = UnityEngine.Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, NavMesh.AllAreas);
        return navHit.position;
    }
}