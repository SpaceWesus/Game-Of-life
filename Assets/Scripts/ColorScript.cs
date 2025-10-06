using UnityEngine;

public class ColorScript : MonoBehaviour
{
    [SerializeField] protected Color color;

    public virtual Color CheckRules(Vector2Int pos)
    {
        Grid1 grid = transform.parent.GetComponent<Grid1>();

        return color;
    }

    public Color GetColor()
    {
        return color;
    }
}
