using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishType : MonoBehaviour
{
    public enum fishName
    {
        Salmon,
        Goldfish,
        Bass,
        Catfish,
        Pirahna,
        Prawn
    };

    public bool firstCatch = true;
    public Sprite collectionImage;
    public Sprite initialImage;

    private void Start()
    {
        firstCatch = true;
    }

}
