using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundRandomizer : MonoBehaviour
{
    public Sprite[] images;

    private Image currentImage;

    // Start is called before the first frame update
    void Start()
    {
        currentImage = GetComponent<Image>();
        currentImage.sprite = images[Random.Range(0, images.Length)];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
