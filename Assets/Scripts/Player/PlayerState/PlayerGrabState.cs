using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrabState : PlayerState
{
    public PlayerGrabState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.SetZeroVelocity();
        player.rb.gravityScale = 0;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Space))
            stateMachine.ChangeState(player.climbState);
    }
}
