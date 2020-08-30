using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_health : MonoBehaviour
{
    public int health;

    public int reduce_health(int amount){
        health = health - amount;
        return health;
    }
    public void double_enemy_health(){
        health = health * 2;

    }
}
