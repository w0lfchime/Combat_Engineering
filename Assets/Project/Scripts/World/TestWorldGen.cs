using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWorldGen : MonoBehaviour
{
    public int worldGenWidth;
    public int worldGenHeight;

    public GameObject testChunkPrefab;

    public Transform origin;

    void Start()
    {
        if (worldGenHeight % 2 == 1)
        {
            worldGenHeight++;
        }
        if (worldGenWidth % 2 == 1)
        {
            worldGenWidth++;
        }
        int world_x_half = worldGenWidth / 2;
        int world_y_half = worldGenHeight / 2;
        float chunk_size = 10.0f;
        for (int i = -world_x_half; i < world_x_half; i++)
        {
            for (int k = -world_y_half; k < world_y_half; k++)
            {
                Vector3 chunkPosition = new Vector3(i * chunk_size + (chunk_size / 2), 0, k * chunk_size + (chunk_size / 2));
                Quaternion rotation = Quaternion.Euler(90, 0, 0);
                Instantiate(testChunkPrefab, chunkPosition, rotation);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
