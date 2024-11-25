using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public Sprite Sprite;
    public string ItemName;
    public string Description;


    private void Start()
    {
        Image image = GetComponent<Image>();
        image.sprite = Sprite;
    }
}
