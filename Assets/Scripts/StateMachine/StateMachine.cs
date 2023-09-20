using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine
{
    protected IState currentState;
    public void ChangeState(IState newState)
    {
        //null 아닐때만 메소드 실행
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }
    public void Update()
    {
        currentState?.Update();
    }

    public void FixedUpdate()
    {
        currentState?.FixedUpdate();
    }

    public void HandleInput()
    {
        currentState?.HandleInput();
    }
}
