using System.Collections.Generic;

public class SequenceNode : Node
{
    private List<Node> _children = new List<Node>();

    public SequenceNode(List<Node> children)
    {
        _children = children;
    }

    public override NodeState Evaluate()
    {
        foreach (Node child in _children)
        {
            NodeState result = child.Evaluate();
            if (result == NodeState.Failure) return NodeState.Failure;
            if (result == NodeState.Running) return NodeState.Running;
        }
        return NodeState.Success;
    }
}