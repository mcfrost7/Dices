using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM 
{
    private FsmState StateCurrent { get; set; }

    private Dictionary<Type, FsmState> _states = new Dictionary<Type, FsmState>();

    public void AddState(FsmState fsmState)
    {
        _states.Add(fsmState.GetType(), fsmState);
    }

    public void SetState<T>() where T: FsmState
    {
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

    public void Update()
    {
        StateCurrent?.Update();
    }
}
