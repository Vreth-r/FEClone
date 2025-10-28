using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Threading;

[Serializable]
public abstract class BehaviourNode
{
    public enum State { Running, Success, Failure }
    public State CurrentState { get; protected set; } = State.Running;

    public abstract UniTask<State> RunAsync(CancellationToken token = default);
}

[Serializable]
public class Sequence : BehaviourNode
{
    private readonly List<BehaviourNode> _children;

    public Sequence(List<BehaviourNode> children)
    {
        _children = children;
    }

    public override async UniTask<State> RunAsync(CancellationToken token = default)
    {
        foreach (var child in _children)
        {
            var result = await child.RunAsync(token);
            if (result == State.Failure)
            {
                CurrentState = State.Failure;
                return CurrentState;
            }
        }

        CurrentState = State.Success;
        return CurrentState;
    }
}

[Serializable]
public class Selector : BehaviourNode
{
    private readonly List<BehaviourNode> _children;

    public Selector(List<BehaviourNode> children)
    {
        _children = children;
    }

    public override async UniTask<State> RunAsync(CancellationToken token = default)
    {
        foreach (var child in _children)
        {
            var result = await child.RunAsync(token);
            if (result == State.Success)
            {
                CurrentState = State.Success;
                return CurrentState;
            }
        }

        CurrentState = State.Failure;
        return CurrentState;
    }
}