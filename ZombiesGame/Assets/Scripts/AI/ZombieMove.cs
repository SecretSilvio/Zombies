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
    [SerializeField] Transform target;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        tree = new BehaviorTree("Zombie");

        PrioritySelector selector = new PrioritySelector("Selector");

        Sequence chaseHumanSeq = new Sequence("ChaseHuman", 100);
        chaseHumanSeq.AddChild(new Leaf("canSeeHuman?", new Condition(canSeeHuman)));
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
        tree.Process();
    }

    bool canSeeHuman()
    {
        Vector3 toTarget = (target.position - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, toTarget);
        if (dot > 0.9f)
        {
            return true;
        }
        return false;
    }
}
