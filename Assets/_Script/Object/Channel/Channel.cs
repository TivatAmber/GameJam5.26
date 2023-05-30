using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Channel : MonoBehaviour
{
    public GameObject target;
    [SerializeField] protected bool filled;
    [SerializeField] protected Kind whatsIn;
    [SerializeField] protected RawImage rawImage;
    [SerializeField] protected Texture2D tmpImage;
    protected void Start()
    {
        rawImage = GetComponent<RawImage>();
        filled = false;
        whatsIn = 0;
    }
    protected void ResetImage()
    {
        rawImage.texture = tmpImage;
    }
    public void ChangeImage(Texture image)
    {
        rawImage.texture = image;
    }
}
