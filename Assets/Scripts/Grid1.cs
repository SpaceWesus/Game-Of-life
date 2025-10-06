using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// IPointerClickHandler to receive OnPointerClick callbacks, requires eventsystems!
// (whenever we click on the object with script, it will call the OnPointerClick()
public class Grid1 : MonoBehaviour, IPointerClickHandler
{
    public int gridWidth = 100;
    public int gridHeight = 100;

    public Color[] colorPalette;

    public RawImage displayImage;
    public bool wrapEdges = true;

    public float stepsPerSecond = 10f;

    private byte[,] currentGrid;
    private byte[,] nextGrid;

    // Rendering data
    private Texture2D gridTexture;
    private Color32[] pixels;

    private byte selectedColorID = 1;

    // Simulation timing
    private float accum;
    private bool isPaused = true;

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

    [SerializeField] private ColorScript[] colors;

    #endregion

    void Start()
    {
        InitializeGrid();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) // 1 key
        {
            selectedColorID = 0; // Select Green
            Debug.Log("Selected Green");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2)) // 2 key
        {
            selectedColorID = 1; // Select Red
            Debug.Log("Selected Red");
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            isPaused = !isPaused;
        }

        if (!isPaused)
        {
            accum += Time.deltaTime;
            float stepInterval = 1f / stepsPerSecond;
            while (accum >= stepInterval)
            {
                Step();
                UploadFrame();
                accum -= stepInterval;
            }
        }
    }


    void InitializeGrid()
    {
        // Initialize the new byte arrays and the pixel buffer
        currentGrid = new byte[gridWidth, gridHeight];
        nextGrid = new byte[gridWidth, gridHeight];
        pixels = new Color32[gridWidth * gridHeight];

        // Create the texture that will display the grid
        gridTexture = new Texture2D(gridWidth, gridHeight);
        gridTexture.filterMode = FilterMode.Point;

        // Set the initial state to Off/Dead (ID 0)
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                currentGrid[x, y] = 0; // 0 = Off/Dead
            }
        }

        // Initial draw of the grid
        UploadFrame();

        // Assign the generated texture to our UI
        displayImage.texture = gridTexture;
    }

    void Step()
    {
        for (int y = 0; y < gridHeight; y++)
            for (int x = 0; x < gridWidth; x++)
                nextGrid[x, y] = CheckRules(currentGrid[x, y], new Vector2Int(x, y));

        // Swap the buffers for the next frame
        var temp = currentGrid;
        currentGrid = nextGrid;
        nextGrid = temp;
    }

    /*
    void Step()
    {
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                byte currentState = currentGrid[x, y];

                if (currentState == 2) // Rule for Red
                {
                    nextGrid[x, y] = 2;
                    continue;
                }

                int aliveNeighbors = 0;   // A count of all live neighbors of any color
                int greenNeighbors = 0;

                for (int dy = -1; dy <= 1; dy++)
                {
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        if (dx == 0 && dy == 0)
                        {
                            continue;
                        }

                        int nx = x + dx;
                        int ny = y + dy;

                        if (wrapEdges)
                        {
                            nx = (nx + gridWidth) % gridWidth;
                            ny = (ny + gridHeight) % gridHeight;
                        }
                        else if (nx < 0 || nx >= gridWidth || ny < 0 || ny >= gridHeight)
                        {
                            continue;
                        }

                        byte neighborState = currentGrid[nx, ny];

                        if (neighborState != 0)
                        {
                            aliveNeighbors++;
                        }

                        if (neighborState == 1)
                        {
                            greenNeighbors++;
                        }
                    }
                }

                byte nextState = currentState;

                //APPLY RULES
                if (currentState == 1) // Rule for live Green cells
                {
                    if (greenNeighbors < 2 || greenNeighbors > 3)
                    {
                        nextState = 0; // Dies
                    }
                }
                else if (currentState == 0) // Rule for dead cells
                {
                    if (greenNeighbors == 3)
                    {
                        nextState = 1; // Becomes green
                    }
                }

                nextGrid[x, y] = nextState;
            }
        }

        // Swap the buffers for the next frame
        var temp = currentGrid;
        currentGrid = nextGrid;
        nextGrid = temp;
    }
    */

    void UploadFrame()
    {
        int i = 0;
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                byte cellID = currentGrid[x, y];
                //pixels[i] = colorPalette[cellID];
                pixels[i] = colors[cellID].GetColor();
                i++;
            }
        }
        gridTexture.SetPixels32(pixels);
        gridTexture.Apply();
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

            
            byte currentCellState = currentGrid[gridX, gridY];
            byte newCellState = (currentCellState == 0) ? selectedColorID : (byte)0; // Toggle between colors

            currentGrid[gridX, gridY] = newCellState;

            // Update the single pixel on the texture for immediate visual feedback
            //gridTexture.SetPixel(gridX, gridY, colorPalette[newCellState]);
            gridTexture.SetPixel(gridX, gridY, colors[newCellState].GetColor());
            gridTexture.Apply();
        }
    }

    #region Donovan Rule Checking

    // Checks the rules for the given color and returns the ID corresponding to the resulting color.
    private byte CheckRules(byte id, Vector2Int pos)
    {
        byte resultByte = 0;
        Color resultColor = colors[id].CheckRules(pos);
        for (byte i = 0; i < colors.Length; i++)
            if (resultColor == colors[i].GetColor()) resultByte = i;
        return resultByte;
    }

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
    public Color GetColorAtPos(Vector2Int pos)
    {
        if (pos.x >= gridWidth) pos.x = 0;
        if (pos.x < 0) pos.x = gridWidth - 1;
        if (pos.y >= gridHeight) pos.y = 0;
        if (pos.y < 0) pos.y = gridHeight - 1;

        byte id = currentGrid[pos.x,pos.y];
        return colors[id].GetColor();
    }

    #endregion
}