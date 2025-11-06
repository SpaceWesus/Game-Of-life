using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using NUnit.Framework;
using System.Collections.Generic;
using TMPro;

// IPointerClickHandler to receive OnPointerClick callbacks, requires eventsystems!
// (whenever we click on the object with script, it will call the OnPointerClick()
public class Grid : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridWidth = 100;
    public int gridHeight = 100;
    public bool wrapEdges = true;
    public bool randomInit = false;
    public float stepsPerSecond = 10f;

    [Header("References")]
    public Renderer gridRenderer;

    // Grid Processing
    private ColorScript[] colors;
    private byte[,] currentGrid;
    private byte[,] nextGrid;

    // Rendering data
    private Texture2D gridTexture;
    private Color32[] pixels;

    // Color Selection
    private Image selectedColorIndicator = null;
    public byte selectedColorID = 1;

    // Simulation Timing
    private float accum;
    private bool isPaused = true;

    //UI Text
    public TMP_Text roundText;
    private int roundCounter = 0;

    #region Donovan Rule Checking Enums

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

    void Start()
    {
        InitializeGrid();

        if (GameObject.Find("Selected Color Indicator"))
            selectedColorIndicator = GameObject.Find("Selected Color Indicator").GetComponent<Image>();

        if (selectedColorIndicator != null) selectedColorIndicator.color = colors[selectedColorID].GetColor();
    }

    void Update()
    {
        // Scroll mouse wheel to swap colors.
        if (Input.GetKeyDown(KeyCode.E))
        {
            selectedColorID--;
            if (selectedColorID > colors.Length) selectedColorID = (byte)(colors.Length - 1);
            if (selectedColorIndicator != null) selectedColorIndicator.color = colors[selectedColorID].GetColor();
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            selectedColorID++;
            if (selectedColorID >= colors.Length) selectedColorID = 0;
            if (selectedColorIndicator != null) selectedColorIndicator.color = colors[selectedColorID].GetColor();
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

        HandleMousePaint();

    }

    // Initializes the grid to all of whatever the first color in the Colors list is.
    void InitializeGrid()
    {
        // Initialize the colors array.
        colors = transform.GetComponentsInChildren<ColorScript>();

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
                //if (randomInit) currentGrid[x, y] = (byte) Random.Range(0, colors.Length);
                if (randomInit) currentGrid[x, y] = PickRandomColor();
                else currentGrid[x, y] = 0; // Default Color
            }
        }

        // Initial draw of the grid
        UploadFrame();

        // Assign the generated texture to our UI
        gridRenderer.material.mainTexture = gridTexture;
    }

    private byte PickRandomColor()
    {
        List<ColorScript> colorPool = new List<ColorScript>();

        for (int i = 0; i < colors.Length; i++)
            for (int j = 0; j < colors[i].GetRandomInitWeight(); j++) 
                colorPool.Add(colors[i]);

        int selection = Random.Range(0, colorPool.Count);
        ColorScript selectedColor = colorPool[selection];
        for (byte i = 0; i < colors.Length; i++)
            if (colors[i] == selectedColor) return i;

        return 0;
    }

    // Loops through every cell in the grid and updates it based on its color's rules.
    void Step()
    {
        for (int y = 0; y < gridHeight; y++)
            for (int x = 0; x < gridWidth; x++)
                nextGrid[x, y] = CheckRules(currentGrid[x, y], new Vector2Int(x, y));

        // Swap the buffers for the next frame
        var temp = currentGrid;
        currentGrid = nextGrid;
        nextGrid = temp;

        //keep track of rounds for ui
        roundCounter++;
    }

    // Renders the cell colors to the canvas.
    void UploadFrame()
    {
        int i = 0;
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                byte cellID = currentGrid[x, y];
                pixels[i] = colors[cellID].GetColor();
                i++;
            }
        }
        gridTexture.SetPixels32(pixels);
        gridTexture.Apply();

        roundText.text = "Round: " + roundCounter;
    }


    // Handles clicking on the canvas to color cells.
    void HandleMousePaint()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Ensure we clicked our GridQuad
                if (hit.collider.GetComponent<Renderer>() == gridRenderer)
                {
                    Vector2 uv = hit.textureCoord; // normalized 0–1 position on texture

                    int gridX = Mathf.FloorToInt(uv.x * gridWidth);
                    int gridY = Mathf.FloorToInt(uv.y * gridHeight);

                    byte newState = selectedColorID;

                    currentGrid[gridX, gridY] = newState;
                    gridTexture.SetPixel(gridX, gridY, colors[newState].GetColor());
                    gridTexture.Apply();
                }
            }
        }
    }

    public ColorScript GetSelectColorID()
    {
        if (colors != null)
        {
            //Debug.Log("selected" + colors[selectedColorID]);
            return colors[selectedColorID];
        }
        else
        {
            return null;
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