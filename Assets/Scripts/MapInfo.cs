using UnityEngine;

public class MapInfo : MonoBehaviour
{
    [Header("Map Points")]
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private Transform cameraPoint;

    public Transform RespawnPoint => respawnPoint;
    public Transform CameraPoint => cameraPoint;
}