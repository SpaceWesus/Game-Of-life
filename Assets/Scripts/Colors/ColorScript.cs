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

    public Color GetColor() { return color; }

    public int GetRandomInitWeight() { return randomInitWeight; }

    public string GetRuleDescription()
    {
        return ruleDescText;
    }

    public void SetRandomInitWeight(int w)
    {
        randomInitWeight = Mathf.Max(0, w);
    }

    public void SetEnabledForRandom(bool enabled)
    {
        if (!enabled) randomInitWeight = 0;
        else if (randomInitWeight == 0) randomInitWeight = 1;
    }


}
