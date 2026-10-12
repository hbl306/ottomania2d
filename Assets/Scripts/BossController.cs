using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    // =========================================================
    // MOVEMENT
    // =========================================================

    [Header("Movement")]
    [SerializeField] private Transform movePointA;
    [SerializeField] private Transform movePointB;
    [SerializeField] private float moveSpeed = 2f;


    // =========================================================
    // PLAYER
    // =========================================================

    [Header("Player")]
    [SerializeField] private Transform player;


    // =========================================================
    // FACING
    // =========================================================

    [Header("Facing")]
    [SerializeField] private SpriteRenderer bossSprite;

    [Tooltip("Bật nếu sprite gốc nhìn sang phải")]
    [SerializeField] private bool spriteFacesRight = true;


    // =========================================================
    // PROJECTILE
    // =========================================================

    [Header("Projectile")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootPoint;


    // =========================================================
    // LASER
    // =========================================================

    [Header("Laser")]
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private Transform lazerPoint;

    [SerializeField] private float laserAngleOffset = 0f;

    [Tooltip("Độ cao điểm Laser ngắm trên Player")]
    [SerializeField] private float laserTargetYOffset = 0.5f;


    // =========================================================
    // TIMING
    // =========================================================

    [Header("Timing")]
    [SerializeField] private float idleBeforeAttack = 0.5f;
    [SerializeField] private float shotWaitTime = 1f;
    [SerializeField] private float laserWaitTime = 1.5f;
    [SerializeField] private float waitAfterAttack = 0.5f;


    // =========================================================
    // COLLIDER
    // =========================================================

    [Header("Collision")]
    [SerializeField] private Collider2D bossCollider;


    // =========================================================
    // PRIVATE
    // =========================================================

    private Animator animator;

    private Transform currentTarget;

    private bool isDead = false;
    private bool bulletSpawned = false;

    private Vector3 originalShootPointPosition;
    private Vector3 originalLazerPointPosition;

    private Coroutine bossRoutine;


    // =========================================================
    // UNITY
    // =========================================================

    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (bossSprite == null)
        {
            bossSprite =
                GetComponent<SpriteRenderer>();
        }

        if (bossCollider == null)
        {
            bossCollider =
                GetComponent<Collider2D>();
        }

        if (shootPoint != null)
        {
            originalShootPointPosition =
                shootPoint.localPosition;
        }

        if (lazerPoint != null)
        {
            originalLazerPointPosition =
                lazerPoint.localPosition;
        }
    }


    // Quan trọng:
    // Map6 bật lên -> Boss bắt đầu hoạt động
    private void OnEnable()
    {
        if (isDead)
            return;
            AudioManager.Instance?.PlayBossMusic();

        FindPlayer();

        currentTarget =
            movePointB;

        FacePlayer();

        if (bossRoutine != null)
        {
            StopCoroutine(
                bossRoutine
            );
        }

        bossRoutine =
            StartCoroutine(
                BossRoutine()
            );
    }


    // Map6 bị tắt -> Boss ngừng hoạt động
    private void OnDisable()
    {
        if (bossRoutine != null)
        {
            StopCoroutine(
                bossRoutine
            );

            bossRoutine = null;
        }
    }


    private void FindPlayer()
    {
        if (player != null)
            return;

        GameObject playerObject =
            GameObject.FindGameObjectWithTag(
                "Player"
            );

        if (playerObject != null)
        {
            player =
                playerObject.transform;
        }
    }


    // =========================================================
    // MAIN AI
    // =========================================================

    private IEnumerator BossRoutine()
    {
        while (!isDead)
        {
            // Di chuyển
            yield return StartCoroutine(
                MoveToTarget()
            );

            if (isDead)
                yield break;


            // Nghỉ
            yield return new WaitForSeconds(
                idleBeforeAttack
            );

            if (isDead)
                yield break;


            // Random:
            // 0 = projectile
            // 1 = laser
            int attackType =
                Random.Range(
                    0,
                    5
                );


            if (attackType <4)
            {
                yield return StartCoroutine(
                    ShotAttack()
                );
            }
            else
            {
                yield return StartCoroutine(
                    LaserAttack()
                );
            }


            if (isDead)
                yield break;


            yield return new WaitForSeconds(
                waitAfterAttack
            );


            ChangeMoveTarget();
        }
    }


    // =========================================================
    // MOVEMENT
    // =========================================================

    private IEnumerator MoveToTarget()
    {
        if (currentTarget == null)
            yield break;


        while (
            Vector2.Distance(
                transform.position,
                currentTarget.position
            ) > 0.05f
        )
        {
            if (isDead)
                yield break;


            float directionX =
                currentTarget.position.x -
                transform.position.x;


            FaceDirection(
                directionX
            );


            transform.position =
                Vector2.MoveTowards(
                    transform.position,
                    currentTarget.position,
                    moveSpeed *
                    Time.deltaTime
                );


            yield return null;
        }


        transform.position =
            currentTarget.position;
    }


    private void ChangeMoveTarget()
    {
        if (currentTarget == movePointA)
        {
            currentTarget =
                movePointB;
        }
        else
        {
            currentTarget =
                movePointA;
        }
    }


    // =========================================================
    // FACING
    // =========================================================

    private void FacePlayer()
    {
        if (player == null)
            return;


        float directionX =
            player.position.x -
            transform.position.x;


        FaceDirection(
            directionX
        );
    }


    private void FaceDirection(
        float directionX
    )
    {
        if (Mathf.Abs(directionX) < 0.01f)
            return;


        bool wantFaceRight =
            directionX > 0f;


        // Flip Sprite
        if (bossSprite != null)
        {
            if (spriteFacesRight)
            {
                bossSprite.flipX =
                    !wantFaceRight;
            }
            else
            {
                bossSprite.flipX =
                    wantFaceRight;
            }
        }


        // Có cần mirror ShootPoint/LazerPoint không
        bool shouldMirror =
            wantFaceRight !=
            spriteFacesRight;


        // ShootPoint
        if (shootPoint != null)
        {
            Vector3 position =
                originalShootPointPosition;

            if (shouldMirror)
            {
                position.x =
                    -originalShootPointPosition.x;
            }

            shootPoint.localPosition =
                position;
        }


        // LazerPoint
        if (lazerPoint != null)
        {
            Vector3 position =
                originalLazerPointPosition;

            if (shouldMirror)
            {
                position.x =
                    -originalLazerPointPosition.x;
            }

            lazerPoint.localPosition =
                position;
        }
    }


    // =========================================================
    // PROJECTILE ATTACK
    // =========================================================

    private IEnumerator ShotAttack()
    {
        if (isDead)
            yield break;


        FacePlayer();


        // Mỗi animation Shot chỉ được sinh 1 viên
        bulletSpawned =
            false;


        animator.SetTrigger(
            "Shot"
        );


        // SpawnBullet() được Animation Event gọi
        yield return new WaitForSeconds(
            shotWaitTime
        );
    }


    // Animation Event của BossShot
    public void SpawnBullet()
    {
        if (isDead)
            return;


        if (bulletSpawned)
            return;


        if (
            projectilePrefab == null ||
            shootPoint == null
        )
            return;


        bulletSpawned =
            true;
            AudioManager.Instance?
        .PlayProjectileFire();


        GameObject bullet =
            Instantiate(
                projectilePrefab,
                shootPoint.position,
                Quaternion.identity
            );


        BossProjectile projectile =
            bullet.GetComponent<BossProjectile>();


        if (projectile != null)
        {
            projectile.Initialize(
                player,
                transform
            );
        }
    }


    // =========================================================
    // LASER ATTACK
    // =========================================================

    private IEnumerator LaserAttack()
    {
        if (isDead)
            yield break;


        // Quay về phía Player trước khi charge
        FacePlayer();
        AudioManager.Instance?
        .PlayLaserCharge();


        animator.SetTrigger(
            "Laser"
        );


        // SpawnLaser() được Animation Event gọi
        yield return new WaitForSeconds(
            laserWaitTime
        );
    }


    // Animation Event của BossLazerShot
    public void SpawnLaser()
    {
        if (isDead)
            return;


        if (
            laserPrefab == null ||
            lazerPoint == null ||
            player == null
        )
            return;


        // =====================================================
        // KHÓA VỊ TRÍ PLAYER NGAY LÚC BẮN
        // =====================================================

        Vector2 targetPosition =
            (Vector2)player.position +
            Vector2.up *
            laserTargetYOffset;


        Vector2 direction =
            targetPosition -
            (Vector2)lazerPoint.position;


        if (direction.sqrMagnitude <= 0.001f)
            return;


        direction.Normalize();


        // =====================================================
        // TÍNH GÓC LASER
        // =====================================================

        float angle =
            Mathf.Atan2(
                direction.y,
                direction.x
            ) *
            Mathf.Rad2Deg;


        angle +=
            laserAngleOffset;


        // =====================================================
        // SPAWN
        //
        // Sau khi spawn:
        // Laser KHÔNG tracking Player nữa.
        // =====================================================
        AudioManager.Instance?
    .PlayLaserFire();

        Instantiate(
            laserPrefab,
            lazerPoint.position,
            Quaternion.Euler(
                0f,
                0f,
                angle
            )
        );
    }


    // =========================================================
    // PLAYER CHẠM BOSS
    // =========================================================

    private void OnCollisionEnter2D(
        Collision2D collision
    )
    {
        if (isDead)
            return;


        PlayerController playerController =
            collision.collider
                .GetComponentInParent<
                    PlayerController
                >();


        if (playerController != null)
        {
            playerController.Die();
        }
    }


    // =========================================================
    // DEATH
    // =========================================================

    public void Die()
    {
        if (isDead)
            return;


        isDead =
            true;


        // Dừng AI
        StopAllCoroutines();

        bossRoutine =
            null;


        // Không cho attack tiếp
        animator.ResetTrigger(
            "Shot"
        );

        animator.ResetTrigger(
            "Laser"
        );


        // =====================================================
        // TẮT COLLIDER BOSS
        // =====================================================

        if (bossCollider != null)
        {
            bossCollider.enabled =
                false;
        }


        // =====================================================
        // DEATH ANIMATION
        // =====================================================

        animator.SetBool(
            "Dead",
            true
        );
         animator.Play(
        "BossDeath",
        0,
        0f
        );
    }
}