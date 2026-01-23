using UnityEngine;
using UnityEngine.UI;

public class BGChange : MonoBehaviour
{
    public Sprite[] bgSprites;
    private int currentIndex = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            if(bgSprites.Length > 0)
            {
                currentIndex = (currentIndex + 1) % bgSprites.Length;
                gameObject.GetComponent<Image>().sprite = bgSprites[currentIndex];
            }
        }
    }
}
