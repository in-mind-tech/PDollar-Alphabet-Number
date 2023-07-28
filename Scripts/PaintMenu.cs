using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public static int thePaintIndex;

    void Start()
    {
        thePaintIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPaintIndex(int index)
    {
        thePaintIndex = index;
    }

}
