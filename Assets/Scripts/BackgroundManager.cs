using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    [Header("Renderer")]
    [SerializeField] private Renderer rend;

    [Header("Background Materials (cycle with B)")]
    [SerializeField] private Material[] backgrounds;

    [Header("Oscillator Settings (only applies when current material is oscillatorMaterial)")]
    [SerializeField] private Material oscillatorMaterial;
    [SerializeField] private Color[] oscColors;
    [SerializeField] private float cycleDuration = 6f;

    private int bgIndex = 0;
    private int a = 0, b = 1;
    private float t = 0f;

    void Awake()
    {
        if (!rend) rend = GetComponent<Renderer>();

        if (backgrounds != null && backgrounds.Length > 0)
            rend.sharedMaterial = backgrounds[bgIndex];

        // Initialize oscillator endpoints safely
        if (oscColors != null && oscColors.Length >= 2)
        {
            a = 0;
            b = 1;
        }
    }

    void Update()
    {
        // B cycles materials
        if (Input.GetKeyDown(KeyCode.B) && backgrounds != null && backgrounds.Length > 0)
        {
            bgIndex = (bgIndex + 1) % backgrounds.Length;
            rend.sharedMaterial = backgrounds[bgIndex];
        }

        // Run oscillator ONLY when oscillator material is the active one
        if (oscillatorMaterial != null && rend.sharedMaterial == oscillatorMaterial)
        {
            RunOscillator();
        }
    }

    private void RunOscillator()
    {
        if (oscColors == null || oscColors.Length < 2) return;

        t += Time.deltaTime / Mathf.Max(0.0001f, cycleDuration);

        // Smooth blend
        Color c = Color.Lerp(oscColors[a], oscColors[b], Mathf.SmoothStep(0, 1, t));

        // IMPORTANT: write to the material asset that's currently assigned
        oscillatorMaterial.color = c;

        if (t >= 1f)
        {
            t = 0f;
            a = b;
            b = (b + 1) % oscColors.Length;
        }
    }
}
