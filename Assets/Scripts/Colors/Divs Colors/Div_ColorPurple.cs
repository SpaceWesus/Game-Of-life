using UnityEngine;

public class Div_ColorPurple : ColorScript
{
    public Color colorBlack;
    
    public override Color CheckRules(Vector2Int pos)
    {

        Grid grid = transform.parent.GetComponent<Grid>();

        Color[] safeColors = new Color[2];
        safeColors[0] = colorBlack;
        safeColors[1] = color;


        //if any of its neighbour is not black or itself, dies
        if(grid.CheckCount(pos, safeColors, 8, Grid.Neighbors.All, Grid.Operation.LessThan))
        {
            return colorBlack;
        }

        return color;
    }

    void Start()
    {
       //ebug.LogWarning("--- My name is " + gameObject.name + " and my color in the Inspector is " + color + " ---");
    }
}
