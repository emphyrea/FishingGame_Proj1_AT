using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectionScript : MonoBehaviour
{

    public static CollectionScript Instance { get; private set; }

    public Image CollectionMenu;


    public List<Image> fishImages;
    public FishType[] childTypes;
    public List<GameObject> collectionObjs;


    private void OnEnable()
    {
        FishingScript.onCollectSender += OnCollectReceive;
    }

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {

        for (int i = 0; i < CollectionMenu.transform.childCount; i++)
        {
            collectionObjs.Add(CollectionMenu.transform.GetChild(i).gameObject);
            fishImages.Add(collectionObjs[i].transform.GetComponent<Image>());

            CollectionMenu.enabled = false;
            fishImages[i].enabled = false;
        }
;

    }

    // Update is called once per frame
    public void OnCollectReceive(FishType caughtType)
    {
        foreach (FishType type in childTypes)
        {
                if (type.fishName == caughtType.fishName)
                {
                    if (type.firstCatch == true)
                    {
                        type.firstCatch = false;
                        GetComponent<Image>().sprite = type.collectionImage;
                    }
                    else
                    {
                        return;
                    }
                }

        }
    }
}
