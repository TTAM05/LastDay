using UnityEngine;
using System.Collections.Generic;

public class Selector : Node
{
    private List<Node> _children;

    public Selector(List<Node> children)
    {
        this._children = children;
    }

    public override NodeState Evaluate()
    {
        foreach (Node child in _children)
        {
            NodeState state = child.Evaluate();

            if (state == NodeState.Success)
                return NodeState.Success;

            if (state == NodeState.Running)
                return NodeState.Running;
        }

        return NodeState.Failure;
    }
}