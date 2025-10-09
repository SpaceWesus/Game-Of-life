using UnityEngine;

public class ColorWhite : ColorScript
{
    public Color colorBlack;

    public override Color CheckRules(Vector2Int pos)
    {
        Grid grid = transform.parent.GetComponent<Grid>();

        // If fewer than two white neighbors, turn black.
        Color[] colorList = new Color[1];
        colorList[0] = color;
        if (grid.CheckCount(pos, colorList, 2, Grid.Neighbors.All, Grid.Operation.LessThan)) return colorBlack;

        // 2-3 white neighbors, stay white.
        if (grid.CheckCount(pos, colorList, 2, Grid.Neighbors.All, Grid.Operation.GreaterThanEqual) &&
            grid.CheckCount(pos, colorList, 3, Grid.Neighbors.All, Grid.Operation.LessThanEqual)) return color;

        // More than 3 white neighbors, turn black.
        if (grid.CheckCount(pos, colorList, 3, Grid.Neighbors.All, Grid.Operation.GreaterThan)) return colorBlack;

        // Fallback case.
        return colorBlack;
    }
}
