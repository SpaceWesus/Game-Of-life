using UnityEngine;
using UnityEngine.UI;

public class MenuBackgroundOscillator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image backgroundImage;

    [Header("Color Cycle")]
    [SerializeField] private Color[] colors;
    [SerializeField] private float cycleDuration = 6f; // seconds per blend

    private int currentIndex;
    private int nextIndex;
    private float t;

    void Awake()
    {
        if (!backgroundImage)
            backgroundImage = GetComponent<Image>();

        currentIndex = 0;
        nextIndex = colors.Length > 1 ? 1 : 0;
        backgroundImage.color = colors[currentIndex];
    }

    void Update()
    {
        if (colors.Length < 2) return;

        t += Time.deltaTime / cycleDuration;
        float blend = Mathf.SmoothStep(0f, 1f, t);

        Color baseColor = Color.Lerp(
            colors[currentIndex],
            colors[nextIndex],
            blend
        );

        float noise = Mathf.PerlinNoise(Time.time * 0.15f, 0f) - 0.5f;
        baseColor.r += noise * 0.05f;
        baseColor.g += noise * 0.03f;
        baseColor.b += noise * 0.04f;

        backgroundImage.color = baseColor;

        if (t >= 1f)
        {
            t = 0f;
            currentIndex = nextIndex;
            nextIndex = (nextIndex + 1) % colors.Length;
        }
    }

}
