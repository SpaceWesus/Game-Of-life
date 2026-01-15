using System.Collections.Generic;
using UnityEngine;

public class RandomInitSettings : MonoBehaviour
{
    public static RandomInitSettings I;
    // new function that overrides old random init function 
    public bool overrideRandomInit = false;
    public List<int> weights = new List<int>();

    void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    public static int GetWeight(int index, int fallback)
    {
        if (I != null && I.overrideRandomInit && index < I.weights.Count)
            return Mathf.Max(0, I.weights[index]);
        return fallback;
    }
}
