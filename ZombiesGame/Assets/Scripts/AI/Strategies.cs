using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Pathfinding.BehaviorTrees
{
    public interface IStrategy
    {
        Node.Status Process();
        void Reset();
    }

    public class PatrolStrategy : IStrategy
    {
        readonly Transform entity;
        readonly NavMeshAgent agent;
        readonly List<Transform> waypoints;
        readonly float patrolSpeed;
        int currentWaypointIndex;
        bool isPathCalculated;

        public PatrolStrategy(Transform entity, NavMeshAgent agent, List<Transform> waypoints, float patrolSpeed = 2f)
        {
            this.entity = entity;
            this.agent = agent;
            this.waypoints = waypoints;
            this.patrolSpeed = patrolSpeed;
            currentWaypointIndex = 0;
        }

        public Node.Status Process()
        {
            if (currentWaypointIndex == waypoints.Count) return Node.Status.Success;

            var target = waypoints[currentWaypointIndex];
            agent.SetDestination(target.position);
            entity.LookAt(target);

            if (isPathCalculated && agent.remainingDistance < agent.stoppingDistance)
            {
                currentWaypointIndex++;
                isPathCalculated = false;
            }
            if (agent.pathPending)
            {
                isPathCalculated = true;
            }

            return Node.Status.Running;
        }

        public void Reset()
        {
            currentWaypointIndex = 0;
        }
    }
}
