using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject pauseMenuRoot;

    [Header("References")]
    [SerializeField] private Grid grid;                  // your simulation controller
    [SerializeField] private GameObject hudRoot;

    private bool isOpen;

    void Awake()
    {
        if (!pauseMenuRoot) pauseMenuRoot = gameObject;
        if (!grid) grid = FindFirstObjectByType<Grid>();
        pauseMenuRoot.SetActive(false);
        isOpen = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isOpen) Resume();
            else Pause();
        }
    }

    public void Pause()
    {
        Debug.Log("Pause MENU OPENED. Pause menu controller");
        
        isOpen = true;
        pauseMenuRoot.SetActive(true);

        if (hudRoot) hudRoot.SetActive(false);
        if (grid) grid.SetPauseMenuOpen(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Resume()
    {
        isOpen = false;
        pauseMenuRoot.SetActive(false);

        if (hudRoot) hudRoot.SetActive(true);
        if (grid) grid.SetPauseMenuOpen(false);
    }

    public void ExitToMenu()
    {
        // Ensure state is clean
        if (grid) grid.SetPauseMenuOpen(false);
        SceneManager.LoadScene(0);
    }
}
