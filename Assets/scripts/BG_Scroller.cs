using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BG_Scroller : MonoBehaviour
{

    public float scrollSpeed;
    public float tileSize;

    private Vector3 startPosition;

    void Start(){
        startPosition = transform.position;
    }

    void Update()
    {
      float newPosition = Mathf.Repeat(Time.time*scrollSpeed,tileSize);  
      transform.position = startPosition + (Vector3.forward * newPosition);
    }
}
