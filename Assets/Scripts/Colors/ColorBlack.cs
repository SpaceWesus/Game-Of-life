using UnityEngine;

public class ColorBlack : ColorScript
{
    public Color colorWhite;
    public Color colorGreen;

    public override Color CheckRules(Vector2Int pos)
    {
        Grid grid = transform.parent.GetComponent<Grid>();
        Color[] colorList = new Color[1];

        // If exactly 3 white neighbors, turn white.
        colorList[0] = colorWhite;
        if (grid.CheckCount(pos, colorList, 3, Grid.Neighbors.All, Grid.Operation.Equal)) return colorWhite;

        // If exactly 3 green neighbors, turn green.
        colorList[0] = colorGreen;
        if (grid.CheckCount(pos, colorList, 3, Grid.Neighbors.All, Grid.Operation.Equal)) return colorGreen;

        // Otherwise turn black.
        return color;
    }
}
