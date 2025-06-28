using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : NetworkBehaviour
{
    OnTrack onTrack;
    CarDrive carDrive;

    public NetworkVariable<int> useItem = new NetworkVariable<int>();
    public NetworkVariable<int> coinCount = new NetworkVariable<int>();

    public GameObject heldItem;
    public GameObject shell;
    public Transform firePoint;

    public GameObject[] itemOptions = new GameObject[4];

    public Image paperScreen;

    float boosting = 0;



    private void Start()
    {
        onTrack = GetComponent<OnTrack>();
        carDrive = GetComponent<CarDrive>();
        paperScreen = GameObject.Find("PaperScreen").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!onTrack.isPlayer && useItem.Value == 1 && heldItem != null)
        {
            switch (heldItem.tag)
            {
                case "PaperBomb":
                    heldItem = null;
                    paperScreen.enabled = true;
                    StartCoroutine(PaperScreen());
                    break;
                case "ClassCredit":
                    heldItem = null;
                    break;
                case "Shell":
                    heldItem = null;
                    GameObject newShell = Instantiate(shell, firePoint);
                    Rigidbody shellRb = newShell.GetComponent<Rigidbody>();
                    shellRb.AddRelativeForce(0, 0, 1500, ForceMode.Force);
                    newShell.GetComponent<Transform>().parent = null;
                    Destroy(newShell, 15);
                    break;
                case "Boost":
                    heldItem = null;
                    break;
            }
        }
        if (Input.GetKeyDown(KeyCode.U) && heldItem != null && onTrack.isPlayer)
        {
            switch (heldItem.tag)
            {
                case "PaperBomb":
                    useItem.Value = 1;
                    heldItem = null;
                    StartCoroutine(UsedItem());
                    break;
                case "ClassCredit":
                    heldItem = null;
                    useItem.Value = 1;
                    if (coinCount.Value <= 5)
                        coinCount.Value++;
                    StartCoroutine(UsedItem());
                    break;
                case "Shell":
                    heldItem = null;
                    useItem.Value = 1;
                    GameObject newShell = Instantiate(shell, firePoint);
                    Rigidbody shellRb = newShell.GetComponent<Rigidbody>();
                    shellRb.AddRelativeForce(0, 0, 1500, ForceMode.Force);
                    newShell.GetComponent<Transform>().parent = null;
                    Destroy(newShell,15);
                    StartCoroutine(UsedItem());
                    break;
                case "Boost":
                    heldItem = null;
                    useItem.Value = 1;
                    boosting = 3;
                    StartCoroutine(UsedItem());
                    break;
            }
        }
        if (onTrack.isPlayer)
        {
            if (coinCount.Value == 0 && carDrive.speed != 2000 && boosting == 0)
            { carDrive.speed = 2000; }
            else if (coinCount.Value == 1 && carDrive.speed != 2100 && boosting == 0)
            { carDrive.speed = 2100; }
            else if (coinCount.Value == 2 && carDrive.speed != 2200 && boosting == 0)
            { carDrive.speed = 2200; }
            else if (coinCount.Value == 3 && carDrive.speed != 2300 && boosting == 0)
            { carDrive.speed = 2300; }
            else if (coinCount.Value == 4 && carDrive.speed != 2400 && boosting == 0)
            { carDrive.speed = 2400; }
            else if (coinCount.Value == 5 && carDrive.speed != 2500 && boosting == 0)
            { carDrive.speed = 2500; }

            if (coinCount.Value > 5)
                coinCount.Value = 5;
        }

        if(onTrack.isPlayer && boosting > 0)
        {
            boosting -= Time.deltaTime;
            carDrive.speed = 2000 * ((boosting / 4) + 1);
        }
        else if(boosting < 0)
            boosting = 0;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "IB")
        {
            ItemBlock itemBlock = other.GetComponent<ItemBlock>();
            heldItem = itemOptions[itemBlock.WhatItem()];
            itemBlock.GotHit();
        }
    }

    IEnumerator PaperScreen()
    {
        yield return new WaitForSeconds(3);
        paperScreen.enabled = false;
    }

    IEnumerator UsedItem()
    {
        yield return new WaitForSeconds(.5f);
        useItem.Value = 0;
    }
}
