using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Player : Character
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float speed = 200;
    [SerializeField] private float jumpForce = 250;
    [SerializeField] private Kunai kunaiPrefab;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private GameObject attackArea;
    
    private bool isGrounded = true;
    private bool isJumping = false;
    private bool isAttack = false;

    private float horizontal;

    private int coins = 0;
    private Vector3 savePoint;

    private void Awake()
    {
        coins = PlayerPrefs.GetInt("coin", 0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (IsDead)
        {
            return;
        }


        isGrounded = CheckGrounded();

        //horizontal = Input.GetAxisRaw("Horizontal");

        if (isAttack)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        if (isGrounded)
        {
            if (isJumping)
            {
                return;
            }

            //change anim run
            if (Mathf.Abs(horizontal) > 0.1f)
            {
                ChangeAnim("run");
            }

            //jump
            if (Input.GetKeyDown(KeyCode.X) && isGrounded)
            {
                Jump();
            }

        }


        //check falling
        if (!isGrounded && rb.velocity.y < 0)
        {
            ChangeAnim("fall");
            isJumping = false;
        }


        //Run
        if (Mathf.Abs(horizontal) > 0.1f)
        {
            rb.velocity = new Vector2 (horizontal * speed, rb.velocity.y);
            transform.rotation = Quaternion.Euler(new Vector3(0, horizontal > 0 ? 0 : 180, 0));
        }

        // Idle
        else if (isGrounded)
        {
            rb.velocity = Vector2.zero;
            if (!isJumping)
            {
                ChangeAnim("idle");
            }
        }

        // Attack
        if (Input.GetKeyDown(KeyCode.C) && isGrounded)
        {
            Attack();
        }

        //Throw
        if (Input.GetKeyDown(KeyCode.V) && isGrounded)
        {
            Throw();
        }

    }

    public override void OnInit()
    {
        base.OnInit();
        isGrounded = true;
        isJumping = false;
        isAttack = false;
        transform.position = savePoint;
        ChangeAnim("idle");
        DeactiveAttack();

        UIManager.instance.SetCoin(coins);
    }

    public override void OnDespawn()
    {
        base.OnDespawn();
        OnInit();
    }
    protected override void OnDeath()
    {
        base.OnDeath();
    }

    private bool CheckGrounded()
    {
        Debug.DrawLine(transform.position, transform.position + Vector3.down * 1.1f, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, groundLayer);
        return hit.collider != null;
    }

    public void Attack()
    {
        ChangeAnim("attack");
        isAttack = true;
        Invoke(nameof(ResetAttack), 0.5f);

        ActiveAttack();
        Invoke(nameof(DeactiveAttack), 0.5f);
    }

    public void Throw()
    {
        ChangeAnim("throw");
        isAttack = true;
        Invoke(nameof(ResetAttack), 0.5f);

        Instantiate(kunaiPrefab, throwPoint.position, throwPoint.rotation);
    }

    public void Jump()
    {
        isJumping = true;
        rb.AddForce(Vector2.up * jumpForce);
        ChangeAnim("jump");

    }
    
    private void ResetAttack()
    {
        isAttack = false;
        ChangeAnim("idle");
    }


    internal void SavePoint()
    {
        savePoint = transform.position;
    }

    private void ActiveAttack()
    {
        attackArea.SetActive(true);
    }

    private void DeactiveAttack()
    {
        attackArea.SetActive(false);
    }

    public void SetMove(float horizontal)
    {
        this.horizontal = horizontal;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Coin")
        {
            coins++;
            PlayerPrefs.SetInt("coin", coins);
            UIManager.instance.SetCoin(coins);
            Destroy(collision.gameObject);
        }

        if(collision.tag == "DeadZone")
        {
            ChangeAnim("die");
            Invoke(nameof(OnInit), 1.0f);
        }
    }
}
