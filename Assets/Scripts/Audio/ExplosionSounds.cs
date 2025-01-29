using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionSounds : MonoBehaviour
{
    [Header("Bomber Explosion SFX")]
    [SerializeField] AudioSource explodesSFX;

    public void ExplodesSound()
    {
        //Debug.Log("Sound triggered");
        explodesSFX.Play();
    }
}
