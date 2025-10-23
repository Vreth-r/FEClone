using System.Collections.Generic;
public abstract class BehaviourNode
{
    public enum State { Running, Success, Failure }
    protected State _state;
    public State CurrentState => _state;

    public abstract State Evaluate();
}

public class Sequence : BehaviourNode
{
    private readonly List<BehaviourNode> _children;
    public Sequence(List<BehaviourNode> children) => _children = children;

    public override State Evaluate()
    {
        bool anyRunning = false;
        foreach (var node in _children)
        {
            switch (node.Evaluate())
            {
                case State.Failure:
                    _state = State.Failure;
                    return _state;
                case State.Running:
                    anyRunning = true;
                    break;
            }
        }
        _state = anyRunning ? State.Running : State.Success;
        return _state;
    }
}

public class Selector : BehaviourNode
{
    private readonly List<BehaviourNode> _children;
    public Selector(List<BehaviourNode> children) => _children = children;

    public override State Evaluate()
    {
        foreach (var node in _children)
        {
            switch (node.Evaluate())
            {
                case State.Success:
                    _state = State.Success;
                    return _state;
                case State.Running:
                    _state = State.Running;
                    return _state;
            }
        }
        _state = State.Failure;
        return _state;
    }
}