using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerState
{

    public PlayerJumpState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        rb.gravityScale = player.currentGravity;
        player.isJump = true;
        rb.AddForce(new Vector2(0, player.jumpForce), ForceMode2D.Impulse);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        //if (Input.GetKeyUp(KeyCode.Space))
        //    rb.gravityScale = 8;

        if (!Input.GetButton("Jump"))
            rb.velocity += Vector2.up * Physics2D.gravity.y * (player.lowJumpMultiplier - player.currentGravity) * Time.deltaTime;

        if (rb.velocity.y < 0)
            stateMachine.ChangeState(player.airState);

        if (player.isJump && player.canGrab && player.isWall && !player.isGround)
            stateMachine.ChangeState(player.grabState);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (xInput != 0)
            rb.velocity = new Vector2(xInput * player.speed * 1.2f, rb.velocity.y);
    }
}
