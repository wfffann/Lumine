using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("�����ȡ")]
    public Rigidbody2D rb;
    public Animator anim;
    public SpriteRenderer sr;

    [Header("��������")]
    public float speed; //�ٶ�
    public float currentGravity; //��ҵ��ܵ�������
    public float fallMultiplier; //��ҵ�����ߵ�����������������ٶ�
    public float lowJumpMultiplier; //����ڵ�����ߵ�ǰ��ǰ�ſ���Ծʱ��������ٶ�
    public float jumpForce; //��Ծʩ����
    public float jumpTime;
    public bool gameOver = false;

    [Header("����")]
    public float rollForce; //����ʩ����
    //public float rollDeceleration; // �������ٶ�
    public float rollFrictionCoefficient;//����Ħ��ϵ��
    public float rotationSpeed;//��ת�ٶ�
    //public float rollTime;// ����ʱ��
    public float rollDistance; // �����ľ���

    [Header("����ƽ̨����")]
    public Vector2 platformSpeed;
    public bool plusPlatformSpeed = false;

    [Header("�����ǽ����")]
    public Transform lightPos;
    public Transform groundCheck;
    public Transform wallCheck;
    public Transform grabCheck;
    public float wallCheckRadius;
    public float groundCheckRadius;//���뾶
    public float grabCheckRadius;
    public LayerMask groundLayer; //�����ͼ��

    [Header("״̬���")]
    public bool isDead = false;
    public bool isGround;
    public bool isWall;
    public bool isJump;
    public bool isRoll;
    public bool canGrab;
    public bool isGrab = false;
    public bool isBusy = false;
    //����
    public int facingDir= 1;
    protected bool facingRight = true;

    [Header("��������")]
    public bool isKnocked = false; //�����˴���

    #region ����״̬
    public PlayerStateMachine stateMachine { get; private set; }
    //����״̬
    public PlayerGroundState groundState { get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerRollState rollState { get; private set; }
    //����״̬
    public PlayerJumpState jumpState { get; private set; }
    public PlayerAirState airState { get; private set; }
    //ǽ�ϵ�״̬
    public PlayerGrabState grabState { get; private set; }
    public PlayerClimbState climbState { get; private set; }
    #endregion

    private void Awake()
    {
        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        rollState = new PlayerRollState(this, stateMachine, "Roll");
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
        stateMachine.currentState.FixedUpdate();

        PhysicsCheck();
    }

    /// <summary>
    /// ʵ�ʼ����û�нӴ�ǽ��͵���
    /// </summary>
    private void PhysicsCheck()
    {
        ////��Բ������Ƿ�Ӵ����˵���
        isGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        isWall = Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, groundLayer);
        canGrab = !Physics2D.OverlapCircle(grabCheck.position, grabCheckRadius, groundLayer);
    }

    /// <summary>
    /// �����λ��ĳЩ״̬ʱ���������л�״̬
    /// </summary>
    /// <param name="_seconds"></param>
    /// <returns></returns>
    public IEnumerable BusyFor(float _seconds)
    {
        isBusy = true;

        yield return new WaitForSeconds(_seconds);

        isBusy = false;
    }

    #region �����ٶ�
    /// <summary>
    /// ʹ��ҵ��ٶ�Ϊ0
    /// </summary>
    public void SetZeroVelocity()
    {
        if (isKnocked)
            return;

        rb.velocity = new Vector2(0, 0) + new Vector2(platformSpeed.x, 0);
    }

    /// <summary>
    /// ������ҵ��ٶ�
    /// </summary>
    /// <param name="_xVelocity"></param>
    /// <param name="_yVelocity"></param>
    public void SetVelocity(float _xVelocity, float _yVelocity)
    {
        rb.velocity = new Vector2(_xVelocity, _yVelocity)+ new Vector2(platformSpeed.x, 0);

        FlipController(_xVelocity);
    }
    #endregion

    #region ��ɫ��ת
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

    #region ����
    public void AnimationTrigger() => stateMachine.currentState.AnimationFinishTrigger(); //�ж϶����Ƿ񲥷����
    #endregion

    /// <summary>
    /// ���Ƶ����ǽ�����
    /// </summary>
    public void OnDrawGizmos()
    {
        //��Բ������Ƿ�Ӵ����˵����������ǽ��
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.DrawWireSphere(wallCheck.position, wallCheckRadius);
    }
}
