using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;

public class GameFlowManager : MonoBehaviour
{
    // =========================================
    // PLAYER
    // =========================================

    [Header("PLAYER")]
    [SerializeField] private PlayerController player;


    // =========================================
    // LIVES
    // =========================================

    [Header("LIVES")]
    [SerializeField] private int maxLives = 3;

    [SerializeField] private Image healthImage;

    [SerializeField] private Sprite threeHeartsSprite;
    [SerializeField] private Sprite twoHeartsSprite;
    [SerializeField] private Sprite oneHeartSprite;
    [SerializeField] private Sprite zeroHeartSprite;


    // =========================================
    // MAP
    // =========================================

    [Header("MAP")]
    [SerializeField] private MapInfo[] maps;

    [SerializeField]
    private Transform cinemachineCameraTransform;

    private int currentMapIndex = 0;


    // =========================================
    // CAMERA DEATH
    // =========================================

    [Header("DEATH CAMERA")]

    [SerializeField] private Camera mainCamera;

    [SerializeField]
    private CinemachineBrain cinemachineBrain;

    [SerializeField]
    private float deathZoomSize = 2.5f;

    [SerializeField]
    private float zoomDuration = 0.6f;

    [SerializeField]
    private float deathAnimationDuration = 2.5f;


    // =========================================
    // FADE
    // =========================================

    [Header("SCREEN FADE")]

    [SerializeField]
    private CanvasGroup fadeCanvasGroup;

    // Thời gian màn hình chuyển sang đen
    [SerializeField]
    private float fadeOutDuration = 0.4f;

    // Giữ màn hình đen bao lâu
    [SerializeField]
    private float blackHoldDuration = 0.15f;

    // Thời gian từ đen trở lại game
    [SerializeField]
    private float fadeInDuration = 0.4f;


    // =========================================
    // MANAGER
    // =========================================

    private GameManager gameManager;
    // =========================================
// CHEST CINEMATIC
// =========================================

[Header("CHEST CINEMATIC")]

[SerializeField]
private float chestZoomSize = 2.5f;

[SerializeField]
private float chestZoomDuration = 0.5f;

// Sau khi item xuất hiện xong,
// giữ camera thêm một chút
[SerializeField]
private float chestHoldDuration = 0.8f;

// Camera quay lại mất bao lâu
[SerializeField]
private float chestCameraReturnDuration = 0.5f;

private bool playingChestSequence = false;


    // =========================================
    // STATE
    // =========================================

    private int currentLives;

    private bool gameOver = false;

    private bool handlingDeath = false;

    private bool transitioning = false;


    // =========================================
    // UNITY
    // =========================================

