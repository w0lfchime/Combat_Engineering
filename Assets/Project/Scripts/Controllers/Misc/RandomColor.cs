using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomColor : MonoBehaviour
{

    public Color[] colorsArray = new Color [] { Color.red, Color.blue, Color.green, Color.yellow, Color.white };

    private Renderer rendererObj;

    void Start()
    {
        rendererObj = GetComponent<Renderer>();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            RandomizeColor();
        }
    }

    void RandomizeColor()
    {
        print("test");
        rendererObj.material.color = colorsArray[Random.Range(0, colorsArray.Length)];
    }
}
