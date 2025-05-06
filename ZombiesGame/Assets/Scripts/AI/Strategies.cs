using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Pathfinding.BehaviorTrees
{
    public interface IStrategy
    {
        Node.Status Process();
        void Reset()
        {
            // Default implementation does nothing
        }
    }

    public class  ActionStrategy : IStrategy
    {
        readonly Action doSomething;

        public ActionStrategy(Action doSomething)
        {
            this.doSomething = doSomething;
        }

        public Node.Status Process()
        {
            doSomething();
            return Node.Status.Success;
        }
    }

    public class Condition : IStrategy
    {
        readonly Func<bool> predicate;

        public Condition(Func<bool> predicate)
        {
            this.predicate = predicate;
        }
        public Node.Status Process() => predicate() ? Node.Status.Success : Node.Status.Failure;
    }

    public class PatrolStrategy : IStrategy
    {
        readonly Transform entity;
        readonly NavMeshAgent agent;
        readonly List<Transform> waypoints;
        readonly float patrolSpeed;
        int currentWaypointIndex;
        bool isPathCalculated;
        bool hasInitialized = false;

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
            if (waypoints.Count == 0) return Node.Status.Failure;

            if (!hasInitialized)
            {
                Reset();
                hasInitialized = true;
                isPathCalculated = true;
            }

            var target = waypoints[currentWaypointIndex];
            agent.SetDestination(target.position);
            agent.speed = patrolSpeed;

            // Smooth horizontal rotation
            Vector3 direction = (target.position - entity.position);
            direction.y = 0f;
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                entity.rotation = Quaternion.Slerp(entity.rotation, lookRotation, Time.deltaTime * 5f);
            }

            if (isPathCalculated && agent.remainingDistance < agent.stoppingDistance)
            {
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
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
            hasInitialized = false;

            if (waypoints == null || waypoints.Count == 0) return;

            float closestDistance = float.MaxValue;
            int closestIndex = 0;

            for (int i = 0; i < waypoints.Count; i++)
            {
                float distance = Vector3.Distance(entity.position, waypoints[i].position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestIndex = i;
                }
            }

            currentWaypointIndex = closestIndex;
            isPathCalculated = false;
        }
    }

    public class ChaseStrategy : IStrategy
    {
        readonly Transform entity;
        readonly NavMeshAgent agent;
        readonly Transform target;
        readonly float chaseSpeed;
        bool isPathCalculated;

        float memoryDuration = 3f; // How long to remember the target
        float lastSeenTime = Mathf.NegativeInfinity;

        public ChaseStrategy(Transform entity, NavMeshAgent agent, Transform target, float chaseSpeed = 5f)
        {
            this.entity = entity;
            this.agent = agent;
            this.target = target;
            this.chaseSpeed = chaseSpeed;
        }

        public Node.Status Process()
        {
            if (target == null)
            {
                return Node.Status.Failure;
            }

            Vector3 toTarget = (target.position - entity.position).normalized;
            float distanceToTarget = Vector3.Distance(entity.position, target.position);
            float dot = Vector3.Dot(entity.forward, toTarget);
            float visionThreshold = 0.9f;
            float maxChaseDistance = 15f;

            bool hasVision = dot >= visionThreshold && distanceToTarget <= maxChaseDistance;

            if (hasVision)
            {
                Ray ray = new Ray(entity.position + Vector3.up * 1.5f, toTarget);
                RaycastHit hit;
                LayerMask obstacleMask = LayerMask.GetMask("Default");

                if (Physics.Raycast(ray, out hit, distanceToTarget, obstacleMask))
                {
                    if (hit.transform != target)
                    {
                        hasVision = false;
                    }
                }
            }

            if (hasVision)
            {
                lastSeenTime = Time.time; // update memory
            }

            if (Time.time - lastSeenTime > memoryDuration)
            {
                return Node.Status.Failure;
            }

            agent.SetDestination(target.position);
            agent.speed = chaseSpeed;
            Quaternion lookRotation = Quaternion.LookRotation(toTarget);
            entity.rotation = Quaternion.Slerp(entity.rotation, lookRotation, Time.deltaTime * 5f);

            if (isPathCalculated && agent.remainingDistance < agent.stoppingDistance)
            {
                isPathCalculated = false;
                return Node.Status.Success;
            }
            if (agent.pathPending)
            {
                isPathCalculated = true;
            }

            return Node.Status.Running;
        }

        public void Reset()
        {
            lastSeenTime = Mathf.NegativeInfinity;
            isPathCalculated = false;
        }
    }

    public class EscapeToSafetyStrategy : IStrategy
    {
        readonly Transform entity;
        readonly NavMeshAgent agent;
        readonly Transform target;
        readonly List<Transform> safepoints;
        readonly float escapeSpeed;
        bool isPathCalculated;

        float memoryDuration = 10f; // How long to remember the target
        float lastSeenTime = Mathf.NegativeInfinity;

        public EscapeToSafetyStrategy(Transform entity, NavMeshAgent agent, Transform target, List<Transform> safepoints, float escapeSpeed = 5f)
        {
            this.entity = entity;
            this.agent = agent;
            this.target = target;
            this.safepoints = safepoints;
            this.escapeSpeed = escapeSpeed;
        }
        public Node.Status Process()
        {
            if (target == null)
            {
                return Node.Status.Failure;
            }

            Vector3 toTarget = (target.position - entity.position).normalized;
            float distanceToTarget = Vector3.Distance(entity.position, target.position);
            float dot = Vector3.Dot(entity.forward, toTarget);
            float visionThreshold = 0.9f;
            float maxChaseDistance = 15f;

            bool hasVision = dot >= visionThreshold && distanceToTarget <= maxChaseDistance;

            if (hasVision)
            {
                Ray ray = new Ray(entity.position + Vector3.up * 1.5f, toTarget);
                RaycastHit hit;
                LayerMask obstacleMask = LayerMask.GetMask("Default");

                if (Physics.Raycast(ray, out hit, distanceToTarget, obstacleMask))
                {
                    if (hit.transform != target)
                    {
                        hasVision = false;
                    }
                }
            }

            if (hasVision)
            {
                lastSeenTime = Time.time; // update memory
            }

            if (Time.time - lastSeenTime > memoryDuration)
            {
                return Node.Status.Failure;
            }

            // Track the closest safe point that is not in the enemy's direction
            Transform bestSafePoint = null;
            float bestDistance = float.MaxValue;

            foreach (var safePoint in safepoints)
            {
                Vector3 toSafe = safePoint.position - entity.position;
                Vector3 toEnemy = target.position - entity.position;

                // Calculate the angle between the safe point and the enemy direction
                float angle = Vector3.Angle(toSafe, toEnemy);
                float distance = Vector3.Distance(entity.position, safePoint.position);

                // Ensure the safe point is more than 90 degrees away from the enemy's position
                if (angle > 90f && distance < bestDistance)
                {
                    bestSafePoint = safePoint;
                    bestDistance = distance;
                }
            }

            // If we found a safe point that isn't in the enemy's direction
            if (bestSafePoint != null)
            {
                agent.SetDestination(bestSafePoint.position);
            }
            else
            {
                // No valid safe point, just run away from the enemy
                Vector3 awayFromTarget = -toTarget;
                agent.SetDestination(entity.position + awayFromTarget * 10f); // Move away from the target

                // Optional: Make the entity face away from the enemy while fleeing
                Quaternion lookRotation = Quaternion.LookRotation(awayFromTarget);
                entity.rotation = Quaternion.Slerp(entity.rotation, lookRotation, Time.deltaTime * 5f);
            }

            // Check if the agent has reached the destination
            agent.speed = escapeSpeed;
            if (isPathCalculated && agent.remainingDistance < agent.stoppingDistance)
            {
                isPathCalculated = false;
                return Node.Status.Success;
            }

            if (agent.pathPending)
            {
                isPathCalculated = true;
            }

            return Node.Status.Running;
        }
        public void Reset()
        {
            isPathCalculated = false;
        }
    }
}
