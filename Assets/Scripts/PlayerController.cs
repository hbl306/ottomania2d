using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // =========================================
    // MOVEMENT
    // =========================================

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;


    // =========================================
    // LADDER
    // =========================================

    [Header("Ladder")]
    [SerializeField] private float climbSpeed = 4f;

    [Header("Attack")]
    [SerializeField] private CircleCollider2D attackHitbox;
    [SerializeField] private LayerMask attackableLayer;

    [SerializeField] private float attackHitDelay = 0.15f;

    


    // =========================================
    // COMPONENTS
    // =========================================

    private Rigidbody2D rb;
    private Animator animator;
    private GameFlowManager gameFlowManager;


    // =========================================
    // STATE
    // =========================================

    private bool isGrounded;
    private bool isDead = false;
    private bool controlsLocked = false;

    private bool isOnLadder = false;
    private bool isClimbing = false;

    private float defaultGravityScale;


    // =========================================
    // MOBILE INPUT
    // =========================================

    private bool leftPressed = false;
    private bool rightPressed = false;
    private bool jumpPressed = false;

    // Nút chung Attack / Climb
    private bool actionPressed = false;

    // Chỉ true ở thời điểm vừa bấm xuống
    private bool actionPressedDown = false;


    // =========================================
    // UNITY
    // =========================================

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        gameFlowManager =
            FindAnyObjectByType<GameFlowManager>();

        defaultGravityScale =
            rb.gravityScale;
    }


    private void Update()
    {
        // Không cho điều khiển lúc chết
        // hoặc đang chuyển map
        if (isDead || controlsLocked)
            return;


        if (gameFlowManager != null &&
            gameFlowManager.IsGameOver())
            return;


        // =====================================
        // LADDER ƯU TIÊN TRƯỚC
        // =====================================

        if (HandleLadder())
        {
            UpdateAnimation();

            return;
        }


        // Nếu không leo thì bật lại gravity
        RestoreGravity();


        HandleMovement();

        HandleJump();

        HandleAttack();

        UpdateAnimation();
    }


    // =========================================
    // MOVEMENT
    // =========================================

    private void HandleMovement()
    {
        float keyboardInput =
            Input.GetAxis("Horizontal");


        float mobileInput = 0f;


        if (leftPressed)
        {
            mobileInput -= 1f;
        }


        if (rightPressed)
        {
            mobileInput += 1f;
        }


        float moveInput =
            mobileInput != 0f
            ? mobileInput
            : keyboardInput;


        rb.linearVelocity =
            new Vector2(
                moveInput * moveSpeed,
                rb.linearVelocity.y
            );


        // Quay mặt Player
        if (moveInput > 0)
        {
            transform.localScale =
                new Vector3(
                    1,
                    1,
                    1
                );
        }
        else if (moveInput < 0)
        {
            transform.localScale =
                new Vector3(
                    -1,
                    1,
                    1
                );
        }
    }


    // =========================================
    // JUMP
    // =========================================

    private void HandleJump()
    {
        isGrounded =
            Physics2D.OverlapCircle(
                groundCheck.position,
                0.2f,
                groundLayer
            );


        bool keyboardJump =
            Input.GetButtonDown("Jump");


        if ((keyboardJump ||
             jumpPressed) &&
            isGrounded)
        {
            rb.linearVelocity =
                new Vector2(
                    rb.linearVelocity.x,
                    jumpForce
                );
                AudioManager.Instance?.PlayJump();
        }


        jumpPressed = false;
    }


    // =========================================
    // LADDER
    // =========================================

    private bool HandleLadder()
    {
        // PC giữ F
        bool keyboardClimb =
            Input.GetKey(KeyCode.F);


        // Mobile giữ nút Action
        bool wantsToClimb =
            isOnLadder &&
            (
                keyboardClimb ||
                actionPressed
            );


        if (!wantsToClimb)
        {
            isClimbing = false;

            return false;
        }


        isClimbing = true;


        // Nếu vừa bấm mobile khi đang ở Ladder
        // thì không dùng lần nhấn này để Attack
        actionPressedDown = false;


        // Tắt trọng lực
        rb.gravityScale = 0f;


        // Leo thẳng lên
        rb.linearVelocity =
            new Vector2(
                0f,
                climbSpeed
            );


        return true;
    }


    private void RestoreGravity()
    {
        if (rb.gravityScale !=
            defaultGravityScale)
        {
            rb.gravityScale =
                defaultGravityScale;
        }


        isClimbing = false;
    }


    // =========================================
    // ATTACK
    // =========================================

    private void HandleAttack()
    {
        /*
         * F / Action Button:
         *
         * Chạm Ladder:
         *      giữ = leo
         *
         * Không chạm Ladder:
         *      nhấn = Attack
         */


        // Nếu đang trong Ladder
        // thì Action dành cho leo
        if (isOnLadder)
        {
            actionPressedDown = false;

            return;
        }


        bool keyboardAttack =
            Input.GetKeyDown(KeyCode.F);


        if (keyboardAttack ||
            actionPressedDown)
        {
            Attack();
        }


        // Attack chỉ chạy 1 lần / lần nhấn
        actionPressedDown = false;
    }


    private void Attack()
    {
         if (isDead || controlsLocked)
        return;

    animator.SetTrigger("Attack");
    AudioManager.Instance?.PlayAttack();

    StartCoroutine(
        AttackHitRoutine()
    );
    }
