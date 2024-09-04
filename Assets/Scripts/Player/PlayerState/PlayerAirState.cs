using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DG.Tweening.DOTweenModuleUtils;

public class PlayerAirState : PlayerState
{

    public PlayerAirState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();

        rb.gravityScale = player.currentGravity;
    }

    public override void Update()
    {
        base.Update();

        //落到地面切换为静止状态
        if (player.isGround)
            stateMachine.ChangeState(player.idleState);

        //空中的移动速度
        if (xInput != 0)
            player.SetVelocity(player.speed * xInput, rb.velocity.y);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        rb.velocity += Vector2.up * Physics2D.gravity.y * (player.fallMultiplier - player.currentGravity) * Time.deltaTime;
    }
}
