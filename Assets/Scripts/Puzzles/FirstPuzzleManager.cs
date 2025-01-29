using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPuzzleManager : MonoBehaviour
{
    [SerializeField] List<PressurePlate> pressurePlates;
    [SerializeField] GameObject barrierPrefab;
    [SerializeField] Vector3 barrierPos;
    [SerializeField] Sprite barriedSprite;
    [SerializeField] Vector3 spriteOffset;
    private GameObject barrier;

    private void Start()
    {
        spawnBarrier();
    }

    private void Update()
    {
        bool doorOpen = isDoorOpen();
        if (!doorOpen && barrier == null)
            spawnBarrier();

        if (doorOpen && barrier != null)
            despawnBarrier();
    }

    private bool isDoorOpen()
    {
        foreach (PressurePlate pressurePlate in pressurePlates)
            if (pressurePlate.getState() == 0)
                return false;

        return true;
    }

    private void spawnBarrier()
    {
        barrier = Instantiate(barrierPrefab, barrierPos, Quaternion.identity);

        GameObject barrierVis = new GameObject("BarrierVisual");
        SpriteRenderer spriteRenderer = barrierVis.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = barriedSprite;
        spriteRenderer.sortingLayerName = "PressurePlate";
        barrierVis.transform.position = barrier.transform.position + spriteOffset;

        barrierVis.transform.parent = barrier.transform;
    }

    private void despawnBarrier()
    {
        Destroy(barrier);
        barrier = null;
    }
}
