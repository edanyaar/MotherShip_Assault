using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class game_controller : MonoBehaviour
{
    public GameObject playerExplosion;
    public GameObject [] hazards;
   
    public float start_delay; //time to wait from begining of game to first astroid
    public float hazard_spawn_delay; //time in between hazards
    public float wave_delay; //time in between waves

    public int [] wave_hazard_nums;
    public int [] lvl_2_wave_nums;

    public bool boss_encounter;

    public Text score_Text;
    public Text fin_score_text;
    private int score;

    public float max_life;
    public float max_mana;
    public float hazard_damage;
    public float hazard_mana_gain;
    public float healthPack_val;
    public float shield_duration;
    public float weapon_upgrade_duration;

    public Text gameOverText;
    public Text restartText;

    public GameObject pauseText;
    public GameObject pausePanel;
    private bool restartflag;


    public GameObject MinesText;
    public GameObject WavesText;
    public GameObject healthPackText;
    public GameObject energyPackText;

    public GameObject wasd;
    public GameObject spaceBar;
    public GameObject lmb;

    public GameObject [] help_icons;

    public GameObject siren;
    private FMODUnity.StudioEventEmitter mines_emitter;
    public GameObject mines_alert;
    private FMODUnity.StudioEventEmitter mines_dialogue_emitter;
    public GameObject healthpack_alert;
    private FMODUnity.StudioEventEmitter healthpack_emitter;
    public GameObject mothership_alert;
    private FMODUnity.StudioEventEmitter mothership_emitter;
    public GameObject energycells_alert;
    private FMODUnity.StudioEventEmitter energycells_emitter;


    private string main_Theme_Event = "event:/Music/Main_theme";

    private FMOD.Studio.EventInstance main_theme;

    private int gameState; // 1:single player (default) 2:cooperative players 3:competative players
    private int level; // the current level
    private bool level_complete;

    public GameObject [] players;
    public GameObject [] bonus;
    public GameObject [] backgrounds;

    
    void Start(){
        main_theme = FMODUnity.RuntimeManager.CreateInstance(main_Theme_Event);    
        main_theme.start();
        main_theme.setParameterValue("Bass",1f);
        main_theme.setParameterValue("Organ",1f);

        gameState = PlayerPrefs.GetInt("gameState",1);
        level = PlayerPrefs.GetInt("Requested_Level",0);
        score = 0; 
        level_complete = false;
        boss_encounter = false;
        restartflag = false;
        MinesText.SetActive(false);
        pausePanel.SetActive(false);
        healthPackText.SetActive(false);
        GameObject mb = GameObject.FindGameObjectWithTag ("ManaBar");
        SimpleHealthBar manaBar = mb.GetComponent<SimpleHealthBar>();
        manaBar.UpdateBar(100,max_mana);
        mines_emitter = siren.GetComponent<FMODUnity.StudioEventEmitter>();
        mines_dialogue_emitter = mines_alert.GetComponent<FMODUnity.StudioEventEmitter>();
        healthpack_emitter = healthpack_alert.GetComponent<FMODUnity.StudioEventEmitter>();
        mothership_emitter = mothership_alert.GetComponent<FMODUnity.StudioEventEmitter>();
        energycells_emitter = energycells_alert.GetComponent<FMODUnity.StudioEventEmitter>();

        string engine_noise_event = "event:/Background/Engine";
        FMOD.Studio.EventInstance engine_noise = FMODUnity.RuntimeManager.CreateInstance(engine_noise_event);
        engine_noise.start();
 
        updateScore();

        switch (level){
            case 1:
                StartCoroutine(spawnWaveslvl1());
                break;
            default:
                StartCoroutine(spawnWaveslvl1());
                break;
        }
    }

    void Update(){

        //restart handeling
        if(restartflag || pausePanel.activeInHierarchy){
            if(Input.GetKeyDown(KeyCode.R)){
                main_theme.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                ContinueGame();
                SceneManager.LoadScene("Main");
            }
        }

        //next level handeling
        if(level_complete){
            if(Input.GetKeyDown(KeyCode.C)){
                //PlayerPrefs.SetInt("Requested_Level", level+1);
                //PlayerPrefs.Save(); 
                main_theme.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                ContinueGame();
                Initiate.Fade("Menu",Color.black,1f);
            }
        }

        //pause handeling
        if(!restartflag){
            if(Input.GetKeyDown (KeyCode.Escape)) 
            {
                if (!pausePanel.activeInHierarchy){ 
                    PauseGame();
                }
                else{ 
                    ContinueGame();   
                }
            } 
        }
        //quit handeling
        if(Input.GetKeyDown(KeyCode.Q)){
            if (restartflag || pausePanel.activeInHierarchy){
               if (pausePanel.activeInHierarchy){
                    ContinueGame();   
               }
               main_theme.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
               Initiate.Fade("Menu",Color.black,1f);
            }
        }
    }

    IEnumerator spawnWaveslvl1(){
        backgrounds[1].SetActive(true);
        backgrounds[2].SetActive(false);

        if (getGameMode() == 1){
            for (int i = 0; i<8; i++){
                help_icons[i].SetActive(true);
            }
        }
        else if (getGameMode() == 2){
            for (int i = 8; i<18; i++){
                help_icons[i].SetActive(true);
            }
            yield return new WaitForSeconds (start_delay);
        }

        yield return new WaitForSeconds (start_delay);

        if (getGameMode() == 1){
            for (int i = 0; i<8; i++){
                help_icons[i].SetActive(false);
            }
        }
        else if (getGameMode() == 2){
            for (int i = 8; i<18; i++){
                help_icons[i].SetActive(false);
            }
        }

        int wave = 1;

        main_theme.setParameterValue("Drums",1f);

        //wave 1
        for(int i = 0; i<wave_hazard_nums[wave]; i++){
            GameObject hazard = hazards[Random.Range(0,3)];
            Vector3 spawnPosition = new Vector3 (Random.Range(-14.2f,14.2f),0,20);
            Quaternion spawnRotation = Quaternion.identity;
            Instantiate(hazard,spawnPosition,spawnRotation);
            yield return new WaitForSeconds (hazard_spawn_delay*2);
        }

        wave++;

        

        yield return new WaitForSeconds (1.5f);
        WavesText.GetComponent<Text>().text = "Wave 1 Complete!";
        yield return new WaitForSeconds (2.8f);
        WavesText.GetComponent<Text>().text = "";
        main_theme.setParameterValue("Synth Main",1f);
        yield return new WaitForSeconds (1);


        //wave 2

        for(int i = 0; i<wave_hazard_nums[wave]; i++){
            GameObject hazard = hazards[Random.Range(0,3)];
            Vector3 spawnPosition = new Vector3 (Random.Range(-14.2f,14.2f),0,20);
            Quaternion spawnRotation = Quaternion.identity;
            Instantiate(hazard,spawnPosition,spawnRotation);
            yield return new WaitForSeconds (hazard_spawn_delay);
        }
        wave++;
        yield return new WaitForSeconds (3.5f);
        WavesText.GetComponent<Text>().text = "Wave 2 Complete!";
        yield return new WaitForSeconds (2.0f);
        WavesText.GetComponent<Text>().text = "";
        yield return new WaitForSeconds (1);


        //wave 3
        for(int i = 0; i<wave_hazard_nums[wave]; i++){
            GameObject hazard;
            if(i%8 == 0)
                hazard = hazards[3];
            else 
                hazard = hazards[Random.Range(0,3)];
            Vector3 spawnPosition = new Vector3 (Random.Range(-14.2f,14.2f),0,20);
            Quaternion spawnRotation = Quaternion.identity;
            Instantiate(hazard,spawnPosition,spawnRotation);
            yield return new WaitForSeconds (hazard_spawn_delay);
        }

        wave++;
        yield return new WaitForSeconds (3f);
        main_theme.setParameterValue("Organ",0f);
        main_theme.setParameterValue("Synth Main",0f);
        yield return new WaitForSeconds (1f);
        MinesText.gameObject.SetActive(true);
        mines_emitter.Play();
        yield return new WaitForSeconds (4);
        mines_dialogue_emitter.Play();
        yield return new WaitForSeconds (wave_delay);
        MinesText.gameObject.SetActive(false);

        //wave 4 
        for(int i = 0; i<wave_hazard_nums[wave]; i++){
            Vector3 spawnPosition = new Vector3 (Random.Range(-14.2f,14.2f),0,20);
            Quaternion spawnRotation = Quaternion.identity;
            Instantiate(hazards[6],spawnPosition,spawnRotation);
            yield return new WaitForSeconds (hazard_spawn_delay);
        }


        wave++;
        yield return new WaitForSeconds (1.4f);
        main_theme.setParameterValue("Organ",1f);
        main_theme.setParameterValue("Synth Main",1f);
        yield return new WaitForSeconds (2.0f);
        healthPackText.SetActive(true);
        healthpack_emitter.Play();
        yield return new WaitForSeconds (3);
        healthPackText.SetActive(false);
        Instantiate(bonus[1],new Vector3(Random.Range(-14.2f,14.2f),0,20),Quaternion.identity);
        yield return new WaitForSeconds (wave_delay);

        //wave 5
        for(int i = 0; i<wave_hazard_nums[wave]; i++){
            GameObject hazard;
            if(i%6 == 0)
                hazard = hazards[3];
            else 
                hazard = hazards[Random.Range(0,3)];
            Vector3 spawnPosition = new Vector3 (Random.Range(-14.2f,14.2f),0,20);
            Quaternion spawnRotation = Quaternion.identity;
            Instantiate(hazard,spawnPosition,spawnRotation);
            yield return new WaitForSeconds (hazard_spawn_delay);
        }

        wave++;
         yield return new WaitForSeconds (3.25f);
        WavesText.GetComponent<Text>().text = "Wave 5 Complete!";
        yield return new WaitForSeconds (2.0f);
        WavesText.GetComponent<Text>().text = "";
        yield return new WaitForSeconds (1);

        //wave 6
        for(int i = 0; i<wave_hazard_nums[wave]; i++){
            GameObject hazard;
            if(i%14 == 0)
                hazard = hazards[4];
            else if(i%7 == 0){
                hazard = hazards[3];
            }    
            else 
                hazard = hazards[Random.Range(0,3)];
            Vector3 spawnPosition = new Vector3 (Random.Range(-14.2f,14.2f),0,20);
            Quaternion spawnRotation = Quaternion.identity;
            Instantiate(hazard,spawnPosition,spawnRotation);
            yield return new WaitForSeconds (hazard_spawn_delay);
        }

        wave++;
         yield return new WaitForSeconds (3.25f);
        WavesText.GetComponent<Text>().text = "Wave 6 Complete!";
        yield return new WaitForSeconds (2);
        WavesText.GetComponent<Text>().text = "";
        yield return new WaitForSeconds (1);

        //wave 7 
        for(int i = 0; i<wave_hazard_nums[wave]; i++){
            GameObject hazard;
            if(i%14 == 0){
                hazard = hazards[3];
            }
            else if (i%7 == 0){
                hazard = hazards[6];
            }
            else{
                hazard = hazards[Random.Range(0,3)];
            }
            Vector3 spawnPosition = new Vector3 (Random.Range(-14.2f,14.2f),0,20);
            Quaternion spawnRotation = Quaternion.identity;
            Instantiate(hazard,spawnPosition,spawnRotation);
            yield return new WaitForSeconds (hazard_spawn_delay);
        }

        wave++;
        yield return new WaitForSeconds (3.0f);
        healthPackText.SetActive(true);
        healthpack_emitter.Play();
        yield return new WaitForSeconds (5.0f);
        energycells_emitter.Play();
        healthPackText.SetActive(false);

        //wave 8 - healthpacks
        for(int i = 0; i<wave_hazard_nums[wave]; i++){
            Vector3 spawnPosition = new Vector3 (Random.Range(-14.2f,14.2f),0,20);
            Quaternion spawnRotation = Quaternion.identity;
            if(i == 0)
                Instantiate(bonus[1],spawnPosition,spawnRotation);//hp
            else if(i == 1)  
                Instantiate(bonus[1],spawnPosition,spawnRotation);//hp
            else if(i == 2) 
                Instantiate(bonus[2],spawnPosition,spawnRotation);//mp
            else if(i == 3) 
                Instantiate(bonus[3],spawnPosition,spawnRotation);//weapon buff
            yield return new WaitForSeconds (hazard_spawn_delay*2);
        }

        wave++;
        mothership_emitter.Play();
        main_theme.setParameterValue("Organ",0f);
        main_theme.setParameterValue("Synth Main",0f);
        main_theme.setParameterValue("Synth Accent",1f);
        yield return new WaitForSeconds (wave_delay*2);

        //wave 9 - boss
        for(int i = 0; i<1; i++){
            boss_encounter = true;
            Vector3 spawnPosition = new Vector3 (0,0,20);
            Quaternion spawnRotation = Quaternion.identity;
            Instantiate(hazards[5],spawnPosition,spawnRotation);
            yield return new WaitWhile(() => boss_encounter);
        }
        level_complete = true;
        levelOver1();
    }

    public void addScore (int newScoreValue){
        score += newScoreValue;
        updateScore();
    }

    void updateScore(){
        score_Text.text = "Score: " + score;
    }

       private void PauseGame()
    {
        Time.timeScale = 0;
        pauseText.SetActive(true);
        pausePanel.SetActive(true);
        //Disable scripts that still work while timescale is set to 0
    } 
    private void ContinueGame()
    {
        Time.timeScale = 1;
        pauseText.SetActive(false);
        pausePanel.SetActive(false);
        //enable the scripts again
    }

    public void gameOver(){
        gameOverText.text = "Game Over";
        score_Text.text = "";
        fin_score_text.text ="Score: " + score;
        restartText.text = "Press R to Restart\nPress Q to Quit";
        restartflag = true;
    }

    private void levelOver1(){
        gameOverText.text = "level 1 Completed";
        score_Text.text = "";
        fin_score_text.text ="Score: " + score;
        int old_score = PlayerPrefs.GetInt("level1Score",0);
        if(score > old_score){//new high score
            PlayerPrefs.SetInt("level1Score", score); 
            PlayerPrefs.Save(); 
        }        
        restartText.text = "Press C to continue";
        restartflag = true;
    }
  
    public int getGameMode(){
        return gameState;
    }

}
