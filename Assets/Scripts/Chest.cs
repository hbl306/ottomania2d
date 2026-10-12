using System.Collections;
using UnityEngine;

public class Chest : MonoBehaviour
{
    // =========================================
    // CHEST
    // =========================================

    [Header("CHEST SPRITES")]
    [SerializeField] private Sprite closedSprite;
    [SerializeField] private Sprite openedSprite;


    // =========================================
    // ITEM
    // =========================================

    [Header("ITEM")]
    [SerializeField] private GameObject itemPrefab;

    [SerializeField]
    private Transform itemSpawnPoint;


    [Header("ITEM REVEAL")]

    // Item bay lên bao nhiêu
    [SerializeField]
    private float itemRiseHeight = 1.2f;

    // Item bay lên trong bao lâu
    [SerializeField]
    private float itemRiseDuration = 1f;


    // =========================================
    // COMPONENT
    // =========================================

    private SpriteRenderer spriteRenderer;

    private GameFlowManager gameFlowManager;


    // =========================================
    // STATE
    // =========================================

    private bool isOpened = false;

    private GameObject spawnedItem;


    // =========================================
    // UNITY
    // =========================================

    private void Awake()
    {
        spriteRenderer =
            GetComponent<SpriteRenderer>();

        gameFlowManager =
            FindAnyObjectByType<GameFlowManager>();


        if (closedSprite != null)
        {
            spriteRenderer.sprite =
                closedSprite;
        }
    }


    // =========================================
    // OPEN CHEST
    // =========================================

    public void OpenChest()
    {
        // Rương chỉ mở 1 lần
        if (isOpened)
            return;


        isOpened = true;


        // Đổi sprite sang rương mở
        if (openedSprite != null)
        {
            spriteRenderer.sprite =
                openedSprite;
        }


        // Nhờ GameFlowManager chạy
        // cinematic camera + item
        if (gameFlowManager != null)
        {
            gameFlowManager.PlayChestReveal(
                this
            );
        }
        else
        {
            // Nếu không tìm thấy GameFlowManager
            // vẫn cho item xuất hiện
            StartCoroutine(
                RevealItem()
            );
        }
    }


    // =========================================
    // CAMERA FOCUS POINT
    // =========================================

    public Transform GetFocusPoint()
    {
        // Camera ưu tiên nhìn vào ItemSpawnPoint
        if (itemSpawnPoint != null)
        {
            return itemSpawnPoint;
        }

        return transform;
    }


    // =========================================
    // REVEAL ITEM
    // =========================================

    public IEnumerator RevealItem()
{
    if (itemPrefab == null ||
        itemSpawnPoint == null)
    {
        yield break;
    }

    // Không sinh lần 2
    if (spawnedItem != null)
        yield break;


    // =====================================
    // TẠO ITEM
    // =====================================

    spawnedItem = Instantiate(
        itemPrefab,
        itemSpawnPoint.position,
        Quaternion.identity,
        transform
    );


    // =====================================
    // TẮT COLLIDER TRONG LÚC ITEM ĐANG BAY
    // =====================================

    Collider2D[] itemColliders =
        spawnedItem.GetComponentsInChildren<Collider2D>();

    foreach (Collider2D col in itemColliders)
    {
        col.enabled = false;
    }


    Vector3 startPosition =
        itemSpawnPoint.position;

    Vector3 targetPosition =
        startPosition +
        Vector3.up * itemRiseHeight;


    // =====================================
    // LẤY SPRITE RENDERER
    // =====================================

    SpriteRenderer[] renderers =
        spawnedItem.GetComponentsInChildren<SpriteRenderer>();

    Color[] originalColors =
        new Color[renderers.Length];


    for (int i = 0; i < renderers.Length; i++)
    {
        originalColors[i] =
            renderers[i].color;

        Color color =
            renderers[i].color;

        // Ban đầu trong suốt
        color.a = 0f;

        renderers[i].color =
            color;
    }


    // =====================================
    // ITEM BAY LÊN + HIỆN DẦN
    // =====================================

    float timer = 0f;

    while (timer < itemRiseDuration)
    {
        // Nếu vì lý do nào đó item đã bị Destroy
        if (spawnedItem == null)
        {
            yield break;
        }

        timer += Time.deltaTime;

        float t =
            Mathf.Clamp01(
                timer / itemRiseDuration
            );

        float smoothT =
            Mathf.SmoothStep(
                0f,
                1f,
                t
            );


        // Bay lên
        spawnedItem.transform.position =
            Vector3.Lerp(
                startPosition,
                targetPosition,
                smoothT
            );


        // Hiện dần
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] == null)
                continue;

            Color color =
                originalColors[i];

            color.a =
                Mathf.Lerp(
                    0f,
                    originalColors[i].a,
                    smoothT
                );

            renderers[i].color =
                color;
        }


        yield return null;
    }


    // =====================================
    // ĐẢM BẢO VỊ TRÍ CUỐI
    // =====================================

    if (spawnedItem == null)
        yield break;


    spawnedItem.transform.position =
        targetPosition;


    // Item rõ hoàn toàn
    for (int i = 0; i < renderers.Length; i++)
    {
        if (renderers[i] != null)
        {
            renderers[i].color =
                originalColors[i];
        }
    }


    // =====================================
    // BÂY GIỜ MỚI CHO PHÉP NHẶT
    // =====================================

    foreach (Collider2D col in itemColliders)
    {
        if (col != null)
        {
            col.enabled = true;
        }
    }
}
}