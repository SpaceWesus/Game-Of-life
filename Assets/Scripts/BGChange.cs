using UnityEngine;
using UnityEngine.UI;

public class BGChange : MonoBehaviour
{
    public Sprite[] bgsprites;
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
            if(bgsprites.Length > 0)
            {
                currentIndex = (currentIndex + 1) % bgsprites.Length;
                gameObject.GetComponent<Image>().sprite = bgsprites[currentIndex];
            }
        }
    }
}
