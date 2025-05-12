using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class HumanAI : MonoBehaviour
{
    public float wanderRadius = 10f;
    public float timeBetweenWanders = 5f;

    public float sightRange = 20f;
    [Range(0, 180)]
    public float fieldOfView = 120f;

    private NavMeshAgent agent;
    private float wanderTimer;

    private enum HumanState { Wandering, Fleeing }
    private HumanState currentState = HumanState.Wandering;

    private Transform currentZombieThreat;
    private float lastSeenTimer; // Time to wait before stopping the flee
    private Transform[] safePoints;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        safePoints = GameObject.FindGameObjectsWithTag("SafePoint").Select(go => go.transform).ToArray();
        wanderTimer = timeBetweenWanders;
    }

    void Update()
    {
        LookForZombies();
        switch (currentState)
        {
            case HumanState.Wandering:
                Wander();
                //LookForZombies();
                break;

            case HumanState.Fleeing:
                Flee();
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

    void LookForZombies()
    {
        Debug.Log("Looking for zombies...");
        GameObject[] zombies = GameObject.FindGameObjectsWithTag("Zombie");

        foreach (GameObject zombie in zombies)
        {
            if (CanSee(zombie.transform))
            {
                Debug.Log("Zombie spotted!");
                currentZombieThreat = zombie.transform;
                currentState = HumanState.Fleeing;
                return;
            }
        }
    }

    void Flee()
    {
        if (currentZombieThreat == null)
        {
            currentState = HumanState.Wandering;
            return;
        }
        else
        {
            lastSeenTimer = 5.0f;
        }
        lastSeenTimer -= Time.deltaTime;
        Transform bestSafePoint = null;
        float closestDist = float.MaxValue;

        foreach (Transform sp in safePoints)
        {
            float distToHuman = Vector3.Distance(transform.position, sp.position);
            float distToZombie = Vector3.Distance(currentZombieThreat.position, sp.position);

            if (distToHuman < distToZombie && distToHuman < closestDist)
            {
                closestDist = distToHuman;
                bestSafePoint = sp;
            }
        }

        if (bestSafePoint != null)
        {
            agent.SetDestination(bestSafePoint.position);
        }
        else
        {
            // No valid safe point: move in the opposite direction
            Vector3 fleeDir = (transform.position - currentZombieThreat.position).normalized;
            Vector3 fleeTarget = transform.position + fleeDir * wanderRadius;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(fleeTarget, out hit, wanderRadius, NavMesh.AllAreas))
                agent.SetDestination(hit.position);
        }

        // Optional: stop fleeing if the zombie is no longer visible
        if (!CanSee(currentZombieThreat) && lastSeenTimer <= 0f)
        {
            currentZombieThreat = null;
            currentState = HumanState.Wandering;
        }
    }

    bool CanSee(Transform t)
    {
        //Debug.Log("Checking visibility...");
        Vector3 directionToTarget = (t.position - transform.position).normalized;
        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

        if (angleToTarget < fieldOfView / 2f && Vector3.Distance(transform.position, t.position) <= sightRange)
        {
            RaycastHit hit;
            Vector3 origin = transform.position + Vector3.up * 0.5f + transform.forward * 0.5f;
            Vector3 destination = t.position + Vector3.up * 0.5f;

            Debug.DrawRay(origin, (destination - origin).normalized, Color.red);
            if (Physics.Raycast(origin, (destination - origin).normalized, out hit, sightRange))
            {
                Debug.Log("Shooting Raycast...");
                if (hit.transform == t)
                    Debug.Log("Zombie spotted!");
                return true;
            }
        }

        return false;
    }

    Vector3 RandomNavSphere(Vector3 origin, float dist)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, NavMesh.AllAreas);
        return navHit.position;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Vector3 leftRay = Quaternion.Euler(0, -fieldOfView / 2f, 0) * transform.forward;
        Vector3 rightRay = Quaternion.Euler(0, fieldOfView / 2f, 0) * transform.forward;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position + Vector3.up * 1.5f, leftRay * sightRange);
        Gizmos.DrawRay(transform.position + Vector3.up * 1.5f, rightRay * sightRange);
    }
}