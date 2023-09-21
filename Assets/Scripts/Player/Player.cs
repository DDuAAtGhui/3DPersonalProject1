using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    public bool isBusy { get; private set; }
    #region states
    public PlayerStateMachine stateMachine { get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerWalkState walkState { get; private set; }
    public PlayerRunState runState { get; private set; }
    protected override void Awake()
    {
        base.Awake();

        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        walkState = new PlayerWalkState(this, stateMachine, "Walk");
        runState = new PlayerRunState(this, stateMachine, "Run");
    }
    #endregion
    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
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
