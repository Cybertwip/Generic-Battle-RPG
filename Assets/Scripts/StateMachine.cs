using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    void Update(float dt);
    void HandleInput();
    void Enter(params object[] args);
    void Exit();
}

public class EmptyState : IState //keeps us from having to check for null state if intitial state exists
{
    public void Update(float dt) { }
    public void HandleInput() { }
    public void Enter(params object[] args) { }
    public void Exit() { }
}
public class StateMachine
{
    Dictionary<string, IState> _stateDict = new Dictionary<string, IState>();
    IState _currentState = new EmptyState();

    public IState CurrentState { get { return _currentState; } }
    public void Add(string id, IState state) { _stateDict.Add(id, state); }
    public void Remove(string id) { _stateDict.Remove(id); }
    public void Clear() { _stateDict.Clear(); }

    public void Change(string id, params object[] args)
    {
        _currentState.Exit();
        IState next = _stateDict[id];
        next.Enter(args);
        _currentState = next; // won't work if we define _currentState (line 15) to be readOnly
    }

    public void Update(float dt)
    {
        _currentState.Update(dt);
    }

    public void HandleInput()
    {
        _currentState.HandleInput();
    }
}
