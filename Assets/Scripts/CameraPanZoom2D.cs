using UnityEngine;

public class CameraPanZoom2D : MonoBehaviour
{
    [Header("Targets")]
    public Camera cam;
    public Renderer gridRenderer;              // If using Quad/Plane (recommended)
    public RectTransform gridRectTransform;    // If using World-Space Canvas (optional)

    [Header("Zoom")]
    public float minSize = 1.5f;
    public float maxSize = 60f;
    public float zoomFactorPerNotch = 0.9f;    // <1 = zoom in per notch; >1 = zoom out
    public float zoomSpeedMultiplier = 1.0f;   // mouse wheel sensitivity

    [Header("Pan")]
    public float panSpeed = 1.0f;              // drag sensitivity
    public int panMouseButton = 1;             // 1 = Right Mouse Button

    private Rect worldBounds;                  // computed from grid
    private bool dragging;
    private Vector3 lastMouseScreen;

    [Header("Clamping")]
    public bool clampToBounds = false;
    public float clampMargin = 2f; // world units you can 'hang' past the edges


    void Reset()
    {
        cam = GetComponent<Camera>();
        if (cam) cam.orthographic = true;
    }

    void Start()
    {
        RecomputeBounds();
        ClampCamera();
    }

    void Update()
    {
        if (!cam) return;

        // --- Zoom toward cursor ---
        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0.0001f)
        {
            Vector3 before = cam.ScreenToWorldPoint(Input.mousePosition);
            float factor = Mathf.Pow(zoomFactorPerNotch, scroll * zoomSpeedMultiplier);
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize * factor, minSize, maxSize);
            Vector3 after = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 delta = before - after; // shift so the point under cursor stays fixed
            transform.position += delta;
            
            if (clampToBounds)
            { 
                ClampCamera();
            }
        }

        // --- Right-click drag pan ---
        if (Input.GetMouseButtonDown(panMouseButton))
        {
            dragging = true;
            lastMouseScreen = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(panMouseButton)) dragging = false;

        if (dragging)
        {
            Vector3 currScreen = Input.mousePosition;
            Vector3 deltaScreen = currScreen - lastMouseScreen;

            // Convert screen pixels → world units for this ortho camera
            float worldPerScreenY = 2f * cam.orthographicSize / Screen.height;
            float worldPerScreenX = worldPerScreenY * cam.aspect;

            Vector3 deltaWorld = new Vector3(
                -deltaScreen.x * worldPerScreenX,
                -deltaScreen.y * worldPerScreenY,
                0f
            );

            transform.position += deltaWorld;
            lastMouseScreen = currScreen;

            if (clampToBounds)
            {
                ClampCamera();
            }
        }
        
    }

    public void RecomputeBounds()
    {
        if (gridRenderer)
        {
            Bounds b = gridRenderer.bounds;
            worldBounds = new Rect(
                b.min.x, b.min.y,
                b.size.x, b.size.y
            );
        }
        else if (gridRectTransform)
        {
            Vector3[] corners = new Vector3[4];
            gridRectTransform.GetWorldCorners(corners);
            float minX = corners[0].x, maxX = corners[2].x;
            float minY = corners[0].y, maxY = corners[2].y;
            worldBounds = new Rect(minX, minY, maxX - minX, maxY - minY);
        }
        else
        {
            // Fallback: infinite bounds
            worldBounds = new Rect(-1e6f, -1e6f, 2e6f, 2e6f);
        }
    }

    private void ClampCamera()
    {
        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;

        float boundsWidth  = worldBounds.width;
        float boundsHeight = worldBounds.height;
        Vector2 boundsCenter = worldBounds.center;

        Vector3 pos = transform.position;

        // If view is wider than bounds, pin to center on X (no oscillation)
        if (2f * halfW >= boundsWidth - 1e-4f)
            pos.x = boundsCenter.x;
        else
            pos.x = Mathf.Clamp(pos.x, worldBounds.xMin + halfW, worldBounds.xMax - halfW);

        // If view is taller than bounds, pin to center on Y
        if (2f * halfH >= boundsHeight - 1e-4f)
            pos.y = boundsCenter.y;
        else
            pos.y = Mathf.Clamp(pos.y, worldBounds.yMin + halfH, worldBounds.yMax - halfH);

        transform.position = pos;
    }

}
