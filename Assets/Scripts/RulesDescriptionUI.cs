using TMPro;
using UnityEngine;

public class RulesDescriptionUI : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private TMP_Text rulesText;

    private ColorScript lastColor;

    void Awake()
    {
        if (!rulesText) rulesText = GetComponentInChildren<TMP_Text>();
        if (!grid) grid = FindFirstObjectByType<Grid>();
    }

    void Update()
    {
        if (!grid) return;

        ColorScript current = grid.GetSelectColorID();
        if (current == null) return;

        if (current != lastColor)
        {
            lastColor = current;
            rulesText.text = current.GetRuleDescription();
        }
    }
}
