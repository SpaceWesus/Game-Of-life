using UnityEngine;

public class ColorBlack : ColorScript
{
    public Color colorWhite;
    public Color colorBlue;
    public Color colorGreen;
    public Color colorYellow;
    public Color colorRed;

    public override Color CheckRules(Vector2Int pos)
    {
        Grid grid = transform.parent.GetComponent<Grid>();
        Color[] colorList = new Color[1];
        Color[] colorList2 = new Color[1];

        // If exactly 3 yellow corner neighbors, turn yellow.
        colorList[0] = colorYellow;
        if (grid.CheckCount(pos, colorList, 3, Grid.Neighbors.Corner, Grid.Operation.Equal)) return colorYellow;

        // If exactly 3 red adjacent neighbors, turn red.
        colorList[0] = colorRed;
        if (grid.CheckCount(pos, colorList, 3, Grid.Neighbors.Adjacent, Grid.Operation.Equal)) { Debug.Log("RED BORN"); return colorRed; }

        // If exactly 3 white neighbors, turn white.
        colorList[0] = colorWhite;
        if (grid.CheckCount(pos, colorList, 3, Grid.Neighbors.All, Grid.Operation.Equal)) return colorWhite;

        // If exactly 3 non-Black or Blue neighbors (5 Black or Blue neighbors) and at least one is Green, turn Green.
        colorList = new Color[2];
        colorList[0] = color;
        colorList[1] = colorBlue;
        colorList2[0] = colorGreen;
        if (grid.CheckCount(pos, colorList, 5, Grid.Neighbors.All, Grid.Operation.Equal) && grid.CheckCount(pos, colorList2, 1, Grid.Neighbors.All, Grid.Operation.GreaterThanEqual)) return colorGreen;

        // Otherwise turn black.
        return color;
    }
}
