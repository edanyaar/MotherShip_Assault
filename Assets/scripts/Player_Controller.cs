using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Boundary{
    public float xMin, xMax, zMin, zMax;
}


public class Player_Controller : MonoBehaviour
{
    public int player_speed;
    public float tilt;
    public GameObject shot;
    public Transform shot_spawn;
    public Transform shot_spawn2;
    public Transform shot_spawn3;
    public float fire_rate;
    private float next_fire = 0;
    public Boundary boundary;
    public Boundary boss_boundary;
    private Rigidbody rigbody;
    private game_controller gamecontroller;
    public string horizontalKey;
    public string verticalKey;
    public string fireKey;
    public string shieldKey;

    private bool vulnerable;

    public bool upgraded;

    public GameObject shield;
    public float shield_mana_loss;

    private SimpleHealthBar manaBar;

    private FMODUnity.StudioEventEmitter shot_Emitter;

    bool shield_on = false;

    void Start(){
        upgraded = false;
        vulnerable = true;
        rigbody = GetComponent<Rigidbody>();
        GameObject gameControllerObject = GameObject.FindGameObjectWithTag ("GameController");
        gamecontroller = gameControllerObject.GetComponent <game_controller>();
        GameObject mb = GameObject.FindGameObjectWithTag ("ManaBar");
        manaBar = mb.GetComponent<SimpleHealthBar>();  
        shot_Emitter = GetComponent<FMODUnity.StudioEventEmitter>();
        shield.GetComponent<MeshRenderer>().enabled = false; 
    }

    void Update(){
        if(Input.GetButtonDown(fireKey) && Time.time>next_fire){
            
            next_fire = Time.time + fire_rate;

            if(upgraded){
                Instantiate(shot, shot_spawn.position,shot_spawn.rotation);
                Instantiate(shot, shot_spawn2.position,shot_spawn.rotation);
                Instantiate(shot, shot_spawn3.position,shot_spawn.rotation);
            }
            else{
                Instantiate(shot, shot_spawn.position,shot_spawn.rotation);
            }

            shot_Emitter.Play();
        }

        float current_mana = manaBar.GetCurrentFraction * gamecontroller.max_mana;
        
        if(Input.GetButtonDown(shieldKey)){
            GetComponent<shield_sound_ctrl>().shield_on_sound();
        }
        if(Input.GetButton(shieldKey)){//shield activation
            if(current_mana > 0){
                shield_on = true;
                float updated_mana = current_mana - shield_mana_loss;
                manaBar.UpdateBar(updated_mana,  gamecontroller.max_mana); 
                vulnerable = false;
                shield.GetComponent<MeshRenderer>().enabled = true; 
            }
            else if (shield_on){//player ran out of mana for shield
                vulnerable = true;
                shield_on = false;
                shield.GetComponent<MeshRenderer>().enabled = false; 
                GetComponent<shield_sound_ctrl>().shield_off_sound();
            }
        }
        else if (shield_on){//player stopped holding shield button
            vulnerable = true;
            shield_on = false;
            shield.GetComponent<MeshRenderer>().enabled = false; 
            GetComponent<shield_sound_ctrl>().shield_off_sound();
        }
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis(horizontalKey);
        float moveVertical = Input.GetAxis(verticalKey);
        Vector3 movment =  new Vector3(moveHorizontal,0.0f,moveVertical);
        rigbody.velocity = movment*player_speed;

        if(gamecontroller.boss_encounter){//limit movement on z axis during boss phase
            rigbody.position = new Vector3(
                Mathf.Clamp(rigbody.position.x, boss_boundary.xMin, boss_boundary.xMax),
                0.0f,
                Mathf.Clamp(rigbody.position.z, boss_boundary.zMin, boss_boundary.zMax)
            );
        }
        else{
            rigbody.position = new Vector3(
                Mathf.Clamp(rigbody.position.x, boundary.xMin, boundary.xMax),
                0.0f,
                Mathf.Clamp(rigbody.position.z, boundary.zMin, boundary.zMax)
            );
        }

        rigbody.rotation = Quaternion.Euler (0.0f,0.0f,rigbody.velocity.x * (-tilt));
    }

    public void upgrade_weapon(){
        StartCoroutine(upgrade_weapon_helper());
    }

    private IEnumerator upgrade_weapon_helper(){
        upgraded = true; 
        yield return new WaitForSeconds (gamecontroller.weapon_upgrade_duration);
        upgraded = false; 
    }

    //no longer used, leaving it here just in case we change our minds
   public void activate_shield(){
       StartCoroutine(activate_shield_helper());
   }

   //no longer used, leaving it here just in case we change our minds
   // in charge of activating (and deactivating) the player shield
   private IEnumerator activate_shield_helper(){
       vulnerable = false;
       GameObject.FindGameObjectWithTag ("Shield").GetComponent<MeshRenderer>().enabled = true;
       yield return new WaitForSeconds (gamecontroller.shield_duration);
       vulnerable = true;
       GameObject.FindGameObjectWithTag ("Shield").GetComponent<MeshRenderer>().enabled = false;
   } 

    public bool isVulnerable(){
        return vulnerable;
    }
}
