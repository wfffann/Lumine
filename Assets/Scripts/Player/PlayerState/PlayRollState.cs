using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRollState : PlayerGroundState
{
    float rollSpeed;
    Vector3 initialRotation; // 记录开始时的旋转角度
    Vector3 initialPosition;//记录开始的位置
    float distanceRolled = 0f;

    public PlayerRollState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        initialPosition = player.transform.position;//记录开始的位置
        initialRotation = player.transform.eulerAngles; // 记录开始时的旋转角度
        player.isRoll = true;
        rollSpeed=player.facingDir * player.rollForce* player.rollFrictionCoefficient;
        player.SetVelocity(rollSpeed, rb.velocity.y);
        player.StartCoroutine(RollControl());

    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
        if(player.isWall)
        {
            endRoll();
        }
    }

    IEnumerator RollControl()
    {
         distanceRolled = 0f;
         initialPosition = player.transform.position;

        while (player.isRoll)//rb.velocity.magnitude < 1f)
        {
            float rotationAngle =player.facingDir * player.rotationSpeed;
            player.transform.Rotate(0, 0, rotationAngle); // 应用旋转
            // 计算已经翻滚的距离
            distanceRolled = Mathf.Abs(initialPosition.x-player.transform.position.x);//Vector3.Distance(initialPosition, player.transform.position);
        if(distanceRolled > player.rollDistance)
        {Debug.Log("distanceRolled="+distanceRolled);
            endRoll();
        }
            yield return null;
        }

        
    }

    private void endRoll()
    {
        // 翻滚结束时的逻辑
        rb.velocity = Vector3.zero;
        player.transform.eulerAngles = initialRotation;
        //player.isRoll = false;
        stateMachine.ChangeState(player.idleState);
    }

}
