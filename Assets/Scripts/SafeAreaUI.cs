
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeAreaUI : MonoBehaviour
{
    private RectTransform panel;
    private Rect previousSafeArea;
    private int previousWidth;
    private int previousHeight;

    private void Awake()
    {
        panel = GetComponent<RectTransform>();
        ApplySafeArea();
    }

    private void Update()
    {
        if (Screen.safeArea != previousSafeArea ||
            Screen.width != previousWidth ||
            Screen.height != previousHeight)
        {
            ApplySafeArea();
        }
    }

    private void ApplySafeArea()
    {
        Rect safeArea = Screen.safeArea;

        Vector2 min = safeArea.position;
        Vector2 max = safeArea.position + safeArea.size;

        min.x /= Screen.width;
        min.y /= Screen.height;

        max.x /= Screen.width;
        max.y /= Screen.height;

        panel.anchorMin = min;
        panel.anchorMax = max;

        panel.offsetMin = Vector2.zero;
        panel.offsetMax = Vector2.zero;

        previousSafeArea = safeArea;
        previousWidth = Screen.width;
        previousHeight = Screen.height;
    }
}