private System.Collections.IEnumerator AttackHitRoutine()
{
    // Chờ animation Attack tới thời điểm va chạm
    yield return new WaitForSeconds(
        attackHitDelay
    );

    DoAttackHit();
}


private void DoAttackHit()
{
    if (attackHitbox == null)
        return;

    // Tâm vùng đánh
    Vector2 center =
        attackHitbox.transform.TransformPoint(
            attackHitbox.offset
        );

    // Radius đúng kể cả khi Player bị flip
    Vector3 scale =
        attackHitbox.transform.lossyScale;

    float radius =
        attackHitbox.radius *
        Mathf.Max(
            Mathf.Abs(scale.x),
            Mathf.Abs(scale.y)
        );

    // Lấy tất cả object nằm trong vùng attack
    Collider2D[] hits =
        Physics2D.OverlapCircleAll(
            center,
            radius,
            attackableLayer
        );

    foreach (Collider2D hit in hits)
    {
        // =========================================
        // PHẢN ĐẠN BOSS
        // =========================================

        BossProjectile projectile =
            hit.GetComponentInParent<BossProjectile>();

        if (projectile != null)
        {
            projectile.Reflect();

            // Projectile đã được xử lý,
            // không cần xử lý Chest nữa.
            continue;
        }

        // =========================================
        // CHEST
        // =========================================

        Chest chest =
            hit.GetComponentInParent<Chest>();

        if (chest != null)
        {
            chest.OpenChest();
            AudioManager.Instance?
    .PlayChestOpen();
    AudioManager.Instance?
    .PlayItemReveal();
        }
    }
}
    // =========================================
    // ANIMATION
    // =========================================

    private void UpdateAnimation()
    {
        // Đang leo thì không chạy
        // Run / Jump animation
        if (isClimbing)
        {
            animator.SetBool(
                "isRunning",
                false
            );

            animator.SetBool(
                "isJumping",
                false
            );

            return;
        }


        bool isRunning =
            Mathf.Abs(
                rb.linearVelocity.x
            ) > 0.1f;


        bool isJumping =
            !isGrounded;


        animator.SetBool(
            "isRunning",
            isRunning
        );


        animator.SetBool(
            "isJumping",
            isJumping
        );
    }


    // =========================================
    // LADDER COLLISION
    // =========================================

    private void OnTriggerEnter2D(
        Collider2D collision
    )
    {
        if (collision.CompareTag("Ladder"))
        {
            isOnLadder = true;
        }
    }


    private void OnTriggerExit2D(
        Collider2D collision
    )
    {
        if (collision.CompareTag("Ladder"))
        {
            isOnLadder = false;

            isClimbing = false;

            actionPressed = false;

            actionPressedDown = false;

            RestoreGravity();
        }
    }
