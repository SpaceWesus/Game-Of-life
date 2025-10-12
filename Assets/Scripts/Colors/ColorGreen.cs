using UnityEngine;

public class ColorGreen : ColorScript
{
    /*
     * COLOR GREEN
     * On its own, functions identical to regular Game of Life White.
     * Takes into account White, Red, and Yellow neighbors when determining survival and reproduction (see ColorBlack).
     * Yellow neighbors poison Green and turn it Lime. It can still form colonies next to Yellow, but less so than Red.
     * Treats Blue like Black. It cannot persist or reproduce off of Blue.
     */

    public Color colorBlack;
    public Color colorBlue;
    public Color colorLime;
    public Color colorYellow;

    public override Color CheckRules(Vector2Int pos)
    {
        Grid grid = transform.parent.GetComponent<Grid>();

        // If at least one Lime neighbor, turn Lime.
        Color[] colorList = new Color[1];
        colorList[0] = colorLime;
        if (grid.CheckCount(pos, colorList, 0, Grid.Neighbors.All, Grid.Operation.GreaterThan)) return colorLime;

        // If at least one Yellow neighbor, turn Lime.
        colorList[0] = colorYellow;
        if (grid.CheckCount(pos, colorList, 0, Grid.Neighbors.All, Grid.Operation.GreaterThan)) return colorLime;

        // If fewer than two non-Black or Blue neighbors (7 or more Black or Blue neighbors), turn Black.
        colorList = new Color[2];
        colorList[0] = colorBlack;
        colorList[1] = colorBlue;
        if (grid.CheckCount(pos, colorList, 7, Grid.Neighbors.All, Grid.Operation.GreaterThanEqual)) return colorBlack;

        // 2-3 non-Black or Blue neighbors (5-6 Black or Blue neighbors), stay Green.
        if (grid.CheckCount(pos, colorList, 5, Grid.Neighbors.All, Grid.Operation.GreaterThanEqual) &&
            grid.CheckCount(pos, colorList, 6, Grid.Neighbors.All, Grid.Operation.LessThanEqual)) return color;

        // More than 3 non-Black or Blue neighbors (4 or less Black or Blue neighbors), turn Black.
        if (grid.CheckCount(pos, colorList, 4, Grid.Neighbors.All, Grid.Operation.LessThanEqual)) return colorBlack;

        // Fallback case.
        return colorBlack;
    }
}
