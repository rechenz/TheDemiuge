using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/*
注意角色正常跳跃高度为4，蹬墙跳可以达到10
*/

public class mainplayer : MonoBehaviour
{
    #region 参数定义
    [Header("基本参数")]
    public float walkspeed = 15f;
    public float runspeed = 20f;
    public float jumpforce = 60f;
    public bool isrun = false;
    public bool isgrounded = true;
    public float wallforce = 40f;
    //变量定义
    private Rigidbody2D rb;
    private Animator anim;
    private float currentspeed;
    private Vector2 move;
    private bool was_grounded = true;
    [SerializeField] private bool canwalljump = true;
    private CapsuleCollider2D capsuleCollider;
    [Header("=== 检测区域设置 ===")]
    [SerializeField][Tooltip("检测地面盒子的大小 (宽度, 高度)")] private Vector2 groundcheckSize = new Vector2(1.5f, 7.06f);
    [SerializeField][Tooltip("检测墙壁盒子的大小")] private Vector2 wallcheckSize = new Vector2(0.1f, 0.5f);
    [SerializeField][Tooltip("检测盒子的垂直偏移 (负值=向下, 正值=向上)")] private float checkOffset = -0.05f;
    [SerializeField][Tooltip("哪些层被认为是地面")] private LayerMask groundLayer;
    [SerializeField][Tooltip("是否在左墙壁上")] private bool isOnLeftWall = false;
    [SerializeField][Tooltip("是否在右墙壁上")] private bool isOnRightWall = false;
    [SerializeField][Tooltip("检测盒子的水平偏移")] private float checkOffsetX = 0.02f;
    #endregion
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        isrun = false;
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    private void move_left_or_right()
    {
        move.x = rb.velocity.x;
        move.y = rb.velocity.y;
        if (!was_grounded && isgrounded)
        {
            move.x = 0;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && isgrounded)
        {
            isrun = !isrun;
        }
        if (isrun)
        {
            currentspeed = runspeed;
        }
        else
        {
            currentspeed = walkspeed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            move.x = -currentspeed;
            move.y = rb.velocity.y;
        }
        if (Input.GetKeyUp(KeyCode.A) && isgrounded)
        {
            move.x = 0;
            move.y = rb.velocity.y;
        }
        else if (Input.GetKeyUp(KeyCode.A) && !isgrounded)
        {
            move.x = rb.velocity.x;
            move.y = rb.velocity.y;
        }
        if (Input.GetKey(KeyCode.D))
        {
            move.x = currentspeed;
            move.y = rb.velocity.y;
        }
        if (Input.GetKeyUp(KeyCode.D) && isgrounded)
        {
            move.x = 0;
            move.y = rb.velocity.y;
        }
        else if (Input.GetKeyUp(KeyCode.D) && !isgrounded)
        {
            move.x = rb.velocity.x;
            move.y = rb.velocity.y;
        }
        rb.velocity = move;
    }


    private void check_isgrounded()
    {
        Vector2 checkpos = (Vector2)capsuleCollider.bounds.center + Vector2.up * checkOffset;

        Collider2D[] colliders = Physics2D.OverlapBoxAll(checkpos, groundcheckSize, 0, groundLayer);

        if (colliders.Length > 0)
        {
            isgrounded = true;
            lastwall = null;
        }
        else
        {
            isgrounded = false;
            Debug.Log("未检测到地面");
        }
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.W) && isgrounded)
        {
            rb.AddForce(Vector2.up * jumpforce, ForceMode2D.Impulse);
            // isgrounded=false;
        }
    }

    private bool check_isonleftwall()
    {
        Vector2 checkpos = (Vector2)capsuleCollider.bounds.center + Vector2.left * checkOffsetX;

        Collider2D[] colliders = Physics2D.OverlapBoxAll(checkpos, wallcheckSize, 0, groundLayer);
        if (colliders.Length > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool check_isonrightwall()
    {
        Vector2 checkpos = (Vector2)capsuleCollider.bounds.center + Vector2.right * checkOffsetX;

        Collider2D[] colliders = Physics2D.OverlapBoxAll(checkpos, wallcheckSize, 0, groundLayer);
        if (colliders.Length > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool? lastwall = null;//右墙为true
    private void UpdateAnimator()
    {
        // 更新动画参数
        if (isgrounded && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            anim.SetBool("isstatic", true);
        }
        else
        {
            anim.SetBool("isstatic", false);
        }
        if (isgrounded)
        {
            lastwall = null;
            anim.SetBool("isonground", true);
        }
        else
        {
            anim.SetBool("isonground", false);
        }
        anim.SetFloat("deltaX", rb.velocity.x);
        anim.SetFloat("deltaY", rb.velocity.y);
        anim.SetBool("isrun", isrun);
        if (check_isonleftwall())
        {
            isOnLeftWall = true;
            anim.SetBool("isleftwall", true);
            if (lastwall == true)
            {
                canwalljump = true;
            }
            lastwall = false;
        }
        else
        {
            isOnLeftWall = false;
        }
        if (check_isonrightwall())
        {
            isOnRightWall = true;
            anim.SetBool("isrightwall", true);
            if (lastwall == false)
            {
                canwalljump = true;
            }
            lastwall = true;
        }
        else
        {
            isOnRightWall = false;
        }
    }

    private void UpdateWall()
    {
        if (!isgrounded)
        {
            if (isOnLeftWall && Input.GetKeyDown(KeyCode.W) && canwalljump)
            {
                rb.AddForce(Vector2.right * wallforce, ForceMode2D.Impulse);
                rb.AddForce(Vector2.up * jumpforce * 0.8f, ForceMode2D.Impulse);
                isOnLeftWall = false;
                canwalljump = false;
            }
            if (isOnRightWall && Input.GetKeyDown(KeyCode.W) && canwalljump)
            {
                rb.AddForce(Vector2.left * wallforce, ForceMode2D.Impulse);
                rb.AddForce(Vector2.up * jumpforce * 0.8f, ForceMode2D.Impulse);
                isOnRightWall = false;
                canwalljump = false;
            }
        }
    }

    private void initialize()
    {
        anim.SetBool("isrightwall", false);
        anim.SetBool("isleftwall", false);
    }

    // Update is called once per frame
    void Update()
    {
        initialize();
        was_grounded = isgrounded;
        check_isgrounded();
        if (was_grounded && !isgrounded)
        {
            canwalljump = true;
            // Debug.Log("离开地面");
        }
        //调用走路函数
        move_left_or_right();
        //跳跃
        Jump();
        UpdateAnimator();
        UpdateWall();
    }
}
