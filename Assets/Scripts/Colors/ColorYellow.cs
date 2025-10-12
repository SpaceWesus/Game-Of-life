using UnityEngine;

public class ColorYellow : ColorScript
{
    public Color colorBlack;

    public override Color CheckRules(Vector2Int pos)
    {
        Grid grid = transform.parent.GetComponent<Grid>();

        // If fewer than two yellow corner neighbors, turn black.
        Color[] colorList = new Color[1];
        colorList[0] = color;
        if (grid.CheckCount(pos, colorList, 2, Grid.Neighbors.Corner, Grid.Operation.LessThan)) return colorBlack;

        // 2-3 yellow corner neighbors, stay yellow.
        if (grid.CheckCount(pos, colorList, 2, Grid.Neighbors.Corner, Grid.Operation.GreaterThanEqual) &&
            grid.CheckCount(pos, colorList, 3, Grid.Neighbors.Corner, Grid.Operation.LessThanEqual)) return color;

        // More than 3 yellow corner neighbors, turn black.
        if (grid.CheckCount(pos, colorList, 3, Grid.Neighbors.Corner, Grid.Operation.GreaterThan)) return colorBlack;

        // Fallback case.
        return colorBlack;
    }
}
