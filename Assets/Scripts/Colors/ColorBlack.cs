using UnityEngine;

public class ColorBlack : ColorScript
{
    public Color colorWhite;

    public override Color CheckRules(Vector2Int pos)
    {
        Grid grid = transform.parent.GetComponent<Grid>();

        // If exactly 3 adjacent white neighbors, turn white.
        Color[] colorList = new Color[1];
        colorList[0] = colorWhite;
        if (grid.CheckCount(pos, colorList, 3, Grid.Neighbors.All, Grid.Operation.Equal)) return colorWhite;

        // Otherwise turn black.
        return color;
    }
}
