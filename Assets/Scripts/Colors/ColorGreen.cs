using UnityEngine;

public class ColorGreen : ColorScript
{
    public Color colorBlack;
    public Color colorWhite;
    public Color colorBlue;
    public Color colorLime;

    public override Color CheckRules(Vector2Int pos)
    {
        Grid grid = transform.parent.GetComponent<Grid>();

        // If at least one Lime neighbor, turn Lime.
        Color[] colorList = new Color[1];
        colorList[0] = colorLime;
        if (grid.CheckCount(pos, colorList, 0, Grid.Neighbors.All, Grid.Operation.GreaterThan)) return colorLime;

        // If fewer than two non-black neighbors, turn black.
        colorList = new Color[3];
        colorList[0] = colorWhite;
        colorList[1] = colorBlue;
        colorList[2] = color;
        if (grid.CheckCount(pos, colorList, 2, Grid.Neighbors.All, Grid.Operation.LessThan)) return colorBlack;

        // 2-3 non-black neighbors, stay white.
        if (grid.CheckCount(pos, colorList, 2, Grid.Neighbors.All, Grid.Operation.GreaterThanEqual) &&
            grid.CheckCount(pos, colorList, 3, Grid.Neighbors.All, Grid.Operation.LessThanEqual)) return color;

        // More than 3 non-black neighbors, turn black.
        if (grid.CheckCount(pos, colorList, 3, Grid.Neighbors.All, Grid.Operation.GreaterThan)) return colorBlack;

        // Fallback case.
        return colorBlack;
    }
}
