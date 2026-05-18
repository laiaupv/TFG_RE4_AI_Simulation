using System.Collections.Generic;

public class SelectorNode : Node
{
    private List<Node> _children = new List<Node>();

    public SelectorNode(List<Node> children)
    {
        _children = children;
    }

    public override NodeState Evaluate()
    {
        foreach (Node child in _children)
        {
            NodeState result = child.Evaluate();
            if (result == NodeState.Success) return NodeState.Success;
            if (result == NodeState.Running) return NodeState.Running;
        }
        return NodeState.Failure;
    }
}