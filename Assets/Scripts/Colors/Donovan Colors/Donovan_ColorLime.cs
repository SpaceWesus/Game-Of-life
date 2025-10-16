using UnityEngine;

public class Donovan_ColorLime : ColorScript
{
    public Color colorBlack;

    public override Color CheckRules(Vector2Int pos)
    {
        // Simply dies on the next frame.
        return colorBlack;
    }
}
