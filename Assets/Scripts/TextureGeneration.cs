using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class TextureGeneration : MonoBehaviour
{
    private bool toggled = false;

    private void Start()
    {
        byte[,] testMap = InitializeTestMap(10, 10);

        Texture2D testTex = GenerateTexture(testMap);

        SetTexture(testTex);
    }

    public void ToggleTestMaps()
    {
        byte[,] map;
        if (toggled)
        {
            map = InitializeTestMap(10, 10);
            toggled = false;
        }
        else
        {
            map = InitializeTestMap(100, 100);
            toggled = true;
        }
        Texture2D testTex = GenerateTexture(map);
        SetTexture(testTex);
    }

    private byte[,] InitializeTestMap(int w, int h)
    {
        byte[,] testMap = new byte[w, h];
        for(int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                if (i % 2 == 0 && j % 2 == 1) testMap[i, j] = 1;
                else testMap[i, j] = 0;
            }
        }
        return testMap;
    }

    private void SetTexture(Texture2D tex)
    {
        RawImage rawImage = GetComponent<RawImage>();
        //rawImage.rectTransform.sizeDelta = new Vector2(tex.width, tex.height);
        rawImage.texture = tex;
    }

    private Texture2D GenerateTexture(byte[,] tileMap)
    {
        Texture2D tex = new Texture2D(tileMap.GetLength(0), tileMap.GetLength(1));

        for (int x = 0; x < tileMap.GetLength(0); x++)
        {
            for (int y = 0; y < tileMap.GetLength(1); y++)
            {
                SetPixel(tex, x, y, tileMap[x, y]);
            }
        }

        tex.Apply();
        tex.filterMode = FilterMode.Point;

        return tex;
    }

    private void SetPixel(Texture2D tex, int x, int y, byte value)
    {
        switch (value)
        {
            case 0:
                tex.SetPixel(x, y, UnityEngine.Color.black);
                break;
            case 1:
                tex.SetPixel(x, y, UnityEngine.Color.white);
                break;
            default:
                tex.SetPixel(x, y, UnityEngine.Color.black);
                break;
        }
    }
}
