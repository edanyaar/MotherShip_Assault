using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Scene_Fade : MonoBehaviour
{
    public Button start;
    public Button quit;
    public Text title;
    public Button [] levels;
    public Color color = Color.black; 
    public float transition_time =1f;

    private string music_Event = "event:/Music/Menu_Theme";

    private  FMOD.Studio.EventInstance music_instance;


    private void Start()
    {
        music_instance = FMODUnity.RuntimeManager.CreateInstance(music_Event);
        music_instance.setParameterValue("Main_Menu",0);
        music_instance.start();

        PlayerPrefs.SetInt("gameState", 1); 
        PlayerPrefs.Save();

        for (int i = 0; i<levels.Length; i++){
            levels[i].gameObject.SetActive(false);
        }
        title.gameObject.SetActive(true);
        start.gameObject.SetActive(true);
        start.onClick.AddListener(startTaskOnClick);
        quit.onClick.AddListener(quitTaskOnClick);

        levels[1].onClick.AddListener(startLevel1);
        levels[2].onClick.AddListener(startLevel2);

    }
    private void startTaskOnClick()
    {
        title.gameObject.SetActive(false);
        start.gameObject.SetActive(false);
        for (int i = 1; i<levels.Length-1; i++){
            levels[i].gameObject.SetActive(true);
            levels[i].interactable = true;
            Text level_button_text = levels[i].GetComponentInChildren<Text>();
            string highScoreKey = "level" + i + "Score";
            int level_highscore =  PlayerPrefs.GetInt(highScoreKey,0);
            level_button_text.text = " level " + i + "      highscore: " + level_highscore;

            if(i > 1){
                string prevLvlScore = "level" + (i-1) + "Score";
                int prev_level_highscore =  PlayerPrefs.GetInt(prevLvlScore,0);
                if(prev_level_highscore==0){//prev level has not been completed yet
                    levels[i].interactable = false;
                }
            }
        }
    }


    private void startLevel1(){
        //set requested level field so the main scene knows which level to load
        PlayerPrefs.SetInt("Requested_Level", 1); 
        PlayerPrefs.Save();

        music_instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        Initiate.Fade("Main",color,transition_time);
    }

    private void startLevel2(){
        //set requested level field so the main scene knows which level to load
        PlayerPrefs.SetInt("Requested_Level", 2); 
        PlayerPrefs.Save();
        Initiate.Fade("Main",color,transition_time);
    }

    private void quitTaskOnClick(){
        Application.Quit();
    }
}
