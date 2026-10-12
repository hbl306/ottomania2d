using UnityEngine;

public static class SaveSystem
{
    private const string HAS_SAVE = "HasSave";

    private const string PLAYER_X = "PlayerX";
    private const string PLAYER_Y = "PlayerY";
    private const string PLAYER_Z = "PlayerZ";

    private const string MAP_INDEX = "MapIndex";
    private const string PLAY_TIME = "PlayTime";

    public static bool LoadFromContinue = false;

    public static void SaveGame(
        Vector3 playerPosition,
        int mapIndex
    )
    {
        float currentPlayTime = 0f;

        GameManager gameManager =
            Object.FindAnyObjectByType<GameManager>();

        if (gameManager != null)
        {
            currentPlayTime =
                gameManager.GetPlayTime();
        }

        SaveGame(
            playerPosition,
            mapIndex,
            currentPlayTime
        );
    }

    public static void SaveGame(
        Vector3 playerPosition,
        int mapIndex,
        float playTime
    )
    {
        PlayerPrefs.SetInt(
            HAS_SAVE,
            1
        );

        PlayerPrefs.SetFloat(
            PLAYER_X,
            playerPosition.x
        );

        PlayerPrefs.SetFloat(
            PLAYER_Y,
            playerPosition.y
        );

        PlayerPrefs.SetFloat(
            PLAYER_Z,
            playerPosition.z
        );

        PlayerPrefs.SetInt(
            MAP_INDEX,
            mapIndex
        );

        PlayerPrefs.SetFloat(
            PLAY_TIME,
            Mathf.Max(0f, playTime)
        );

        PlayerPrefs.Save();
    }

    public static bool HasSave()
    {
        return PlayerPrefs.GetInt(
            HAS_SAVE,
            0
        ) == 1;
    }

    public static Vector3 GetPlayerPosition()
    {
        float x =
            PlayerPrefs.GetFloat(
                PLAYER_X,
                0f
            );

        float y =
            PlayerPrefs.GetFloat(
                PLAYER_Y,
                0f
            );

        float z =
            PlayerPrefs.GetFloat(
                PLAYER_Z,
                0f
            );

        return new Vector3(
            x,
            y,
            z
        );
    }

    public static int GetSavedMapIndex()
    {
        return PlayerPrefs.GetInt(
            MAP_INDEX,
            0
        );
    }

    public static float GetSavedPlayTime()
    {
        return PlayerPrefs.GetFloat(
            PLAY_TIME,
            0f
        );
    }

    public static void DeleteSave()
    {
        PlayerPrefs.DeleteKey(HAS_SAVE);

        PlayerPrefs.DeleteKey(PLAYER_X);
        PlayerPrefs.DeleteKey(PLAYER_Y);
        PlayerPrefs.DeleteKey(PLAYER_Z);

        PlayerPrefs.DeleteKey(MAP_INDEX);
        PlayerPrefs.DeleteKey(PLAY_TIME);

        PlayerPrefs.Save();

        LoadFromContinue = false;
    }
}