using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weapon_controller : MonoBehaviour
{
    public GameObject [] shots;
    public Transform [] shot_spawns;
    public float fire_rate;
    public float delay;
    public int boss_flag;

    private FMODUnity.StudioEventEmitter eventEmitter;

    void Start()
    {
        InvokeRepeating("fire",delay,fire_rate);
        eventEmitter = GetComponent<FMODUnity.StudioEventEmitter>();
    }

    void fire()
    {
        if(boss_flag == 0){
            GameObject shot = shots[0];
            Transform shot_spawn = shot_spawns[0];
            Instantiate(shot, shot_spawn.position,shot_spawn.rotation);
            eventEmitter.Play();
        }
        else if (boss_flag == 1){
            GameObject shot = shots[0];
            for(int i = 0; i<3; i++){
                Transform shot_spawn = shot_spawns[i];
                Instantiate(shot, shot_spawn.position,shot_spawn.rotation);
                eventEmitter.Play();
            }
        }
    }
}
