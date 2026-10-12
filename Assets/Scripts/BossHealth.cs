using System.Collections;
using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHP = 5;

    [Header("Hit Flash")]
    [SerializeField] private SpriteRenderer bossSprite;
    [SerializeField] private Color hitColor = Color.red;
    [SerializeField] private float hitFlashTime = 0.12f;

    private int currentHP;
    private bool isDead = false;

    private BossController bossController;
    private Color originalColor;
    private Coroutine hitFlashCoroutine;

    public int CurrentHP => currentHP;
    public int MaxHP => maxHP;

    private void Awake()
    {
        bossController = GetComponent<BossController>();

        if (bossSprite == null)
        {
            bossSprite = GetComponent<SpriteRenderer>();
        }

        if (bossSprite != null)
        {
            originalColor = bossSprite.color;
        }
    }

    private void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        if (damage <= 0)
            return;

        currentHP -= damage;
        currentHP = Mathf.Max(currentHP, 0);

        Debug.Log("Boss HP: " + currentHP + "/" + maxHP);

        if (currentHP > 0)
        {
            PlayHitFlash();
            return;
        }

        Die();
    }

    private void PlayHitFlash()
    {
        if (bossSprite == null)
            return;

        if (hitFlashCoroutine != null)
        {
            StopCoroutine(hitFlashCoroutine);
        }

        hitFlashCoroutine = StartCoroutine(HitFlashRoutine());
    }

    private IEnumerator HitFlashRoutine()
    {
        bossSprite.color = hitColor;

        yield return new WaitForSeconds(hitFlashTime);

        bossSprite.color = originalColor;
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        if (bossSprite != null)
        {
            bossSprite.color = originalColor;
        }

        if (bossController != null)
        {
            bossController.Die();
        }
    }
}