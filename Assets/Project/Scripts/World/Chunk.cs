using UnityEngine;
using System.Collections.Generic;

public class Chunk
{
	public Vector2Int chunkPosition;
	public Vector2Int chunkSize;
	private int chunk_ID;
	private GameObject chunkQuad;

	public bool load; 

	private Dictionary<Vector2Int, GameObject> chunkObjects;

	// Constructor
	public Chunk(Vector2Int position, Vector2Int size, Transform parent, int ID)
	{
		chunkPosition = position;
		chunkSize = size;
		chunkObjects = new Dictionary<Vector2Int, GameObject>();
		chunk_ID = ID;

		load = true; 
		// Create and scale the quad
		CreateQuad(parent);
	}

	public override string ToString()
	{
		return string.Format("chunk with ID: {0}, Grid X: {1}, Grid Y: {2}", chunk_ID, chunkPosition.x, chunkPosition.y);
	}

	// Method to create the quad representing the chunk in the worldS
	private void CreateQuad(Transform parent)
	{
		chunkQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
		chunkQuad.transform.rotation = Quaternion.Euler(90, 0, 0);
		chunkQuad.transform.position = new Vector3(chunkPosition.x * chunkSize.x, 0, chunkPosition.y * chunkSize.y);
		chunkQuad.transform.localScale = new Vector3(chunkSize.x, chunkSize.y, 1);
		chunkQuad.transform.SetParent(parent);
		chunkQuad.SetActive(false);  // Initially deactivate the quad
	}

	
	public void LoadChunk()
	{
		chunkQuad.SetActive(true);
	}

	public void UnloadChunk()
	{
		chunkQuad.SetActive(false);   
	}

}
