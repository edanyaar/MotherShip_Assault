using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shield_sound_ctrl : MonoBehaviour
{

    private string shield_on_Event = "event:/sfx/Shield_power_Up";
    private string shield_off_Event = "event:/sfx/Shield_power_Down";
    private string shield_hit_Event = "event:/sfx/Shield_Hit";

    private FMOD.Studio.EventInstance shield_on_instance;
    private FMOD.Studio.EventInstance shield_off_instance;
    private FMOD.Studio.EventInstance shield_hit_instance;

    public void shield_on_sound(){
        shield_on_instance = FMODUnity.RuntimeManager.CreateInstance(shield_on_Event);
        shield_on_instance.start();
    }
    public void shield_off_sound(){
        shield_off_instance = FMODUnity.RuntimeManager.CreateInstance(shield_off_Event);
        shield_off_instance.start();
    }
    public void shield_hit_sound(){
        shield_hit_instance = FMODUnity.RuntimeManager.CreateInstance(shield_hit_Event);
        shield_hit_instance.start();
    }
}
