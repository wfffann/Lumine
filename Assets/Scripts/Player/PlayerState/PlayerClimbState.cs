using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClimbState : PlayerState
{
    public PlayerClimbState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        rb.AddForce(new Vector2(0, 10f), ForceMode2D.Impulse);
        player.isGrab = true;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (!player.isWall && player.isGrab)
        {
            rb.gravityScale = 50;
            player.isGrab = false;
            rb.AddForce(new Vector2(3f * player.facingDir, 0f), ForceMode2D.Impulse);
        }
        if (player.isGround)
            stateMachine.ChangeState(player.idleState);
    }
}
