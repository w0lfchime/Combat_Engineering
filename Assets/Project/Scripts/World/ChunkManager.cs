using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public Transform playerTransform;

    public Vector2Int chunkSize;

    public int viewDistance = 2;

    private Dictionary<Vector2Int, Chunk> loadedChunks;
    private Queue<Chunk> chunkLoadQueue;

    [SerializeField] private int chunk_count;

    void Start()
    {
        chunk_count = 0;
        loadedChunks = new Dictionary<Vector2Int, Chunk>();
        chunkLoadQueue = new Queue<Chunk>();

        // Start the coroutine to load chunks around the player every second
        StartCoroutine(LoadChunksCo());
    }

    IEnumerator LoadChunksCo()
    {
        // Loop to continuously check and load/unload chunks every second
        while (true)
        {
            // Load chunks based on player movement
            LoadChunksAroundPlayer();

            // Wait for 1 second before running the next iteration
            yield return new WaitForSeconds(1.0f);
        }
    }


    // Function to load/unload chunks around the player
    private void LoadChunksAroundPlayer()
    {
        // Get player's current chunk position
        Vector2Int playerChunkPos = GetChunkCoordsFromPosition(playerTransform.position);

        // Iterate over the area around the player and load chunks within view distance
        for (int x = -viewDistance; x <= viewDistance; x++)
        {
            for (int y = -viewDistance; y <= viewDistance; y++)
            {
                Vector2Int chunkCoord = new Vector2Int(playerChunkPos.x + x, playerChunkPos.y + y);
                if (!loadedChunks.ContainsKey(chunkCoord))
                {
                    // Load chunk if not loaded
                    LoadChunk(chunkCoord);
                }
            }
        }

        // Unload chunks outside the view distance
        List<Vector2Int> chunksToUnload = new List<Vector2Int>();
        foreach (var loadedChunk in loadedChunks)
        {
            if (Vector2Int.Distance(loadedChunk.Key, playerChunkPos) > viewDistance)
            {
                chunksToUnload.Add(loadedChunk.Key);
            }
        }

        foreach (var chunkCoord in chunksToUnload)
        {
            UnloadChunk(chunkCoord);
        }
    }

    // Helper to get chunk coordinates from a world position
    private Vector2Int GetChunkCoordsFromPosition(Vector3 position)
    {
        // Ensure the chunk coordinates are properly calculated by using the correct axis
        int chunkX = Mathf.RoundToInt(position.x / chunkSize.x);
        int chunkZ = Mathf.RoundToInt(position.z / chunkSize.y); // Make sure you intend to use 'z' for a 3D world
        return new Vector2Int(chunkX, chunkZ);
    }


    // Function to load a chunk
    private void LoadChunk(Vector2Int chunkCoord)
    {
        if (!loadedChunks.ContainsKey(chunkCoord))
        {
            // Create the chunk and set its parent (optionally a container for all chunks)
            chunk_count++;
            print("Creating new chunk with ID: " + chunk_count);
            Chunk newChunk = new Chunk(chunkCoord, chunkSize, transform, chunk_count);
            loadedChunks.Add(chunkCoord, newChunk);
        }

        // Activate the chunk
        loadedChunks[chunkCoord].ActivateChunk();
    }

    private void UnloadChunk(Vector2Int chunkCoord)
    {
        if (loadedChunks.ContainsKey(chunkCoord))
        {
            // Deactivate the chunk instead of destroying it
            loadedChunks[chunkCoord].DeactivateChunk();
        }
    }


    // Optional: Asynchronous or coroutine to handle chunk loading in the background (for large worlds)
    private void ProcessChunkLoadQueue()
    {
        // Example of processing queued chunks over time
        while (chunkLoadQueue.Count > 0)
        {
            Chunk chunkToLoad = chunkLoadQueue.Dequeue();
            chunkToLoad.LoadChunk();
        }
    }
}
