using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroy_by_contact : MonoBehaviour
{
    public GameObject explosion;
    public int score_value;

    private game_controller gamecontroller;
    private SimpleHealthBar healthBar;
    private SimpleHealthBar manaBar;


    private GameObject damage_sound_source;
    private GameObject healthsoundsource;

    void Start ()
        {
            GameObject gameControllerObject = GameObject.FindGameObjectWithTag ("GameController");
            gamecontroller = gameControllerObject.GetComponent <game_controller>();
            GameObject hb = GameObject.FindGameObjectWithTag ("HealthBar");
            healthBar = hb.GetComponent<SimpleHealthBar>();
            GameObject mb = GameObject.FindGameObjectWithTag ("ManaBar");
            manaBar = mb.GetComponent<SimpleHealthBar>();    
            damage_sound_source = GameObject.FindGameObjectWithTag ("Damage_sound_source");
            healthsoundsource = GameObject.FindGameObjectWithTag ("HealthSoundSource");
        }

   
   
    void OnTriggerEnter(Collider other){
        if(other.CompareTag("Boundary") || other.CompareTag("Enemy")){
            return;
        }
        if(other.CompareTag("Weapon") && explosion != null ){//player bolt has hit enemy
            Destroy(other.gameObject);
            if(gameObject.CompareTag("mine")){//mines are handled separetly 
                return;
            }
            enemy_health eh = gameObject.GetComponent<enemy_health>();
            int enemy_hp = eh.reduce_health(1);
            if(enemy_hp<=0){
                Instantiate(explosion, transform.position,transform.rotation);
                add_mana();
                if (gamecontroller.boss_encounter) {//boss;
                    Instantiate(explosion, new Vector3(transform.position.x - 1,transform.position.y,transform.position.z),transform.rotation);
                    Instantiate(explosion, new Vector3(transform.position.x + 1,transform.position.y,transform.position.z),transform.rotation);
                    Instantiate(explosion, new Vector3(transform.position.x,transform.position.y,transform.position.z+1),transform.rotation);
                    Instantiate(explosion, new Vector3(transform.position.x,transform.position.y,transform.position.z-1),transform.rotation);
                    gamecontroller.boss_encounter = false;
                }
                Destroy(gameObject);
                gamecontroller.addScore(score_value);
            }
            else{
                damage_sound_source.GetComponent<FMODUnity.StudioEventEmitter>().Play();
            }
        }
        else if (other.CompareTag("Player")){//enemy has hit player
            if(gameObject.CompareTag("mine")){//mines are handled separetly 
                return;
            }
            if(gameObject.CompareTag("Health_Pickup")){
                healthsoundsource.GetComponent<FMODUnity.StudioEventEmitter>().Play();
                float current_health = healthBar.GetCurrentFraction * gamecontroller.max_life;
                float updated_health = current_health + gamecontroller.healthPack_val;
                if (updated_health > gamecontroller.max_life ) {
                    updated_health = gamecontroller.max_life;
                }
                healthBar.UpdateBar(updated_health,  gamecontroller.max_life); 
            }
            else if(gameObject.CompareTag("Shield_Pickup")){
                healthsoundsource.GetComponent<FMODUnity.StudioEventEmitter>().Play();
                other.GetComponent<Player_Controller>().activate_shield();
            }
            else if(gameObject.CompareTag("mana_pickup")){
                healthsoundsource.GetComponent<FMODUnity.StudioEventEmitter>().Play();
                manaBar.UpdateBar(100,  gamecontroller.max_mana); 
            }
            else if(gameObject.CompareTag("weapon_pickup")){
                healthsoundsource.GetComponent<FMODUnity.StudioEventEmitter>().Play();
                other.gameObject.GetComponent<Player_Controller>().upgrade_weapon();
            }
            else if(other.GetComponent<Player_Controller>().isVulnerable()){
                damage_player(other.gameObject);
            }
            else{//player is shielded
                other.GetComponent<shield_sound_ctrl>().shield_hit_sound();
            }
            Destroy(gameObject);                
        }
    }

    public void damage_player(GameObject player){
        Destroy(gameObject);    
        float current_health = healthBar.GetCurrentFraction * gamecontroller.max_life;
        float updated_health = current_health - gamecontroller.hazard_damage;
        if(updated_health <= 0){
            GameObject [] players = gamecontroller.players;
            foreach(GameObject p in players ){
                if(p != null && p.activeInHierarchy){
                    Instantiate(gamecontroller.playerExplosion, transform.position,transform.rotation);
                    Destroy(p);
                }
            }
            gamecontroller.gameOver();
        }
        else {
            damage_sound_source.GetComponent<FMODUnity.StudioEventEmitter>().Play();
            if(explosion != null){
                Instantiate(explosion, transform.position,transform.rotation);
            }
        }
        healthBar.UpdateBar(updated_health,  gamecontroller.max_life); 
    } 
    public void add_mana(){
        float current_mana = manaBar.GetCurrentFraction * gamecontroller.max_mana;
        float updated_mana = current_mana + gamecontroller.hazard_mana_gain;
        manaBar.UpdateBar(updated_mana,  gamecontroller.max_mana); 
    }
}