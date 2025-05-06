using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM 
{
    private FsmState StateCurrent { get; set; }

    private Dictionary<Type, FsmState> _states = new Dictionary<Type, FsmState>();
    public bool IsRunning { get; private set; } = true;
    public void AddState(FsmState fsmState)
    {

        _states.Add(fsmState.GetType(), fsmState);
    }

    public void SetState<T>() where T: FsmState
    {
        if (!IsRunning) return;
        var type = typeof(T);

        if (StateCurrent != null && StateCurrent.GetType() == type)
        {
            return;
        }

        if (_states.TryGetValue(type, out var newState))
        {
            StateCurrent?.Exit();

            StateCurrent = newState;

            StateCurrent.Enter();
        }

    }
    public void StopMachine()
    {
        IsRunning = false;
        StateCurrent?.Exit();
        StateCurrent = null;
        _states.Clear();
    }
    public void Update()
    {
        if (IsRunning)
        {
            StateCurrent?.Update();
        }
    }
}
