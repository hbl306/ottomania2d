using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    // =====================================================
    // AUDIO SOURCES
    // =====================================================

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource ambientSource;


    // =====================================================
    // MUSIC
    // =====================================================

    [Header("Music")]
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private AudioClip gamePlayMusic;
    [SerializeField] private AudioClip bossMusic;
    [SerializeField] private AudioClip victoryMusic;
    [SerializeField] private AudioClip gameOverMusic;


    // =====================================================
    // PLAYER
    // =====================================================

    [Header("Player SFX")]
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip attackSwingSound;
    [SerializeField] private AudioClip playerHurtSound;
    [SerializeField] private AudioClip playerDeathSound;
    [SerializeField] private AudioClip playerRespawnSound;


    // =====================================================
    // ITEM
    // =====================================================

    [Header("Item SFX")]
    [SerializeField] private AudioClip coinSound;
    [SerializeField] private AudioClip itemPickupSound;
    [SerializeField] private AudioClip chestOpeningSound;
    [SerializeField] private AudioClip itemRevealSound;


    // =====================================================
    // BOSS
    // =====================================================

    [Header("Boss SFX")]
    [SerializeField] private AudioClip projectileFireSound;
    [SerializeField] private AudioClip projectileReflectSound;
    [SerializeField] private AudioClip reflectedHitSound;
    [SerializeField] private AudioClip laserChargeSound;
    [SerializeField] private AudioClip laserFireSound;


    // =====================================================
    // UI
    // =====================================================

    [Header("UI SFX")]
    [SerializeField] private AudioClip buttonClickSound;


    // =====================================================
    // AMBIENT
    // =====================================================

    [Header("Ambient")]
    [SerializeField] private AudioClip waterSound;


    private float musicVolume = 1f;
    private float soundVolume = 1f;


    // =====================================================
    // INITIALIZE
    // =====================================================

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);

            LoadVolume();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }


    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    private void Start()
    {
        PlayAudioForScene(
            SceneManager.GetActiveScene().name
        );
    }


    // =====================================================
    // SCENE MUSIC
    // =====================================================

    private void OnSceneLoaded(
        Scene scene,
        LoadSceneMode mode
    )
    {
        PlayAudioForScene(scene.name);
    }


    private void PlayAudioForScene(string sceneName)
    {
        if (sceneName == "Menu")
        {
            StopAmbient();

            PlayMusic(mainMenuMusic);
        }
        else if (sceneName == "Game")
        {
            PlayMusic(gamePlayMusic);

            //PlayWater();
        }
    }


    // =====================================================
    // MUSIC
    // =====================================================

    private void PlayMusic(AudioClip clip)
    {
        if (clip == null)
            return;

        if (musicSource.clip == clip &&
            musicSource.isPlaying)
        {
            return;
        }

        musicSource.Stop();

        musicSource.clip = clip;
        musicSource.loop = true;

        musicSource.Play();
    }


    public void PlayMainMenuMusic()
    {
        PlayMusic(mainMenuMusic);
    }


    public void PlayGameMusic()
    {
        PlayMusic(gamePlayMusic);
    }


    public void PlayBossMusic()
    {
        PlayMusic(bossMusic);
    }


    public void PlayVictoryMusic()
    {
        PlayMusic(victoryMusic);
    }


    public void PlayGameOverMusic()
    {
        PlayMusic(gameOverMusic);
    }


    // =====================================================
    // SFX CORE
    // =====================================================

    private void PlaySFX(AudioClip clip)
    {
        if (clip == null)
            return;

        sfxSource.PlayOneShot(clip);
    }


    // =====================================================
    // PLAYER
    // =====================================================

    public void PlayJump()
    {
        PlaySFX(jumpSound);
    }

    public void PlayAttack()
    {
        PlaySFX(attackSwingSound);
    }

    public void PlayPlayerHurt()
    {
        PlaySFX(playerHurtSound);
    }

    public void PlayPlayerDeath()
    {
        PlaySFX(playerDeathSound);
    }

    public void PlayPlayerRespawn()
    {
        PlaySFX(playerRespawnSound);
    }


    // =====================================================
    // ITEM
    // =====================================================

    public void PlayCoin()
    {
        PlaySFX(coinSound);
    }

    public void PlayItemPickup()
    {
        PlaySFX(itemPickupSound);
    }

    public void PlayChestOpen()
    {
        PlaySFX(chestOpeningSound);
    }

    public void PlayItemReveal()
    {
        PlaySFX(itemRevealSound);
    }


    // =====================================================
    // BOSS
    // =====================================================

    public void PlayProjectileFire()
    {
        PlaySFX(projectileFireSound);
    }

    public void PlayProjectileReflect()
    {
        PlaySFX(projectileReflectSound);
    }

    public void PlayReflectedHit()
    {
        PlaySFX(reflectedHitSound);
    }

    public void PlayLaserCharge()
    {
        PlaySFX(laserChargeSound);
    }

    public void PlayLaserFire()
    {
        PlaySFX(laserFireSound);
    }


    // =====================================================
    // UI
    // =====================================================

    public void PlayButtonClick()
    {
        PlaySFX(buttonClickSound);
    }


    // =====================================================
    // AMBIENT
    // =====================================================

    public void PlayWater()
    {
        if (waterSound == null)
            return;

        if (ambientSource.clip == waterSound &&
            ambientSource.isPlaying)
        {
            return;
        }

        ambientSource.Stop();

        ambientSource.clip = waterSound;
        ambientSource.loop = true;

        ambientSource.Play();
    }


    public void StopAmbient()
    {
        ambientSource.Stop();
    }


    // =====================================================
    // VOLUME
    // =====================================================

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);

        musicSource.volume = musicVolume;

        PlayerPrefs.SetFloat(
            "MusicVolume",
            musicVolume
        );

        PlayerPrefs.Save();
    }


    public void SetSoundVolume(float volume)
    {
        soundVolume = Mathf.Clamp01(volume);

        sfxSource.volume = soundVolume;
        ambientSource.volume = soundVolume;

        PlayerPrefs.SetFloat(
            "SoundVolume",
            soundVolume
        );

        PlayerPrefs.Save();
    }


    private void LoadVolume()
    {
        musicVolume =
            PlayerPrefs.GetFloat(
                "MusicVolume",
                1f
            );

        soundVolume =
            PlayerPrefs.GetFloat(
                "SoundVolume",
                1f
            );

        musicSource.volume = musicVolume;

        sfxSource.volume = soundVolume;

        ambientSource.volume = soundVolume;
    }


    public float GetMusicVolume()
    {
        return musicVolume;
    }


    public float GetSoundVolume()
    {
        return soundVolume;
    }
    public void ResetAudioSettings()
{
    PlayerPrefs.DeleteKey("MusicVolume");
    PlayerPrefs.DeleteKey("SoundVolume");

    musicVolume = 1f;
    soundVolume = 1f;

    musicSource.volume = 1f;
    sfxSource.volume = 1f;
    ambientSource.volume = 1f;

    PlayerPrefs.Save();

    Debug.Log("Audio Settings Reset");
}
}