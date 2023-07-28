using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintButton : MonoBehaviour
{
    // Start is called before the first frame update

    public int index;// Boolean variable to control scaling
    private Vector3 originalScale;
    private Vector3 targetScale;

    void Start()
    {
    
        originalScale = transform.localScale; // Store the original scale
        targetScale = originalScale * 1.7f; // Calculate the target scale

    }

    // Update is called once per frame
    void Update() {
        

            if (index == PaintMenu.thePaintIndex){
                transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * 5f);
            }else{
                transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * 5f);
            }
       
    }
}
