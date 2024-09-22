using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovePlatform : MonoBehaviour
{
    [Header("组件获取")]
    public Transform upPoint;
    public Transform downPoint;
    public Transform leftPoint;
    public Transform rightPoint;
    public Transform playerFather;

    [Header("平台设置")]
    public PlatformState currentState;
    public float speed;
    public Vector2 currentSpeed;
    public float waitTime;
    public PlatformMoveDir currentDir;
    public float waitTimer;
    private bool isWaiting = false;

    private void Awake()
    {
        //rb = GetComponent<Rigidbody2D>();     
    }

    private void Start()
    {
        //给出初始移动方向,并锁定另外一个方向的移动
        if (currentState == PlatformState.horizontalMove)
        {
            currentDir = PlatformMoveDir.Right;
        }
        else if (currentState == PlatformState.verticalMove)
        {
            currentDir = PlatformMoveDir.Up;
        }
    }

    private void FixedUpdate()
    {
        if (currentState == PlatformState.noMove)
            return;

        CheckMoveDir();
        Move();
    }

    /// <summary>
    /// 检查平台目前状态
    /// </summary>
    private void CheckMoveDir()
    {
        if (isWaiting)
            currentDir = PlatformMoveDir.No;

        #region 水平移动
        if (currentState == PlatformState.horizontalMove)
        {
            if(currentDir == PlatformMoveDir.Right && Mathf.Abs(transform.position.x - rightPoint.position.x) < 0.1f && !isWaiting)
            {
                isWaiting = true;
                waitTimer = Time.time + waitTime;
            }
            else if(currentDir == PlatformMoveDir.Left && Mathf.Abs(transform.position.x - leftPoint.position.x) < 0.1f && !isWaiting)
            {
                isWaiting = true;
                waitTimer = Time.time + waitTime;
            }

            if (Time.time > waitTimer && isWaiting)
            {
                isWaiting = false;
                if(Mathf.Abs(transform.position.x - rightPoint.position.x) > Mathf.Abs(transform.position.x - leftPoint.position.x))
                    currentDir = PlatformMoveDir.Right;
                else
                    currentDir = PlatformMoveDir.Left;
            }
        }
        #endregion

        #region 竖直移动
        if (currentState == PlatformState.verticalMove)
        {
            if (currentDir == PlatformMoveDir.Up && Mathf.Abs(transform.position.y - upPoint.position.y) < 0.1f && !isWaiting)
            {
                isWaiting = true;
                waitTimer = Time.time + waitTime;
            }
            else if (currentDir == PlatformMoveDir.Down && Mathf.Abs(transform.position.y - downPoint.position.y) < 0.1f && !isWaiting)
            {
                isWaiting = true;
                waitTimer = Time.time + waitTime;
            }

            if (Time.time > waitTimer && isWaiting)
            {
                isWaiting = false;
                if (Mathf.Abs(transform.position.y - upPoint.position.y) > Mathf.Abs(transform.position.y - downPoint.position.y))
                    currentDir = PlatformMoveDir.Up;
                else
                    currentDir = PlatformMoveDir.Down;
            }
        }
        #endregion
    }

    /// <summary>
    /// 移动
    /// </summary>
    private void Move()
    {
        switch (currentDir)
        {
            case PlatformMoveDir.Up:
                transform.Translate(new Vector2(0, speed) * Time.deltaTime);
                currentSpeed.y = speed;
                break;
            case PlatformMoveDir.Down:
                transform.Translate(new Vector2(0, -speed) * Time.deltaTime);
                currentSpeed.y = -speed;
                break;
            case PlatformMoveDir.Left:
                transform.Translate(new Vector2(-speed, 0) * Time.deltaTime);
                currentSpeed.x = -speed;
                break;
            case PlatformMoveDir.Right:
                transform.Translate(new Vector2(speed, 0) * Time.deltaTime);
                currentSpeed.x = speed;
                break;
            case PlatformMoveDir.No:
                currentSpeed = Vector2.zero;
                break;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //if (collision.gameObject.GetComponent<Player>().platformSpeed != currentSpeed)
            //{
            //    collision.gameObject.GetComponent<Player>().platformSpeed = currentSpeed;
            //    collision.gameObject.GetComponent<Player>().SetZeroVelocity();
            //}

            playerFather = collision.gameObject.transform.parent;
            if(collision.gameObject.GetComponent<Player>().rb.velocity.x != 0)
                collision.gameObject.transform.SetParent(null);
            else
                collision.gameObject.transform.SetParent(transform);


        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //collision.gameObject.GetComponent<Player>().platformSpeed = Vector2.zero;
            //collision.gameObject.GetComponent<Player>().plusPlatformSpeed = false;
        }
    }
}
