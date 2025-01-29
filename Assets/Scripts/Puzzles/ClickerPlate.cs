using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickerPlate : MonoBehaviour
{
    [SerializeField] Sprite defaultState;
    [SerializeField] Sprite pressedState;
    [SerializeField] ThirdPuzzleManager door;
    [SerializeField] int plateIndex;


    private int state = 0;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    public void resetPlate()
    {
        state = 0;
        spriteRenderer.sprite = defaultState;
    }

    private void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (state == 0)
        {
            state = 1;
            spriteRenderer.sprite = pressedState;
            door.setCurrentPlateIndex(plateIndex);
            audioSource.Play();
        }
    }
}
