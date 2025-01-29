using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySounds : MonoBehaviour
{
    [Header("Enemy Damage SFX")]
    [SerializeField] AudioSource dealDamageSFX;

    [Header("Enemy Dies SFX")]
    [SerializeField] AudioSource enemyDeadSFX;

    public void EnemyTakesDamageSound()
    {
        //Debug.Log("Sound triggered");
        dealDamageSFX.Play();
    }

    public void EnemyDiesSound()
    {
        //Debug.Log("Sound triggered");
        enemyDeadSFX.Play();
    }
}
