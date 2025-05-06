using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;

namespace Pathfinding.BehaviorTrees
{
    public class BehaviorTree : Node
    {
        public BehaviorTree(string name) : base(name) { }
        public override Status Process()
        {
            while (currentChild < children.Count)
            {
                var status = children[currentChild].Process();
                if (status != Status.Success)
                {
                    return status;
                }
                currentChild++;
            }
            return Status.Success;
        }
    }

    public class PrioritySelector : Selector
    {
        List<Node> sortedChildren;
        List<Node> SortedChildren => sortedChildren ??= SortChildren();

        protected virtual List<Node> SortChildren()
        {
            return children.OrderByDescending(child => child.priority).ToList();
        }

        public PrioritySelector(string name) : base(name) { }

        public override void Reset()
        {
            base.Reset();
            sortedChildren = null;
        }

        public override Status Process()
        {
            foreach (var child in SortedChildren)
            {
                switch (child.Process())
                {
                    case Status.Running:
                        return Status.Running;
                    case Status.Success:
                        return Status.Success;
                    default:
                        continue;
                }
            }

            return Status.Failure;
        }
    }

    public class Selector : Node
    {
        public Selector(string name, int priority = 0) : base(name, priority) { }
        public override Status Process()
        {
            if (currentChild < children.Count)
            {
                switch (children[currentChild].Process())
                {
                    case Status.Running:
                        return Status.Running;
                    case Status.Success:
                        Reset();
                        return Status.Success;
                    default:
                        currentChild++;
                        return Status.Running;
                }
            }
            Reset();
            return Status.Failure;
        }
    }

    public class Sequence : Node
    {
        public Sequence(string name, int priority = 0) : base(name, priority) { }
        public override Status Process()
        {
            if (currentChild < children.Count)
            {
                switch (children[currentChild].Process()) {
                    case Status.Running:
                        return Status.Running;
                    case Status.Failure:
                        Reset();
                        return Status.Failure;
                    default:
                        currentChild++;
                        return currentChild == children.Count ? Status.Success : Status.Running;
                }
            }

            Reset();
            return Status.Success;
        }
    }

    public class Leaf : Node
    {
        readonly IStrategy strategy;

        public Leaf(string name, IStrategy strategy, int priority = 0) : base(name, priority)
        {
            this.strategy = strategy;
        }

        public override Status Process() => strategy.Process();

        public override void Reset() => strategy.Reset();
    }

    public class Node
    {
        public enum Status
        {
            Success,
            Failure,
            Running
        }

        public readonly string name;
        public readonly int priority;
        public readonly List<Node> children = new();

        protected int currentChild;
        private bool hasEntered = false;

        public Node(string name = "Node", int priority = 0)
        {
            this.name = name;
            this.priority = priority;
        }

        public void AddChild(Node child) => children.Add(child);

        public virtual Status Process()
        {
            if (!hasEntered)
            {
                Enter();
                hasEntered = true;
            }

            return children.Count > 0 ? children[currentChild].Process() : Status.Failure;
        }

        protected virtual void Enter()
        {
            // Called once when the node is entered
            foreach (var child in children)
            {
                child.Reset(); // Optional: Reset all children on entry
            }
        }

        public virtual Node GetNode(string searchName)
        {
            if (this.name == searchName)
                return this;

            foreach (var child in children)
            {
                Node result = child.GetNode(searchName);
                if (result != null)
                    return result;
            }

            return null;
        }

        public virtual void Reset()
        {
            hasEntered = false;
            currentChild = 0;

            foreach (var child in children)
            {
                child.Reset();
            }
        }
    }
}

