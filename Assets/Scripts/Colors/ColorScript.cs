using UnityEngine;

public class ColorScript : MonoBehaviour
{
    [SerializeField] protected Color color;
    [SerializeField] protected int randomInitWeight = 1;

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
}
