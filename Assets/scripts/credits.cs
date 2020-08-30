using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class credits : MonoBehaviour
{
    public int credits_length;
    // Start is called before the first frame update
    void Start()
    {
        credits_helper();
    }

    void credits_helper(){
        StartCoroutine(credits_wait());
    }
   
   IEnumerator credits_wait(){
       yield return new WaitForSeconds(credits_length);
        Initiate.Fade("Menu",Color.black,1f);
   }
}
