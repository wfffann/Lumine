using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerGroundState
{
    public PlayerIdleState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        rb.gravityScale = 4;
        player.isJump = false;
        player.isRoll = false;
        player.SetZeroVelocity();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base .Update();

        //贴着墙往墙移动时状态不变
        if (xInput == player.facingDir && player.isWall)
            return;

        //正常移动
        if (xInput != 0 && !player.isBusy)
            stateMachine.ChangeState(player.moveState);

        //if (player.isWall && !player.isGround)
        //    stateMachine.ChangeState(player.wallSlide);
            
    }
}
