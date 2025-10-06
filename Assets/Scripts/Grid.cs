using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// IPointerClickHandler to receive OnPointerClick callbacks, requires eventsystems!
// (whenever we click on the object with script, it will call the OnPointerClick()
public class Grid : MonoBehaviour, IPointerClickHandler
{
    
    public int gridWidth = 100;
    public int gridHeight = 100;

    #region Donovan Rule Checking

    public enum Neighbors 
    { 
        All,
        Adjacent,
        Corner
    }

    public enum Operation
    {
        Equal,
        NotEqual,
        LessThan,
        GreaterThan,
        LessThanEqual,
        GreaterThanEqual
    }

    #endregion

    public RawImage displayImage;

    private Texture2D gridTexture;
    private Color[,] gridData;

    void Start()
    {
        InitializeGrid();
    }

    void InitializeGrid()
    {
        // Initialize the data array
        gridData = new Color[gridWidth, gridHeight];

        // Create the texture that will display grid
        gridTexture = new Texture2D(gridWidth, gridHeight);

        // filter mode point to not have blurring
        gridTexture.filterMode = FilterMode.Point;

        // Fill the data and texture with off/dead cell color
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                Color initialColor = Color.black;
                gridData[x, y] = initialColor;
                gridTexture.SetPixel(x, y, initialColor);
            }
        }

        // Apply all SetPixel ^above^ changes to the texture, without apply the changes arent visible.
        gridTexture.Apply();

        // Assign the generated texture to our UI
        displayImage.texture = gridTexture;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        RectTransform imageRect = displayImage.rectTransform;

        //eventdata position would give us pixel based position and it would consider the whole screen, not just canvas
        // Convert the screen-space click position to a local position within the RawImage
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(imageRect, eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
        {
            // The localPoint's origin is at the center of the image by default
            // so we need to adjust it so (0,0) is at the bottom left corner

            float pivotX = imageRect.pivot.x * imageRect.rect.width; //the pivot would be half (center of img) * width,
                                                                     //would give us how far the center is from the edge

            float pivotY = imageRect.pivot.y * imageRect.rect.height;

            localPoint.x += pivotX; //converts the click coords by offset of the center.
                                    //(if center is 50 away from edge, this does -50 + 50) so the edge now has x = 0

            localPoint.y += pivotY;

            // Normalize the coordinates to a 0-1 range (Math!) (so the rawimage size doesnt affect the grid ccords)

            float normalizedX = Mathf.Clamp01(localPoint.x / imageRect.rect.width); //local point should now be (0,0) at bottom left / actual width // clamp01 for safety
            float normalizedY = Mathf.Clamp01(localPoint.y / imageRect.rect.height);

            // Scale the normalized coordinates to grid coordinates
            int gridX = Mathf.FloorToInt(normalizedX * gridWidth); //if normalizedX is .687 and width 100, we get x cord to be 68 (int)
            int gridY = Mathf.FloorToInt(normalizedY * gridHeight);

            Debug.Log($"Clicked Grid Position: ({gridX}, {gridY})");

            //update the color at that position
            ChangeCellColor(gridX, gridY, Color.white);
        }
    }

    public void ChangeCellColor(int x, int y, Color newColor)
    {
        // Update the grid array
        gridData[x, y] = newColor;

        // Update tthe texture
        gridTexture.SetPixel(x, y, newColor);

        // Apply the change.
        gridTexture.Apply();
    }

    #region Donovan Rule Checking

    // Compares the total number of tiles of a certain list of colors to a target amount, using a designated operation.
    public bool CheckCount(Vector2Int currentPos, Color[] targetColors, int targetCount, Neighbors targetNeighbors, Operation op)
    {
        int total = 0;

        // Check adjacent neighbors.
        if (targetNeighbors == Neighbors.All || targetNeighbors == Neighbors.Adjacent)
        {
            Vector2Int xPlus = new Vector2Int(currentPos.x + 1, currentPos.y);
            Vector2Int xMinus = new Vector2Int(currentPos.x - 1, currentPos.y);
            Vector2Int yPlus = new Vector2Int(currentPos.x, currentPos.y + 1);
            Vector2Int yMinus = new Vector2Int(currentPos.x, currentPos.y - 1);

            if (CheckPosition(targetColors, xPlus)) total++;
            if (CheckPosition(targetColors, xMinus)) total++;
            if (CheckPosition(targetColors, yPlus)) total++;
            if (CheckPosition(targetColors, yMinus)) total++;
        }

        // Check corner neighbors.
        if (targetNeighbors == Neighbors.All || targetNeighbors == Neighbors.Corner)
        {
            Vector2Int xPyP = new Vector2Int(currentPos.x + 1, currentPos.y + 1);
            Vector2Int xMyP = new Vector2Int(currentPos.x - 1, currentPos.y + 1);
            Vector2Int xPyM = new Vector2Int(currentPos.x + 1, currentPos.y - 1);
            Vector2Int xMyM = new Vector2Int(currentPos.x - 1, currentPos.y - 1);

            if (CheckPosition(targetColors, xPyP)) total++;
            if (CheckPosition(targetColors, xMyP)) total++;
            if (CheckPosition(targetColors, xPyM)) total++;
            if (CheckPosition(targetColors, xMyM)) total++;
        }

        // Compare the total number of target colors found to the target count using the designated operation.
        switch (op)
        {
            case Operation.Equal:
                if (total == targetCount) return true;
                else return false;
            case Operation.NotEqual:
                if (total != targetCount) return true;
                else return false;
            case Operation.GreaterThan:
                if (total > targetCount) return true;
                else return false;
            case Operation.LessThan:
                if (total < targetCount) return true;
                else return false;
            case Operation.GreaterThanEqual:
                if (total >= targetCount) return true;
                else return false;
            case Operation.LessThanEqual:
                if (total <= targetCount) return true;
                else return false;
            default:
                return false;
        }
    }

    // Checks if the color at a certain position is within the targetColors list.
    public bool CheckPosition(Color[] targetColors, Vector2Int targetPos)
    {
        if (ColorInList(targetColors, GetColorAtPos(targetPos))) return true;
        else return false;
    }

    // Checks if a color is within a list of colors.
    public bool ColorInList(Color[] colorList, Color targetColor)
    {
        foreach (Color color in colorList)
            if (color == targetColor) return true;

        return false;
    }

    // Gets the color at a given position of the previous frame.
    // TODO: Add color getting.
    public Color GetColorAtPos(Vector2Int pos)
    {
        return new Color(0, 0, 0, 0);
    }

    #endregion
}