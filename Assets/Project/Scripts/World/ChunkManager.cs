using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject chunkPrefab;
    [SerializeField] GameObject[] chunkParents;
    [SerializeField] float activationDistance = 50.0f;
    [SerializeField] float checkInterval = 1.0f;


    public int chunk_size = 25;

    void Start()
    {
        StartCoroutine(CheckChunkDistance());
    }

    void GenerateMap(int width, int height)
    {

    }

    IEnumerator CheckChunkDistance()
    {
        while (true)
        {
            foreach (GameObject chunkParent in chunkParents)
            {
                float distanceToPlayer = Vector3.Distance(player.transform.position, chunkParent.transform.position);

                if (distanceToPlayer <= activationDistance)
                {
                    chunkParent.SetActive(true);
                }
                else
                {
                    chunkParent.SetActive(false);
                }

            }
            yield return new WaitForSeconds(checkInterval);
        }
    }
}
