
using UnityEngine;
using Unity.Cinemachine;

public class CameraAspectFix : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cmCamera;
    [SerializeField] private float baseOrthoSize = 5f;

    private const float referenceAspect = 16f / 9f;
    private float previousAspect = -1f;

    private void LateUpdate()
    {
        if (cmCamera == null)
            return;

        float aspect = (float)Screen.width / Screen.height;

        if (Mathf.Approximately(aspect, previousAspect))
            return;

        previousAspect = aspect;

        float targetSize = baseOrthoSize;

        if (aspect > referenceAspect)
        {
            targetSize *= referenceAspect / aspect;
        }

        var lens = cmCamera.Lens;
        lens.OrthographicSize = targetSize;
        cmCamera.Lens = lens;
    }
}
