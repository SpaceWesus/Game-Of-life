using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CustomizeUI : MonoBehaviour
{
    public UnityEngine.UI.Slider[] sliders;
    public TMP_Text[] labels;                    // for tmp
    public UnityEngine.UI.Image[] swatches;      // force Unity's Image
    public ColorScript[] colorDefs;
    // todo scroll wheel import

    void Start()
    {
        if (RandomInitSettings.I == null)
            new GameObject("RandomInitSettings").AddComponent<RandomInitSettings>();

        if (RandomInitSettings.I.weights == null)
            RandomInitSettings.I.weights = new List<int>();
        while (RandomInitSettings.I.weights.Count < colorDefs.Length)
            RandomInitSettings.I.weights.Add(0);

        for (int i = 0; i < colorDefs.Length; i++)
        {
            if (i < labels.Length && labels[i] != null)
                labels[i].text = colorDefs[i].name;

            if (i < swatches.Length && swatches[i] != null)
                swatches[i].color = colorDefs[i].GetColor();

            int defaultW = colorDefs[i].GetRandomInitWeight();
            int currentW = RandomInitSettings.I.weights[i] > 0 ? RandomInitSettings.I.weights[i] : defaultW;

            sliders[i].minValue = 0;
            sliders[i].maxValue = 50;
            sliders[i].wholeNumbers = true;
            sliders[i].value = currentW;

            int idx = i;
            sliders[i].onValueChanged.AddListener(v =>
            {
                RandomInitSettings.I.weights[idx] = (int)v;
                RandomInitSettings.I.overrideRandomInit = true;
            });
        }
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
