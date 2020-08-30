using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mine_explosion : MonoBehaviour
{
    public float explosion_radius;
    private game_controller gamecontroller;

    void Start(){
        GameObject gameControllerObject = GameObject.FindGameObjectWithTag ("GameController");
        gamecontroller = gameControllerObject.GetComponent <game_controller>();
    }
 
    //when hit by player/shots, query all objects within explosion radius and subtract one health from each 
    void OnTriggerEnter(Collider other){
        if(other.CompareTag("Weapon") || other.CompareTag("Player")){
            explode();
        }
    }

    public void explode(){
        int layerMask = Physics.AllLayers;
        QueryTriggerInteraction queryTriggerInteraction  = QueryTriggerInteraction.UseGlobal;
        Collider [] cols  = Physics.OverlapSphere(gameObject.transform.position,explosion_radius,layerMask,queryTriggerInteraction);
        for(int i = 0; i<cols.Length; i++){
            Collider col = cols[i];
            if(col.CompareTag("Player")){
                if(col.GetComponent<Player_Controller>().isVulnerable()){
                    gameObject.GetComponent<destroy_by_contact>().damage_player(col.gameObject);
                }
            }
            else if (col.CompareTag("Enemy")){
                enemy_health eh = col.GetComponent<enemy_health>();
                int enemy_hp = eh.reduce_health(1);
                if(enemy_hp<=0){
                    Instantiate(col.GetComponent<destroy_by_contact>().explosion,col.gameObject.transform.position,col.gameObject.transform.rotation);
                    col.gameObject.GetComponent<destroy_by_contact>().add_mana();
                    Destroy(col.gameObject);
                    gamecontroller.addScore(col.gameObject.GetComponent<destroy_by_contact>().score_value);
                }
            }
            else if (col.CompareTag("mine")){
                Instantiate(gamecontroller.playerExplosion, transform.position,transform.rotation);
                Destroy(col.gameObject);
            }
        }
        Instantiate(gamecontroller.playerExplosion, transform.position,transform.rotation);
        Destroy(gameObject);
    }
}
