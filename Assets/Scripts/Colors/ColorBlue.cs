using UnityEngine;

public class ColorBlue : ColorScript
{
    public override Color CheckRules(Vector2Int pos)
    {
        // Inert color, always returns itself.
        return color;
    }
}
