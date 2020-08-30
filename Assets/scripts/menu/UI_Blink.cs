using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Blink : MonoBehaviour
{

  //get the Text component
  public Text flashingText;
   
  void Start(){
    //Call coroutine BlinkText on Start
    StartCoroutine(BlinkText());
  }
 
  //function to blink the text
  public IEnumerator BlinkText(){
  //blink it forever. You can set a terminating condition depending upon your requirement
  while(true){
    //set the Text's text to blank
    flashingText.text= "";
    //display blank text for 0.5 seconds
    yield return new WaitForSeconds(0.5f);
    //display “Play” for the next 0.5 seconds
    flashingText.text= "Play";
    yield return new WaitForSeconds(1f);
    }
  }
 
}

