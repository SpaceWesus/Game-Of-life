using UnityEngine;
using UnityEngine.UI;

public class TilePainter : MonoBehaviour
{
    private RawImage rawImage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rawImage = GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Clicked()
    {
        Texture tex = rawImage.texture;

        Vector3 mousePos = Input.mousePosition;
        Debug.Log("Mouse Position: " + mousePos);

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Camera.main.transform.position.z * -1));
        Debug.Log("World Position: " + worldPos);

        //Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        //Debug.Log("Screen Size: " + screenSize);

        //Vector2 texSize = new Vector2(tex.width, tex.height);
        //Debug.Log("Texture Size: " + texSize);

        // texPos = rawImage.transform.position;
        //Debug.Log("Texture Position: " + texPos);

    }
}
