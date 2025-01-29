using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    [Header("Player Damage SFX")]
    [SerializeField]
    AudioSource playerHitSFX;

    [Header("Player Sword SFX")]
    [SerializeField]
    AudioSource swordSwingSFX;

    [Header("Player Dies / Game Over")]
    [SerializeField]
    AudioSource gameOverSFX;

    [Header("Box Pickup")]
    [SerializeField]
    AudioSource pickupBoxSFX;

    [Header("Box Drop")]
    [SerializeField]
    AudioSource dropBoxSFX;

    public void PlayerTakesDamageSound()
    {
        //Debug.Log("Sound triggered");
        playerHitSFX.Play();
    }

    public void PlayerSwingsSwordSound()
    {
        //Debug.Log("Sound triggered");
        swordSwingSFX.Play();
    }

    public void PlayerDiesSound()
    {
        //Debug.Log("Sound triggered");
        gameOverSFX.Play();
    }

    public void BoxPickupSound()
    {
        //Debug.Log("Sound triggered");
        pickupBoxSFX.Play();
    }

    public void BoxDropSound()
    {
        //Debug.Log("Sound triggered");
        dropBoxSFX.Play();
    }
}
