using UnityEngine;

public class BackgroundPan : MonoBehaviour
{
    public float panSpeedX = 0.1f; // Speed of horizontal panning
    public float panSpeedY = 0.05f; // Speed of vertical panning
    private Material backgroundMaterial;
    private Vector2 offset;

    void Start()
    {
        // Get the material from the MeshRenderer
        backgroundMaterial = GetComponent<Renderer>().material;
    }

    void Update()
    {
        // Calculate the new offset based on time and speed
        offset = new Vector2(Time.time * panSpeedX, Time.time * panSpeedY);

        // Apply the offset to the material to pan the background
        backgroundMaterial.mainTextureOffset = offset;
    }
}
