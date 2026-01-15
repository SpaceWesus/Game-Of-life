using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RandomWeightsAutoUI : MonoBehaviour
{
    [Header("refs")]
    public Grid grid;                          
    public RectTransform contentParent;        // the vertical list parent (with VLG + CSF)
    public WeightRowUI rowPrefab;              // row prefab (swatch + label + slider)
    public UnityEngine.UI.Button randomizeButton; // click to refill board
    public int sliderMax = 50;                 // cap per weight

    // cache colors from the scene
    private ColorScript[] colors;

    void Awake()
    {
        if (RandomInitSettings.I == null)
            new GameObject("RandomInitSettings").AddComponent<RandomInitSettings>();
    }

    void Start()
    {
        // find grid if not wired
        if (!grid) grid = FindObjectOfType<Grid>();

        // pull color defs from grid children (order matters)
        Transform src = grid.colorRoot ? grid.colorRoot : grid.transform;
        colors = grid.transform.GetComponentsInChildren<ColorScript>();

        // make sure weights list has same length
        EnsureWeightsSize(colors.Length);

        // clear any old rows under the content
        for (int i = contentParent.childCount - 1; i >= 0; i--)
            Destroy(contentParent.GetChild(i).gameObject);

        // build one UI row per color
        for (int i = 0; i < colors.Length; i++)
        {
            var c = colors[i];
            var row = Instantiate(rowPrefab, contentParent);

            int saved = RandomInitSettings.I.weights[i];
            int fallback = c.GetRandomInitWeight();
            int startVal = (saved > 0) ? saved : fallback;

            // connect up BOTH sources so UI, override list, and colors all match
            RandomInitSettings.I.weights[i] = startVal;   // override list
            colors[i].SetRandomInitWeight(startVal);      // per-color too (nice to keep in sync)

            int idx = i;
            row.Init(c.name, c.GetColor(), startVal, sliderMax, (newVal) =>
            {
                // update on slider drag
                RandomInitSettings.I.weights[idx] = newVal;
                colors[idx].SetRandomInitWeight(newVal);
                RandomInitSettings.I.overrideRandomInit = true;
            });
        }

        // override 
        RandomInitSettings.I.overrideRandomInit = true;



        // click to apply (refill using weights)
        if (randomizeButton)
            randomizeButton.onClick.AddListener(() =>
            {
                if (RandomInitSettings.I) RandomInitSettings.I.overrideRandomInit = true; // off for now used to be on
                if (grid) grid.ReRandomizeFromSettings();
            });

    }

    // grow/shrink the weights list to match color count
    private void EnsureWeightsSize(int n)
    {
        if (RandomInitSettings.I.weights == null)
            RandomInitSettings.I.weights = new List<int>();

        while (RandomInitSettings.I.weights.Count < n)
            RandomInitSettings.I.weights.Add(0);

        if (RandomInitSettings.I.weights.Count > n)
            RandomInitSettings.I.weights.RemoveRange(n, RandomInitSettings.I.weights.Count - n);
    }
}
