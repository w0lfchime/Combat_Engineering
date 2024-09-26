using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
	public Transform playerTransform;
	[SerializeField] private Vector2Int playerChunkPos;

	public int chunk_size;
	private Vector2Int chunkSize;
	public int viewDistance = 2;

	private Dictionary<Vector2Int, Chunk> chunks;
	private HashSet<Vector2Int> loadedChunks;
	private Queue<Vector2Int> chunkLoadQueue;


	[SerializeField] private int chunk_count;
	[SerializeField] private int chunkLoadQueueSize;

	[Header("Chunk Loading")]
	public float chunkProcessDelay = 0.2f;
	public float chunkScanDelay = 1;
	public int chunkLoadingBatchSize = 1;

	void Start()
	{
		chunk_count = 0;
		chunks = new Dictionary<Vector2Int, Chunk>();
		loadedChunks = new HashSet<Vector2Int>();
		chunkLoadQueue = new Queue<Vector2Int>();
		chunkSize = new Vector2Int(chunk_size, chunk_size);

		StartCoroutine(ChunkManagementCoroutine());
	}

	IEnumerator ChunkManagementCoroutine()
	{
		float chunkLoadTimer = 0f;
		float chunkProcessTimer = 0f;

		while (true)
		{
			// Update timers with delta time
			chunkLoadTimer += Time.deltaTime;
			chunkProcessTimer += Time.deltaTime;

			// Process chunk loading every 'chunkScanDelay' seconds
			if (chunkLoadTimer >= chunkScanDelay)
			{
				ScanChunks();
				chunkLoadTimer = 0f;  // Reset timer
			}

			// Process one chunk from the queue every 'chunkProcessDelay' seconds
			if (chunkProcessTimer >= chunkProcessDelay)
			{
				if (chunkLoadQueue.Count > 0)
				{
					for (int i = 0; i < chunkLoadingBatchSize; i++)
					{
						ProcessChunkLoadQueue();
						if (chunkLoadQueue.Count == 0)
						{
							i = chunkLoadingBatchSize;
						}
					}

				}
				chunkProcessTimer = 0f;  // Reset timer
			}

			yield return null;  // Return control until the next frame
		}
	}

	// Helper to get chunk coordinates from a world position
	private void UpdatePlayerChunkCoordinates()
	{
		Vector3 pt = playerTransform.position;
		playerChunkPos.x = Mathf.RoundToInt(pt.x / chunkSize.x);
		playerChunkPos.y = Mathf.RoundToInt(pt.z / chunkSize.y);
	}


	// Function to load/unload chunks around the player
	private void ScanChunks()
	{
		// Update player's current chunk position
		UpdatePlayerChunkCoordinates();

		HashSet<Vector2Int> loadedChunkPool = new HashSet<Vector2Int>(loadedChunks);
		// Iterate over the area around the player and load chunks within view distance
		for (int x = -viewDistance; x <= viewDistance; x++)
		{
			for (int y = -viewDistance; y <= viewDistance; y++)
			{
				Vector2Int chunkCoord = new Vector2Int(playerChunkPos.x + x, playerChunkPos.y + y);

				loadedChunkPool.Add(chunkCoord);
			}
		}

		foreach (var chunkCoord in loadedChunkPool)
		{
			bool isLoadedChunk = loadedChunks.Contains(chunkCoord);
			if (Vector2Int.Distance(chunkCoord, playerChunkPos) < viewDistance)
			{
				if (!loadedChunks.Contains(chunkCoord))
				{
					QueueChunk(chunkCoord, true);
				}
			} 
			else if (isLoadedChunk)
			{
				QueueChunk(chunkCoord, false);
			}
		}
	
	}

	private void QueueChunk(Vector2Int chunk, bool load)
	{

		if (chunks.ContainsKey(chunk))
		{
			if (chunks[chunk].load == load)
			{
				return;
			} 
			else
			{
				chunks[chunk].load = load;
			}
		}
	
		if (!chunkLoadQueue.Contains(chunk))
		{
			chunkLoadQueue.Enqueue(chunk);
		}

	}

	private void ProcessChunkLoadQueue()
	{
		Vector2Int chunkCoord = chunkLoadQueue.Dequeue();
		if (!chunks.ContainsKey(chunkCoord))
		{
			chunk_count++;
			print("Creating new chunk with ID: " + chunk_count);
			Chunk newChunk = new Chunk(chunkCoord, chunkSize, transform, chunk_count);
			chunks.Add(chunkCoord, newChunk);
		}
		Chunk c = chunks[chunkCoord];
		if (c.load == true)
		{
			c.LoadChunk();
			loadedChunks.Add(chunkCoord);
		} else
		{
			c.UnloadChunk();
			loadedChunks.Remove(chunkCoord);
		}
	}
}
