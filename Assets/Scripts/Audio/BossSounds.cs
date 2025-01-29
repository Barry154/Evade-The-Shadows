using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSounds : MonoBehaviour
{
    [Header("Boss Damage SFX")]
    [SerializeField] AudioSource takeDamageSFX;

    [Header("Boss Roar SFX")]
    [SerializeField] AudioSource roarSFX;

    [Header("Boss Damage Roar SFX")]
    [SerializeField] AudioSource roarDamageSFX;

    [Header("Boss Dies SFX")]
    [SerializeField] AudioSource bossDeadSFX;

    public void BossTakesDamageSound()
    {
        //Debug.Log("Sound triggered");
        takeDamageSFX.Play();
        roarDamageSFX.Play();
    }

    public void BossDiesSound()
    {
        //Debug.Log("Sound triggered");
        bossDeadSFX.Play();
    }

    public void BossRoarsSound()
    {
        //Debug.Log("Sound triggered");
        roarSFX.Play();
    }
}
