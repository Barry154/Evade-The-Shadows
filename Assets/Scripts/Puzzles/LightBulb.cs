using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBulb : MonoBehaviour
{
    [SerializeField] Sprite defaultState;
    [SerializeField] Sprite correctState;

    private int state = 0;
    SpriteRenderer spriteRenderer;

    public int getState() { return state; }

    private void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    public void changeState()
    {
        state = (state + 1) % 2;
        spriteRenderer.sprite = (state == 0) ? defaultState : correctState;
    }
}
