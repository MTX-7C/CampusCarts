using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class ItemBlock : NetworkBehaviour
{
    public NetworkVariable<int> itemHeld = new NetworkVariable<int>();
    public bool hit = false;
    Collider hitCollider;
    MeshRenderer meshRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hitCollider = GetComponent<Collider>();
        meshRenderer = GetComponent<MeshRenderer>();
        itemHeld.Value = Random.Range(0, 4);
    }

    public int WhatItem()
    {
        return itemHeld.Value;
    }

    public void GotHit()
    {
        hitCollider.enabled = false;
        meshRenderer.enabled = false;
        StartCoroutine(Regenerate());
    }

    IEnumerator Regenerate()
    {
        yield return new WaitForSeconds(5);
        itemHeld.Value = Random.Range(0, 4);
        hit = false;
        hitCollider.enabled = true;
        meshRenderer.enabled = true;
    }
}
