using UnityEngine;
using System.Collections.Generic;

public class Chunk
{
    public Vector2Int chunkPosition;
    public Vector2Int chunkSize;
    private GameObject chunkQuad;

    private Dictionary<Vector2Int, GameObject> chunkObjects;

    // Constructor
    public Chunk(Vector2Int position, Vector2Int size, Transform parent)
    {
        chunkPosition = position;
        chunkSize = size;
        chunkObjects = new Dictionary<Vector2Int, GameObject>();

        // Create and scale the quad
        CreateQuad(parent);
    }

    // Method to create the quad representing the chunk in the world
    private void CreateQuad(Transform parent)
    {
        chunkQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        chunkQuad.transform.position = new Vector3(chunkPosition.x * chunkSize.x, 0, chunkPosition.y * chunkSize.y);
        chunkQuad.transform.localScale = new Vector3(chunkSize.x, 1, chunkSize.y);
        chunkQuad.transform.SetParent(parent);
        chunkQuad.SetActive(false);  // Initially deactivate the quad
    }

    // Activate and deactivate the chunk
    public void ActivateChunk()
    {
        if (chunkQuad != null)
        {
            chunkQuad.SetActive(true);
        }
    }

    public void DeactivateChunk()
    {
        if (chunkQuad != null)
        {
            chunkQuad.SetActive(false);
        }
    }

    public void LoadChunk()
    {
        // Load the chunk's objects, data, etc.
        ActivateChunk();
    }

    public void UnloadChunk()
    {
        // Unload chunk objects and data
        DeactivateChunk();
    }
}
