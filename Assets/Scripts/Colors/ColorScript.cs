using UnityEngine;

public class ColorScript : MonoBehaviour
{
    [SerializeField] protected Color color;
    [SerializeField] protected int randomInitWeight = 1;

    [SerializeField] private string ruleDescText = "No Rules to show now";

    public virtual Color CheckRules(Vector2Int pos)
    {
        Grid grid = transform.parent.GetComponent<Grid>();

        return color;
    }

    public Color GetColor() 
    {
        //bug.Log(gameObject.name + " is reporting its color as: " + color);
        return color; 
    }

    public int GetRandomInitWeight() { return randomInitWeight; }

    public string GetRuleDescription()
    {
        return ruleDescText;
    }
}