public void TakeDamage(int damage)
{
    if (isDead || controlsLocked)
        return;

    if (damage <= 0)
        return;

    // Hiện tại mỗi lần Boss đánh trúng
    // sẽ làm Player mất 1 mạng / 1 tim.
    Die();
}

    // =========================================
    // DEATH
    // =========================================

    public void Die()
    {
        if (isDead)
            return;


        if (gameFlowManager != null &&
            gameFlowManager.IsGameOver())
            return;


        isDead = true;


        // Reset Input
        leftPressed = false;
        rightPressed = false;
        jumpPressed = false;

        actionPressed = false;
        actionPressedDown = false;


        // Reset Ladder
        isOnLadder = false;
        isClimbing = false;


        // Gravity trở lại bình thường
        rb.gravityScale =
            defaultGravityScale;


        rb.linearVelocity =
            Vector2.zero;


        // Animation
        animator.SetBool(
            "isRunning",
            false
        );


        animator.SetBool(
            "isJumping",
            false
        );


        animator.ResetTrigger(
            "Attack"
        );


        animator.SetTrigger(
            "Death"
        );


        // Ngừng physics khi chết
        rb.simulated = false;


        if (gameFlowManager != null)
        {
            gameFlowManager.PlayerDied();
        }
    }


    // =========================================
    // RESPAWN
    // =========================================

    public void Respawn(
        Vector3 respawnPosition
    )
    {
        rb.simulated = false;


        transform.position =
            respawnPosition;


        rb.position =
            respawnPosition;


        rb.linearVelocity =
            Vector2.zero;


        rb.gravityScale =
            defaultGravityScale;


        // Reset Ladder
        isOnLadder = false;
        isClimbing = false;


        // Reset Mobile Action
        actionPressed = false;
        actionPressedDown = false;


        Physics2D.SyncTransforms();


        // Reset Animator
        animator.ResetTrigger(
            "Death"
        );


        animator.ResetTrigger(
            "Attack"
        );


        animator.SetBool(
            "isRunning",
            false
        );


        animator.SetBool(
            "isJumping",
            false
        );


        isDead = false;


        animator.Play(
            "OttoIdle",
            0,
            0f
        );


        animator.Update(0f);
        AudioManager.Instance?
    .PlayPlayerRespawn();


        rb.simulated = true;
    }


    // =========================================
    // CONTROL LOCK
    // =========================================

public void SetControlsLocked(bool locked)
{
    controlsLocked = locked;

    if (locked)
    {
        // Reset input
        leftPressed = false;
        rightPressed = false;
        jumpPressed = false;

        actionPressed = false;
        actionPressedDown = false;

        // Dừng leo
        isClimbing = false;

        // Trọng lực bình thường
        rb.gravityScale = defaultGravityScale;

        // Dừng Player
        rb.linearVelocity = Vector2.zero;

        // Tắt trạng thái di chuyển
        animator.SetBool("isRunning", false);
        animator.SetBool("isJumping", false);
    }
    else
    {
        // =====================================
        // KHI MỞ KHÓA SAU CINEMATIC
        // RESET ANIMATION VỀ TRẠNG THÁI SẠCH
        // =====================================

        animator.ResetTrigger("Attack");

        animator.SetBool("isRunning", false);
        animator.SetBool("isJumping", false);

        // Thoát khỏi OttoSlap nếu Animator còn mắc ở đó
        animator.Play(
            "OttoIdle",
            0,
            0f
        );

        animator.Update(0f);

        rb.linearVelocity = Vector2.zero;
    }
}

    // =========================================
    // MOBILE MOVEMENT
    // =========================================

    public void LeftButtonDown()
    {
        leftPressed = true;
    }


    public void LeftButtonUp()
    {
        leftPressed = false;
    }


    public void RightButtonDown()
    {
        rightPressed = true;
    }


    public void RightButtonUp()
    {
        rightPressed = false;
    }


    public void JumpButtonDown()
    {
        jumpPressed = true;
    }


    // =========================================
    // MOBILE ACTION BUTTON
    // Attack + Climb dùng chung
    // =========================================

    public void ActionButtonDown()
    {
        actionPressed = true;

        actionPressedDown = true;
    }


    public void ActionButtonUp()
    {
        actionPressed = false;
    }
}