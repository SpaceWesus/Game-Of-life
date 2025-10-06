using UnityEngine;

public class ColorWhite : ColorScript
{
    public Color colorBlack;

    public override Color CheckRules(Vector2Int pos)
    {
        Grid1 grid = transform.parent.GetComponent<Grid1>();

        // If fewer than two adjacent white neighbors, turn black.
        Color[] colorList = new Color[1];
        colorList[0] = colorBlack;
        if (grid.CheckCount(pos, colorList, 2, Grid1.Neighbors.Adjacent, Grid1.Operation.LessThan)) return colorBlack;

        // 2-3 adjacent white neighbors, stay white.
        if (grid.CheckCount(pos, colorList, 2, Grid1.Neighbors.Adjacent, Grid1.Operation.GreaterThanEqual) &&
            grid.CheckCount(pos, colorList, 3, Grid1.Neighbors.Adjacent, Grid1.Operation.LessThanEqual)) return color;

        // More than 3 adjacent white neighbors, turn black.
        if (grid.CheckCount(pos, colorList, 3, Grid1.Neighbors.Adjacent, Grid1.Operation.GreaterThan)) return colorBlack;

        // Fallback case.
        return colorBlack;
    }
}
