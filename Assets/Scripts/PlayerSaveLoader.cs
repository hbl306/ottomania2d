using UnityEngine;

public class PlayerSaveLoader : MonoBehaviour
{
    private void Start()
    {
        if (!SaveSystem.LoadFromContinue)
            return;

        if (!SaveSystem.HasSave())
            return;

        transform.position =
            SaveSystem.GetPlayerPosition();

        SaveSystem.LoadFromContinue = false;
    }
}