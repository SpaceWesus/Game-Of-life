using UnityEngine;

public class UITweak : MonoBehaviour
{
    public float speed = 1.5f;

    public float minAlpha = 0;
    public float maxAlpha = 1;

    private CanvasGroup canvasGroup;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        float wave = Mathf.Sin(speed * Time.time);
        float normalizedWave = (wave + 1.0f) / 2.0f;

        canvasGroup.alpha = Mathf.Lerp(minAlpha, maxAlpha, normalizedWave);
    }
}
