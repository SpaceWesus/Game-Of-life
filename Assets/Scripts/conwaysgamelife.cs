using UnityEngine;

/// attach this to a single empty GameObject and press Play.
/// controls:
///  - Space: pause/resume
///  - R: randomize
///  - Left click: toggle a cell
///  - +/- : speed up / slow down (or = and - on some keyboards)
public class LifeOfConway : MonoBehaviour
{
    [Header("Grid")]
    public int width = 128;
    public int height = 128;
    public bool wrapEdges = true;

    [Header("Timing (steps per second)")]
    public float stepsPerSecond = 10f;

    [Header("Colors")]
    public Color aliveColor = Color.white;
    public Color deadColor = Color.black;

    // --- simulation state (2D for simplicity) ---
    private byte[,] current;
    private byte[,] next;

    // --- rendering ---
    private Texture2D tex;
    private Color32[] pixels;       // 1D buffer to upload to the texture
    private SpriteRenderer sr;

    // timing
    private float accum;
    private bool paused;

    void Start()
    {
        // camera setup so the sprite fills the view nicely
        var cam = Camera.main;
        if (cam == null)
        {
            var camGO = new GameObject("Main Camera");
            cam = camGO.AddComponent<Camera>();
            cam.tag = "MainCamera";
        }
        cam.orthographic = true;
        cam.backgroundColor = Color.black;

        // create a sprite renderer to show the texture
        sr = gameObject.AddComponent<SpriteRenderer>();

        // create texture
        tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point; // crisp pixels

        // create a sprite from the texture and assign
        var sprite = Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 1f);
        sr.sprite = sprite;

        // scale sprite so it fits the camera view
        FitSpriteToCamera(cam);

        // alloc sim + pixel buffers
        current = new byte[width, height];
        next = new byte[width, height];
        pixels = new Color32[width * height];

        // start with a random field (feel free to comment this line)
        Randomize();
        UploadFrame();
    }

    void Update()
    {
        // input
        if (Input.GetKeyDown(KeyCode.Space)) paused = !paused;
        if (Input.GetKeyDown(KeyCode.R)) { Randomize(); UploadFrame(); }
        if (Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.Plus)) stepsPerSecond = Mathf.Min(stepsPerSecond + 1, 120);
        if (Input.GetKeyDown(KeyCode.Minus)) stepsPerSecond = Mathf.Max(1, stepsPerSecond - 1);

        // click to toggle a cell
        if (Input.GetMouseButton(0))
        {
            if (ScreenToCell(Input.mousePosition, out int x, out int y))
            {
                current[x, y] = (byte)(current[x, y] == 0 ? 1 : 0);
                // update just that pixel immediately
                SetPixel(x, y, current[x, y] == 1 ? aliveColor : deadColor);
                tex.SetPixels32(pixels);
                tex.Apply(false, false);
            }
        }

        // step the simulation on a fixed rate
        if (!paused)
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

    // --- simulation: classic Game of Life ---
    void Step()
    {
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                int alive = 0;
                for (int dy = -1; dy <= 1; dy++)
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        if (dx == 0 && dy == 0) continue;
                        int nx = x + dx, ny = y + dy;
                        if (wrapEdges)
                        {
                            nx = (nx + width) % width;
                            ny = (ny + height) % height;
                        }
                        else
                        {
                            if (nx < 0 || nx >= width || ny < 0 || ny >= height) continue;
                        }
                        if (current[nx, ny] == 1) alive++;
                    }

                byte s = current[x, y];
                byte outS = s == 1
                    ? (byte)((alive == 2 || alive == 3) ? 1 : 0)
                    : (byte)(alive == 3 ? 1 : 0);

                next[x, y] = outS;
            }

        // swap buffers
        var temp = current; current = next; next = temp;
    }

    // --- rendering helpers ---
    void UploadFrame()
    {
        // map 2D state -> 1D pixel buffer
        int i = 0;
        Color32 alive = aliveColor;
        Color32 dead = deadColor;
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++, i++)
            {
                pixels[i] = (current[x, y] == 1) ? alive : dead;
            }
        tex.SetPixels32(pixels);
        tex.Apply(false, false);
    }

    void SetPixel(int x, int y, Color c)
    {
        int i = x + y * width;
        pixels[i] = (Color32)c;
    }

    void Randomize(float aliveChance = 0.35f)
    {
        var rng = new System.Random();
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                current[x, y] = (byte)(rng.NextDouble() < aliveChance ? 1 : 0);
            }
    }

    // convert screen mouse position → cell coords on the sprite
    bool ScreenToCell(Vector3 screenPos, out int cx, out int cy)
    {
        cx = cy = 0;
        var cam = Camera.main;
        var world = cam.ScreenToWorldPoint(screenPos);
        var local = transform.InverseTransformPoint(world);

        // sprite is centered at (0,0), with size (width,height) in local units (pixels per unit = 1)
        float halfW = width * 0.5f;
        float halfH = height * 0.5f;

        float px = local.x + halfW;
        float py = local.y + halfH;

        if (px < 0 || py < 0 || px >= width || py >= height) return false;

        cx = Mathf.FloorToInt(px);
        cy = Mathf.FloorToInt(py);
        return true;
    }

    // scale sprite to fit the camera view neatly
    void FitSpriteToCamera(Camera cam)
    {
        // make the sprite's scale 1 pixel = 1 unit, then size camera to see full texture
        transform.position = Vector3.zero;

        // match camera size to show the whole texture with a small border
        float aspect = (float)Screen.width / Screen.height;
        float texAspect = (float)width / height;

        if (texAspect >= aspect)
        {
            // width-limited: set camera size from height
            cam.orthographicSize = (height * 0.5f) + 1f;
        }
        else
        {
            // height-limited: compute size from width and screen aspect
            float neededHalfHeight = (width / aspect) * 0.5f;
            cam.orthographicSize = neededHalfHeight + 1f;
        }
    }
}
