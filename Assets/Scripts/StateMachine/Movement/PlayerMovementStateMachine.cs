using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementStateMachine : IState
{
    public void Enter()
    {
        Debug.Log("State : " + GetType().Name);
    }

    public void Exit()
    {
        throw new System.NotImplementedException();
    }
    public void HandleInput()
    {
        throw new System.NotImplementedException();
    }
    public void FixedUpdate()
    {
        throw new System.NotImplementedException();
    }

    public void Update()
    {
        throw new System.NotImplementedException();
    }
}
