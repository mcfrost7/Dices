using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FsmState 
{
    protected readonly FSM Fsm;

    protected FsmState(FSM fsm)
    {
        Fsm = fsm;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }
}
