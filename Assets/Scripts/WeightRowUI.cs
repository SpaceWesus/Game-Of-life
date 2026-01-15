using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeightRowUI : MonoBehaviour
{
    public Image swatch;
    public TMP_Text label;
    public Slider slider;

    // adding everything based on color, and making a swatch of that color to display.
    public void Init(string displayName, Color color, int start, int max, System.Action<int> onChanged)
    {
        if (label) label.text = displayName;
        if (swatch) swatch.color = color;

        if (slider)
        {
            slider.wholeNumbers = true;
            slider.minValue = 0;
            slider.maxValue = Mathf.Max(1, max);
            slider.value = Mathf.Max(0, start);
            slider.onValueChanged.AddListener(v => onChanged?.Invoke((int)v));
        }
    }
}
