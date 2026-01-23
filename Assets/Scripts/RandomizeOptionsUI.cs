using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RandomizeOptionsUI : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Grid grid;
    [SerializeField] private Transform contentRoot;
    [SerializeField] private WeightRow rowTemplate;
    private bool built = false;

    private List<WeightRow> rows = new();

    void Awake()
    {
        if (!grid) grid = FindFirstObjectByType<Grid>();
        if (panelRoot) panelRoot.SetActive(false);
    }

    IEnumerator Start()
    {
        if (panelRoot) panelRoot.SetActive(false);

        // Wait until Grid has initialized its colors array
        while (grid != null && !grid.IsInitialized)
            yield return null;


        Build();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
            panelRoot.SetActive(!panelRoot.activeSelf);
    }

    void Build()
    {
        if (built) return;
            built = true;

        var colors = grid.GetColorScripts(); // we’ll add this

        if (colors == null || colors.Length == 0)
        {
            Debug.LogError("RandomizeOptionsUI: Grid colors not ready. Ensure Grid initialized before building UI.");
            return;
        }
        for (int i = 0; i < colors.Length; i++)
        {
            var r = Instantiate(rowTemplate, contentRoot);
            r.gameObject.SetActive(true);
            r.Bind(colors[i]);
            rows.Add(r);
        }
            
        Debug.Log($"RandomizeOptionsUI built {rows.Count} rows.");

    }
}
