using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour
{
    [SerializeField] List<LightBulb> lightBulbs;

    private int state = 0;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void changeState()
    {
        state = (state + 1) % 2;

        foreach (LightBulb manager in lightBulbs)
            manager.changeState();

        float angle = 45 * ((state == 0) ? 1 : -1);

        // Lever should have a child for visual at 0 index
        transform.GetChild(0).rotation = Quaternion.Euler(0, 0, angle);

        audioSource.Play();
    }
}
