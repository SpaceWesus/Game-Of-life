using UnityEngine;

public class BackgroundColorOscillatorWorld : MonoBehaviour
{
    [SerializeField] private Renderer rend;
    [SerializeField] private Color[] colors;
    [SerializeField] private float cycleDuration = 6f;

    private int a, b;
    private float t;
    private Material mat;

    void Awake()
    {
        if (!rend) rend = GetComponent<Renderer>();
        mat = rend.material; // instance material
        a = 0;
        b = colors.Length > 1 ? 1 : 0;
        mat.color = colors[a];
    }

    void Update()
    {
        if (colors == null || colors.Length < 2) return;

        t += Time.deltaTime / cycleDuration;
        mat.color = Color.Lerp(colors[a], colors[b], Mathf.SmoothStep(0, 1, t));

        if (t >= 1f)
        {
            t = 0f;
            a = b;
            b = (b + 1) % colors.Length;
        }
    }
}
