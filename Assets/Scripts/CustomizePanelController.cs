using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SidePanelController : MonoBehaviour
{
    public float shownX = 0f;        // where the panel sits when visible
    public float hiddenX = -500f;    
    public float duration = 0.2f;    // slide time
    public AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1);

    RectTransform rt;
    bool isOpen = false;
    bool animating = false;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        // start hidden
        var p = rt.anchoredPosition;
        p.x = hiddenX;
        rt.anchoredPosition = p;
    }

    public void Toggle()
    {
        if (animating) return;
        isOpen = !isOpen;
        StopAllCoroutines();
        StartCoroutine(Slide(isOpen ? shownX : hiddenX));
    }

    System.Collections.IEnumerator Slide(float targetX)
    {
        animating = true;
        Vector2 start = rt.anchoredPosition;
        Vector2 target = new Vector2(targetX, start.y);
        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / duration;
            float k = ease.Evaluate(Mathf.Clamp01(t));
            rt.anchoredPosition = Vector2.LerpUnclamped(start, target, k);
            yield return null;
        }
        rt.anchoredPosition = target;
        animating = false;
    }
}
