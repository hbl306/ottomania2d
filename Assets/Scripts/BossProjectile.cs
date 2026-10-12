using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 6f;
    [SerializeField] private float reflectedSpeed = 8f;
    [SerializeField] private float lifeTime = 6f;

    [Header("Damage")]
    [SerializeField] private int playerDamage = 1;
    [SerializeField] private int bossDamage = 1;

    [Header("Sprites")]
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite reflectedSprite;

    [Header("Collision")]
    [SerializeField] private LayerMask groundLayer;

    private SpriteRenderer spriteRenderer;

    private Transform player;
    private Transform boss;

    private Vector2 direction;

    private bool isReflected = false;

    public bool IsReflected => isReflected;

    private void Awake()
    {
        spriteRenderer =
            GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        Destroy(
            gameObject,
            lifeTime
        );
    }

    // =========================================================
    // INITIALIZE
    // =========================================================

    public void Initialize(
        Transform playerTarget,
        Transform bossTarget
    )
    {
        player = playerTarget;
        boss = bossTarget;

        // Bắn về vị trí Player
        if (player != null)
        {
            direction =
                (
                    player.position -
                    transform.position
                ).normalized;
        }
        else
        {
            direction =
                Vector2.left;
        }

        if (
            spriteRenderer != null &&
            normalSprite != null
        )
        {
            spriteRenderer.sprite =
                normalSprite;
        }
    }

    // =========================================================
    // MOVEMENT
    // =========================================================

    private void Update()
    {
        float currentSpeed =
            isReflected
                ? reflectedSpeed
                : speed;

        transform.position +=
            (Vector3)(
                direction *
                currentSpeed *
                Time.deltaTime
            );
    }

    // =========================================================
    // REFLECT
    // =========================================================

    public void Reflect()
    {
        // Một viên đạn chỉ phản được 1 lần
        if (isReflected)
            return;

        isReflected = true;
         AudioManager.Instance?
        .PlayProjectileReflect();

        // =============================================
        // BAY VỀ BOSS
        // =============================================

        if (boss != null)
        {
            direction =
                (
                    boss.position -
                    transform.position
                ).normalized;
        }
        else
        {
            // Không có Boss thì đảo hướng
            direction =
                -direction;
        }

        // =============================================
        // ĐỔI SPRITE
        // =============================================

        if (
            spriteRenderer != null &&
            reflectedSprite != null
        )
        {
            spriteRenderer.sprite =
                reflectedSprite;
        }
    }

    // =========================================================
    // COLLISION
    // =========================================================

    private void OnTriggerEnter2D(
        Collider2D other
    )
    {
        // =============================================
        // ĐẠN BÌNH THƯỜNG TRÚNG PLAYER
        // =============================================

        if (!isReflected)
        {
            PlayerController playerController =
                other.GetComponentInParent<PlayerController>();

            if (playerController != null)
            {
                playerController.TakeDamage(
                    playerDamage
                );

                Destroy(gameObject);

                return;
            }
        }

        // =============================================
        // ĐẠN ĐÃ PHẢN TRÚNG BOSS
        // =============================================

        if (isReflected)
        {
            BossHealth bossHealth =
                other.GetComponentInParent<BossHealth>();

            if (bossHealth != null)
            {
                AudioManager.Instance?
            .PlayReflectedHit();
                bossHealth.TakeDamage(
                    bossDamage
                );

                Destroy(gameObject);

                return;
            }
        }

        // =============================================
        // CHẠM GROUND / WALL
        // =============================================

        if (
            ((1 << other.gameObject.layer)
            & groundLayer) != 0
        )
        {
            Destroy(gameObject);

            return;
        }
    }
}