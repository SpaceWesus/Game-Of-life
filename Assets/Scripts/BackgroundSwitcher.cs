using UnityEngine;

public class BackgroundSwitcher : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Renderer rend;

    [Header("Background Modes")]
    [SerializeField] private Material[] backgrounds;

    [Header("Hotkeys")]
    [SerializeField] private KeyCode cycleKey = KeyCode.B;

    private int index = 0;

    void Awake()
    {
        if (!rend) rend = GetComponent<Renderer>();
        Apply();
    }

    void Update()
    {
        if (Input.GetKeyDown(cycleKey))
        {
            index = (index + 1) % backgrounds.Length;
            Apply();
        }
    }

    private void Apply()
    {
        if (backgrounds == null || backgrounds.Length == 0) return;

        rend.material = backgrounds[index];
    }
}
