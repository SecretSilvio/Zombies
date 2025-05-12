using System;
using System.Collections.Generic;
using Pathfinding.BehaviorTrees;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(NavMeshAgent))]
public class ZombieMove : MonoBehaviour
{
    [SerializeField] private List<Transform> waypoints;

    NavMeshAgent agent;
    BehaviorTree tree;
    [SerializeField] private List<Transform> humans;
    public Transform target = null;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        tree = new BehaviorTree("Zombie");

        PrioritySelector selector = new PrioritySelector("Selector");

        Sequence chaseHumanSeq = new Sequence("ChaseHuman", 100);
        chaseHumanSeq.AddChild(new Leaf("Chase", new ChaseStrategy(transform, agent, target)));
        chaseHumanSeq.AddChild(new Leaf("AttackHuman", new ActionStrategy(() =>
        {
            target.gameObject.SetActive(false);
            Debug.Log("Attacking human!");
        })));

        selector.AddChild(chaseHumanSeq);
        selector.AddChild(new Leaf("Patrol", new PatrolStrategy(transform, agent, waypoints), 50));

        tree.AddChild(selector);
    }

    // Update is called once per frame  
    void Update()
    {
        VisionCheck();

        tree.Process();
    }

    void VisionCheck()
    {
        target = GetClosestTransform(humans);

        // Calculate direction to the target and the dot product for vision cone check
        Vector3 toTarget = (target.position - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, toTarget);

        // If the dot product is greater than the threshold, proceed to raycast check
        if (dot > 0.9f)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            Ray ray = new Ray(transform.position + Vector3.up * 1.5f, toTarget); // Ray starting from the entity's eye level
            RaycastHit hit;
            LayerMask obstacleMask = LayerMask.GetMask("Default"); // Ensure this layer matches obstacles

            // Perform the raycast to see if there are any obstructions
            if (Physics.Raycast(ray, out hit, distanceToTarget, obstacleMask))
            {
                // If the ray hits something other than the target, vision is blocked
                if (hit.transform != target)
                {
                    target = null;
                }
            }
        }

        // If dot product fails, the target is not within the field of view
        else
        {
            target = null;
        }

    }

    Transform GetClosestTransform(List<Transform> transforms)
    {
        if (transforms == null || transforms.Count == 0) return null;

        Transform closestTransform = transforms[0];
        float closestDistance = Vector3.Distance(transform.position, closestTransform.position);

        foreach (Transform t in transforms)
        {
            float distance = Vector3.Distance(transform.position, t.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTransform = t;
            }
        }

        return closestTransform;
    }
}
