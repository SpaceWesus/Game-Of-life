using UnityEngine;

public class ColorScript : MonoBehaviour
{
    [SerializeField] protected Color color;

    public virtual Color CheckRules(Vector2Int pos)
    {
        Grid grid = transform.parent.GetComponent<Grid>();

        return color;
    }

    public Color GetColor()
    {
        return color;
    }
}