    private void Awake()
    {
        if (player == null)
        {
            player =
                FindAnyObjectByType<PlayerController>();
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (cinemachineBrain == null &&
            mainCamera != null)
        {
            cinemachineBrain =
                mainCamera.GetComponent<CinemachineBrain>();
        }

        gameManager =
            FindAnyObjectByType<GameManager>();
    }


private void Start()
{
    // =====================================================
    // STATE BAN ĐẦU
    // =====================================================

    currentLives = maxLives;

    gameOver = false;
    handlingDeath = false;
    transitioning = false;


    // =====================================================
    // FADE BAN ĐẦU
    // =====================================================

    if (fadeCanvasGroup != null)
    {
        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.blocksRaycasts = false;
    }


    UpdateHeartUI();


    // =====================================================
    // KIỂM TRA CÓ PHẢI CONTINUE KHÔNG
    // =====================================================

    bool isContinue =
        SaveSystem.LoadFromContinue &&
        SaveSystem.HasSave();


    // =====================================================
    // XÁC ĐỊNH MAP
    // =====================================================

    if (
        isContinue &&
        maps != null &&
        maps.Length > 0
    )
    {
        currentMapIndex =
            Mathf.Clamp(
                SaveSystem.GetSavedMapIndex(),
                0,
                maps.Length - 1
            );

        Debug.Log(
            "CONTINUE | Load MapIndex = " +
            currentMapIndex
        );
    }
    else
    {
        // New Game
        currentMapIndex = 0;

        Debug.Log(
            "NEW GAME | MapIndex = 0"
        );
    }


    // =====================================================
    // BẬT ĐÚNG MAP
    // =====================================================

    ActivateCurrentMap();


    // =====================================================
    // CAMERA VỀ ĐÚNG MAP
    // =====================================================

    MoveCameraToCurrentMap();


    // =====================================================
    // PLAYER
    // =====================================================

    if (isContinue)
    {
        Vector3 savedPosition =
            SaveSystem.GetPlayerPosition();

        if (player != null)
        {
            // Dùng Respawn để reset trạng thái Player cho sạch
            player.Respawn(
                savedPosition
            );
        }

        Debug.Log(
            "CONTINUE | Player Position = " +
            savedPosition
        );

        // Đã load xong
        SaveSystem.LoadFromContinue =
            false;
    }
    else
    {
        // New Game -> SpawnPoint của Map1
        RespawnCurrentMap();
    }
}

    // =========================================
    // STATE
    // =========================================

    public bool IsGameOver()
    {
        return gameOver;
    }


    public bool IsHandlingDeath()
    {
        return handlingDeath;
    }


    public bool IsTransitioning()
    {
        return transitioning;
    }


    // =========================================
    // PLAYER DEATH
    // =========================================

    public void PlayerDied()
    {
        if (handlingDeath ||
            transitioning ||
            gameOver)
        {
            return;
        }

        StartCoroutine(
            DeathRoutine()
        );
    }


    private IEnumerator DeathRoutine()
    {
        handlingDeath = true;


        // =========================================
        // MẤT 1 MẠNG
        // =========================================

        currentLives--;

        currentLives =
            Mathf.Max(currentLives, 0);

        UpdateHeartUI();


        // =========================================
        // CAMERA TRƯỚC KHI CHẾT
        // =========================================

        Vector3 originalCameraPosition =
            mainCamera.transform.position;

        float originalCameraSize =
            mainCamera.orthographicSize;


        bool brainWasEnabled = false;


        if (cinemachineBrain != null)
        {
            brainWasEnabled =
                cinemachineBrain.enabled;

            cinemachineBrain.enabled = false;
        }


        // =========================================
        // TARGET CAMERA = PLAYER
        // =========================================

        Vector3 targetPosition =
            player.transform.position;

        targetPosition.z =
            originalCameraPosition.z;


        // =========================================
        // KHÔNG CHO CAMERA RA NGOÀI MAP
        // =========================================

        float originalHalfHeight =
            originalCameraSize;

        float originalHalfWidth =
            originalHalfHeight *
            mainCamera.aspect;


        float zoomHalfHeight =
            deathZoomSize;

        float zoomHalfWidth =
            zoomHalfHeight *
            mainCamera.aspect;


        float minX =
            originalCameraPosition.x
            - originalHalfWidth
            + zoomHalfWidth;

        float maxX =
            originalCameraPosition.x
            + originalHalfWidth
            - zoomHalfWidth;


        float minY =
            originalCameraPosition.y
            - originalHalfHeight
            + zoomHalfHeight;

        float maxY =
            originalCameraPosition.y
            + originalHalfHeight
            - zoomHalfHeight;


        targetPosition.x =
            Mathf.Clamp(
                targetPosition.x,
                minX,
                maxX
            );

        targetPosition.y =
            Mathf.Clamp(
                targetPosition.y,
                minY,
                maxY
            );


        // =========================================
        // ZOOM VÀO PLAYER
        // =========================================

        float timer = 0f;


        while (timer < zoomDuration)
        {
            timer += Time.deltaTime;


            float t =
                Mathf.Clamp01(
                    timer /
                    zoomDuration
                );


            mainCamera.transform.position =
                Vector3.Lerp(
                    originalCameraPosition,
                    targetPosition,
                    t
                );


            mainCamera.orthographicSize =
                Mathf.Lerp(
                    originalCameraSize,
                    deathZoomSize,
                    t
                );


            yield return null;
        }


        mainCamera.transform.position =
            targetPosition;

        mainCamera.orthographicSize =
            deathZoomSize;


        // =========================================
        // CHỜ ANIMATION DEATH
        // =========================================

        float remainingDeathTime =
            Mathf.Max(
                0,
                deathAnimationDuration
                - zoomDuration
            );


        yield return new WaitForSeconds(
            remainingDeathTime
        );


        // =========================================
        // HẾT MẠNG
        // =========================================

        if (currentLives <= 0)
        {
            gameOver = true;

            handlingDeath = false;


            if (cinemachineBrain != null)
            {
                cinemachineBrain.enabled =
                    brainWasEnabled;
            }


            if (gameManager != null)
            {
                gameManager.GameOver();
            }


            yield break;
        }


        // =========================================
        // FADE ĐEN TRƯỚC RESPAWN
        // =========================================

        yield return StartCoroutine(
            FadeTo(
                1f,
                fadeOutDuration
            )
        );


        if (blackHoldDuration > 0)
        {
            yield return
                new WaitForSecondsRealtime(
                    blackHoldDuration
                );
        }


        // =========================================
        // RESPAWN TRONG MÀN HÌNH ĐEN
        // =========================================

        RespawnCurrentMap();


        // Camera quay về vị trí trước khi chết
        mainCamera.transform.position =
            originalCameraPosition;

        mainCamera.orthographicSize =
            originalCameraSize;


        if (cinemachineBrain != null)
        {
            cinemachineBrain.enabled =
                brainWasEnabled;
        }


        // Chờ camera update 1 frame
        yield return null;


        // =========================================
        // FADE SÁNG LẠI
        // =========================================

        yield return StartCoroutine(
            FadeTo(
                0f,
                fadeInDuration
            )
        );


        handlingDeath = false;
    }


    // =========================================
    // NEXT MAP
    // =========================================

    public void GoToNextMap()
    {
        if (transitioning ||
            handlingDeath ||
            gameOver)
        {
            return;
        }


        int nextMapIndex =
            currentMapIndex + 1;


        // Đã hết Map6
        if (nextMapIndex >= maps.Length)
        {
            Debug.Log(
                "ĐÃ HOÀN THÀNH TẤT CẢ MAP!"
            );

            return;
        }


        StartCoroutine(
            ChangeMapRoutine(
                nextMapIndex
            )
        );
    }


    private IEnumerator ChangeMapRoutine(
        int nextMapIndex
    )
    {
        transitioning = true;


        // Khóa điều khiển
        if (player != null)
        {
            player.SetControlsLocked(true);
        }


        // =========================================
        // FADE ĐEN
        // =========================================

        yield return StartCoroutine(
            FadeTo(
                1f,
                fadeOutDuration
            )
        );


        if (blackHoldDuration > 0)
        {
            yield return
                new WaitForSecondsRealtime(
                    blackHoldDuration
                );
        }


        // =========================================
        // ĐỔI MAP TRONG MÀN HÌNH ĐEN
        // =========================================

        currentMapIndex =
            nextMapIndex;


        // Tắt map cũ / bật map mới
        ActivateCurrentMap();


        // Camera → CameraPoint map mới
        MoveCameraToCurrentMap();


        // Player → RespawnPoint map mới
        RespawnCurrentMap();


        // Đợi camera update
        yield return null;


        // =========================================
        // FADE SÁNG
        // =========================================

        yield return StartCoroutine(
            FadeTo(
                0f,
                fadeInDuration
            )
        );


        if (player != null)
        {
            player.SetControlsLocked(false);
        }


        transitioning = false;
    }


    // =========================================
    // FADE
    // =========================================

    private IEnumerator FadeTo(
        float targetAlpha,
        float duration
    )
    {
        if (fadeCanvasGroup == null)
        {
            yield break;
        }


        float startAlpha =
            fadeCanvasGroup.alpha;


        // Trong lúc fade khóa click UI
        fadeCanvasGroup.blocksRaycasts = true;


        if (duration <= 0f)
        {
            fadeCanvasGroup.alpha =
                targetAlpha;


            if (targetAlpha <= 0f)
            {
                fadeCanvasGroup.blocksRaycasts =
                    false;
            }


            yield break;
        }


        float timer = 0f;


        while (timer < duration)
        {
            timer +=
                Time.unscaledDeltaTime;


            float t =
                Mathf.Clamp01(
                    timer /
                    duration
                );


            fadeCanvasGroup.alpha =
                Mathf.Lerp(
                    startAlpha,
                    targetAlpha,
                    t
                );


            yield return null;
        }


        fadeCanvasGroup.alpha =
            targetAlpha;


        // Khi fade xong và đã trong suốt
        if (targetAlpha <= 0f)
        {
            fadeCanvasGroup.blocksRaycasts =
                false;
        }
    }


    // =========================================
    // HEART UI
    // =========================================

    private void UpdateHeartUI()
    {
        if (healthImage == null)
            return;


        switch (currentLives)
        {
            case 3:

                healthImage.sprite =
                    threeHeartsSprite;

                break;


            case 2:

                healthImage.sprite =
                    twoHeartsSprite;

                break;


            case 1:

                healthImage.sprite =
                    oneHeartSprite;

                break;


            default:

                healthImage.sprite =
                    zeroHeartSprite;

                break;
        }
    }


    // =========================================
    // MAP ACTIVE
    // =========================================

    private void ActivateCurrentMap()
    {
        if (maps == null)
            return;


        for (int i = 0;
             i < maps.Length;
             i++)
        {
            if (maps[i] != null)
            {
                maps[i].gameObject.SetActive(
                    i == currentMapIndex
                );
            }
        }
    }


    // =========================================
    // CAMERA MAP
    // =========================================

    private void MoveCameraToCurrentMap()
    {
        if (maps == null ||
            currentMapIndex < 0 ||
            currentMapIndex >= maps.Length)
        {
            return;
        }


        MapInfo currentMap =
            maps[currentMapIndex];


        if (currentMap == null ||
            currentMap.CameraPoint == null)
        {
            return;
        }


        Vector3 cameraPoint =
            currentMap.CameraPoint.position;


        // =========================================
        // CINEMACHINE CAMERA
        // =========================================

        if (cinemachineCameraTransform != null)
        {
            Vector3 newPosition =
                cinemachineCameraTransform.position;


            newPosition.x =
                cameraPoint.x;

            newPosition.y =
                cameraPoint.y;


            cinemachineCameraTransform.position =
                newPosition;
        }


        // =========================================
        // MAIN CAMERA
        // =========================================

        if (mainCamera != null)
        {
            Vector3 mainPosition =
                mainCamera.transform.position;


            mainPosition.x =
                cameraPoint.x;

            mainPosition.y =
                cameraPoint.y;


            mainCamera.transform.position =
                mainPosition;
        }
    }


    // =========================================
    // RESPAWN
    // =========================================

    private void RespawnCurrentMap()
    {
        if (player == null ||
            maps == null ||
            maps.Length == 0)
        {
            return;
        }


        if (currentMapIndex < 0 ||
            currentMapIndex >= maps.Length)
        {
            return;
        }


        MapInfo currentMap =
            maps[currentMapIndex];


        if (currentMap == null ||
            currentMap.RespawnPoint == null)
        {
            return;
        }


        player.Respawn(
            currentMap.RespawnPoint.position
        );
    }


    // =========================================
    // GET MAP
    // =========================================

    public int GetCurrentMapIndex()
    {
        return currentMapIndex;
    }
    // =========================================
// CHEST REVEAL
// =========================================

public void PlayChestReveal(
    Chest chest
)
{
    if (chest == null)
        return;


    if (playingChestSequence ||
        handlingDeath ||
        transitioning ||
        gameOver)
    {
        return;
    }


    StartCoroutine(
        ChestRevealRoutine(
            chest
        )
    );
}


private IEnumerator ChestRevealRoutine(
    Chest chest
)
{
    playingChestSequence = true;


    // =====================================
    // KHÓA PLAYER
    // =====================================

    if (player != null)
    {
        player.SetControlsLocked(
            true
        );
    }


    // =====================================
    // LƯU CAMERA
    // =====================================

    Vector3 originalPosition =
        mainCamera.transform.position;


    float originalSize =
        mainCamera.orthographicSize;


    bool brainWasEnabled = false;


    if (cinemachineBrain != null)
    {
        brainWasEnabled =
            cinemachineBrain.enabled;


        // Tắt Cinemachine để tự điều khiển camera
        cinemachineBrain.enabled =
            false;
    }


    // =====================================
    // VỊ TRÍ CAMERA MUỐN TỚI
    // =====================================

    Transform focus =
        chest.GetFocusPoint();


    Vector3 targetPosition =
        focus.position;


    targetPosition.z =
        originalPosition.z;


    // =====================================
    // GIỚI HẠN CAMERA KHÔNG RA NGOÀI MAP
    // =====================================

    float originalHalfHeight =
        originalSize;


    float originalHalfWidth =
        originalHalfHeight *
        mainCamera.aspect;


    float zoomHalfHeight =
        chestZoomSize;


    float zoomHalfWidth =
        zoomHalfHeight *
        mainCamera.aspect;


    float minX =
        originalPosition.x
        - originalHalfWidth
        + zoomHalfWidth;


    float maxX =
        originalPosition.x
        + originalHalfWidth
        - zoomHalfWidth;


    float minY =
        originalPosition.y
        - originalHalfHeight
        + zoomHalfHeight;


    float maxY =
        originalPosition.y
        + originalHalfHeight
        - zoomHalfHeight;


    targetPosition.x =
        Mathf.Clamp(
            targetPosition.x,
            minX,
            maxX
        );


    targetPosition.y =
        Mathf.Clamp(
            targetPosition.y,
            minY,
            maxY
        );


    // =====================================
    // ZOOM TỚI RƯƠNG
    // =====================================

    float timer = 0f;


    while (timer <
           chestZoomDuration)
    {
        timer +=
            Time.deltaTime;


        float t =
            Mathf.Clamp01(
                timer /
                chestZoomDuration
            );


        float smoothT =
            Mathf.SmoothStep(
                0f,
                1f,
                t
            );


        mainCamera.transform.position =
            Vector3.Lerp(
                originalPosition,
                targetPosition,
                smoothT
            );


        mainCamera.orthographicSize =
            Mathf.Lerp(
                originalSize,
                chestZoomSize,
                smoothT
            );


        yield return null;
    }


    mainCamera.transform.position =
        targetPosition;


    mainCamera.orthographicSize =
        chestZoomSize;


    // =====================================
    // ITEM XUẤT HIỆN
    // =====================================

    yield return StartCoroutine(
        chest.RevealItem()
    );


    // =====================================
    // GIỮ CAMERA
    // =====================================

    if (chestHoldDuration > 0f)
    {
        yield return
            new WaitForSeconds(
                chestHoldDuration
            );
    }


    // =====================================
    // CAMERA TRỞ LẠI
    // =====================================

    Vector3 returnStartPosition =
        mainCamera.transform.position;


    float returnStartSize =
        mainCamera.orthographicSize;


    timer = 0f;


    while (timer <
           chestCameraReturnDuration)
    {
        timer +=
            Time.deltaTime;


        float t =
            Mathf.Clamp01(
                timer /
                chestCameraReturnDuration
            );


        float smoothT =
            Mathf.SmoothStep(
                0f,
                1f,
                t
            );


        mainCamera.transform.position =
            Vector3.Lerp(
                returnStartPosition,
                originalPosition,
                smoothT
            );


        mainCamera.orthographicSize =
            Mathf.Lerp(
                returnStartSize,
                originalSize,
                smoothT
            );


        yield return null;
    }


    // =====================================
    // ĐẢM BẢO CAMERA CHUẨN
    // =====================================

    mainCamera.transform.position =
        originalPosition;


    mainCamera.orthographicSize =
        originalSize;


    // =====================================
    // BẬT CINEMACHINE LẠI
    // =====================================

    if (cinemachineBrain != null)
    {
        cinemachineBrain.enabled =
            brainWasEnabled;
    }


    // =====================================
    // MỞ KHÓA PLAYER
    // =====================================

    if (player != null)
    {
        player.SetControlsLocked(
            false
        );
    }


    playingChestSequence = false;
}
}