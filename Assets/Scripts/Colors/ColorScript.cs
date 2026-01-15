using UnityEngine;

public class ColorScript : MonoBehaviour
{
    [SerializeField] protected Color color;              // display color
    [SerializeField] protected int randomInitWeight = 1; // used by random init

    // base rule (children override). returns current color by default.
    public virtual Color CheckRules(Vector2Int pos)
    {
        // grid is available if a child needs it: transform.parent.GetComponent<Grid>()
        return color;
    }

    // getters
    public Color GetColor() { return color; }
    public int GetRandomInitWeight() { return randomInitWeight; }

    // setter so UI sliders can update the weight live
    public void SetRandomInitWeight(int w)
    {
        randomInitWeight = Mathf.Max(0, w);
    }
}
