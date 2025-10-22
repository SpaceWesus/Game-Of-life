using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class UIDetectHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Grid grid;
    public UI_Tween ui_tween;

    public TMP_Text ruleText;

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        ColorScript selectedColor = grid.GetSelectColorID();

        ruleText.text = selectedColor.GetRuleDescription();
        ui_tween.OpenWindow();

    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        ui_tween.CloseWindow();
    }
}
