using System.Collections.Generic;
using Pathfinding.BehaviorTrees;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ZombieMove : MonoBehaviour
{
    [SerializeField] private List<Transform> waypoints;

    NavMeshAgent agent;
    BehaviorTree tree;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        tree = new BehaviorTree("Zombie");
        tree.AddChild(new Leaf("Patrol", new PatrolStrategy(transform, agent, waypoints)));
    }

    // Update is called once per frame  
    void Update()
    {
        tree.Process();
    }
}
