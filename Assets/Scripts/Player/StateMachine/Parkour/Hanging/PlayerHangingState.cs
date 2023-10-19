using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�Ŵ޸� ���¿��� �ٸ� Ledge ����
public class PlayerHangingState : PlayerParkourState
{
    protected Neighbour neighbour;

    public PlayerHangingState(Player player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }
    public override void Update()
    {
        base.Update();

        neighbour = player.currentPoint.GetNeighbour(player._inputXZ);

        Debug.Log(neighbour);

        if (neighbour != null)
        {
            if (neighbour.connectionType == ConnectionType.Jump)
            {
                player.currentPoint = neighbour.climbpoint;

            }

            if (neighbour.connectionType == ConnectionType.Move)
            {

            }
        }

    }
    public override void Exit()
    {
        base.Exit();
    }


}
