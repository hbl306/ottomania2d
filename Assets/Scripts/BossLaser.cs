using UnityEngine;

public class BossLaser : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private int damage = 1;

    private BoxCollider2D laserCollider;

    private bool canDamage = false;

    private void Awake()
    {
        laserCollider =
            GetComponent<BoxCollider2D>();

        if (laserCollider != null)
        {
            laserCollider.enabled =
                false;
        }
    }

    // Animation Event
    public void EnableLaserHitbox()
    {
        canDamage = true;

        if (laserCollider != null)
        {
            laserCollider.enabled =
                true;
        }
    }

    // Animation Event
    public void DisableLaserHitbox()
    {
        canDamage = false;

        if (laserCollider != null)
        {
            laserCollider.enabled =
                false;
        }
    }

    private void OnTriggerEnter2D(
        Collider2D other
    )
    {
        if (!canDamage)
            return;

        PlayerController player =
            other.GetComponentInParent<PlayerController>();

        if (player == null)
            return;

        player.TakeDamage(
            damage
        );
    }

    // Animation Event cuối
    public void EndLaser()
    {
        Destroy(gameObject);
    }
}