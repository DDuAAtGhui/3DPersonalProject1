using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    public bool isBusy { get; private set; }
    #region states
    public PlayerStateMachine stateMachine { get; private set; }
    protected override void Awake()
    {
        base.Awake();

        stateMachine = new PlayerStateMachine();

    }
    #endregion
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        stateMachine.currentState.FixedUpdate();
    }
}
