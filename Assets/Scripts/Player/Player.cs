using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("组件获取")]
    public Rigidbody2D rb;
    public Animator anim;
    public SpriteRenderer sr;

    [Header("基本设置")]
    public float speed; //速度
    public float jumpForce; //跳跃施加力
    public float jumpTime;
    public bool gameOver = false;

    [Header("地面和墙面检测")]
    public Transform groundCheck;
    public Transform wallCheck;
    public Transform grabCheck;
    public float wallCheckRadius;
    public float groundCheckRadius;//检测半径
    public float grabCheckRadius;
    public LayerMask groundLayer; //地面的图层

    [Header("状态检测")]
    public bool isDead = false;
    public bool isGround;
    public bool isWall;
    public bool isJump;
    public bool canGrab;
    public bool isGrab = false;
    public bool isBusy = false;
    //朝向
    public int facingDir= 1;
    protected bool facingRight = true;

    [Header("攻击设置")]
    public bool isKnocked = false; //被敌人打中

    #region 所有状态
    public PlayerStateMachine stateMachine { get; private set; }
    //地面状态
    public PlayerGroundState groundState { get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    //空中状态
    public PlayerJumpState jumpState { get; private set; }
    public PlayerAirState airState { get; private set; }
    //墙上的状态
    public PlayerGrabState grabState { get; private set; }
    public PlayerClimbState climbState { get; private set; }
    #endregion

    private void Awake()
    {
        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState = new PlayerAirState(this, stateMachine, "Jump");
        grabState = new PlayerGrabState(this, stateMachine, "Grab");
        climbState = new PlayerClimbState(this, stateMachine, "Climb");
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        stateMachine.Initialize(idleState);
    }

    private void Update()
    {
        stateMachine.currentState.Update();
    }

    private void FixedUpdate()
    {
        PhysicsCheck();
    }

    /// <summary>
    /// 实际检测有没有接触墙面和地面
    /// </summary>
    private void PhysicsCheck()
    {
        ////画圆来检测是否接触到了地面
        isGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        isWall = Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, groundLayer);
        canGrab = !Physics2D.OverlapCircle(grabCheck.position, grabCheckRadius, groundLayer);
        if (isGround)
        {
            rb.gravityScale = 4;
        }
    }

    /// <summary>
    /// 当玩家位于某些状态时不能随意切换状态
    /// </summary>
    /// <param name="_seconds"></param>
    /// <returns></returns>
    public IEnumerable BusyFor(float _seconds)
    {
        isBusy = true;

        yield return new WaitForSeconds(_seconds);

        isBusy = false;
    }

    #region 设置速度
    /// <summary>
    /// 使玩家的速度为0
    /// </summary>
    public void SetZeroVelocity()
    {
        if (isKnocked)
            return;

        rb.velocity = new Vector2(0, 0);
    }

    /// <summary>
    /// 设置玩家的速度
    /// </summary>
    /// <param name="_xVelocity"></param>
    /// <param name="_yVelocity"></param>
    public void SetVelocity(float _xVelocity, float _yVelocity)
    {
        rb.velocity = new Vector2(_xVelocity, _yVelocity);
        FlipController(_xVelocity);
    }
    #endregion

    #region 角色翻转
    public virtual void Flip()
    {
        facingDir = facingDir * -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }

    public virtual void FlipController(float _x)
    {
        if (_x > 0 && !facingRight)
            Flip();
        else if (_x < 0 && facingRight)
            Flip();
    }
    #endregion

    #region 动画
    public void AnimationTrigger() => stateMachine.currentState.AnimationFinishTrigger(); //判断动画是否播放完毕
    #endregion

    /// <summary>
    /// 绘制地面和墙面检测点
    /// </summary>
    public void OnDrawGizmos()
    {
        //画圆来检测是否接触到了地面或者贴着墙面
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.DrawWireSphere(wallCheck.position, wallCheckRadius);
    }
}
