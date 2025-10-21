using UnityEngine;

public class Donovan_ColorBlack : ColorScript
{
    public Color colorWhite;
    public Color colorBlue;
    public Color colorGreen;
    public Color colorYellow;
    public Color colorRed;
    public Color colorPurple;

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

        //if black has 1 purple adjacent, and the rest black, it grows
        colorList[0] = colorPurple;

        Color[] blackList = new Color[1] { color };
        if (grid.CheckCount(pos, colorList, 1, Grid.Neighbors.Adjacent, Grid.Operation.Equal) &&
        grid.CheckCount(pos, blackList, 7, Grid.Neighbors.All, Grid.Operation.Equal))
        {
            return colorPurple;
        }

        //if all 4 corners have a different color (excluding black, turn purple)
        Color corner1 = grid.GetColorAtPos(new Vector2Int(pos.x - 1, pos.y + 1));
        Color corner2 = grid.GetColorAtPos(new Vector2Int(pos.x + 1, pos.y + 1));
        Color corner3 = grid.GetColorAtPos(new Vector2Int(pos.x - 1, pos.y - 1));
        Color corner4 = grid.GetColorAtPos(new Vector2Int(pos.x + 1, pos.y - 1));

        //no black corner
        if(corner1 != color && corner2 != color && corner3 != color && corner4 != color)
        {
            //all corners are different colors
            if (corner1 != corner2 && corner1 != corner3 && corner1 != corner4 &&
            corner2 != corner3 && corner2 != corner4 &&
            corner3 != corner4)
            {
                return colorPurple;
            }

        }

        // Otherwise turn black.
        return color;
    }
}
