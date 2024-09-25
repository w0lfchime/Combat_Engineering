using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public Transform playerTransform;

    public Vector2Int chunkSize;

    public int viewDistance = 2;

    private Dictionary<Vector2Int, Chunk> loadedChunks;
    private Queue<Chunk> chunkLoadQueue;

    void Start()
    {
        loadedChunks = new Dictionary<Vector2Int, Chunk>();
        chunkLoadQueue = new Queue<Chunk>();

        // Initially load chunks around the player
        LoadChunksAroundPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if we need to load/unload chunks based on player movement
        LoadChunksAroundPlayer();
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
        return new Vector2Int(Mathf.FloorToInt(position.x / chunkSize.x), Mathf.FloorToInt(position.z / chunkSize.y));
    }

    // Function to load a chunk
    private void LoadChunk(Vector2Int chunkCoord)
    {
        if (!loadedChunks.ContainsKey(chunkCoord))
        {
            // Create the chunk and set its parent (optionally a container for all chunks)
            print("creating new chunk");
            Chunk newChunk = new Chunk(chunkCoord, chunkSize, transform);
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
