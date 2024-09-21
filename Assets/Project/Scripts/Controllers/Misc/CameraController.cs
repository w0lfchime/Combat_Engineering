using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public MonoBehaviour playerObject;
	public IPlayer player;   
	public Vector3 offset = new Vector3(0f, 10f, -10f); 
	public float followSpeed = 5f;     
	public float rotationSpeed = 100f;  
	public float zoomSpeed = 5f;       
	public float minZoom = 5f;
	public float maxZoom = 20f;
	public float cameraSpeed = 5f;
	public float cameraLimit = 5f;
	public bool allowPlayerControl = true;

	private Vector3 targetPosition;
	private Vector3 currentVelocity = Vector3.zero;
	private float currentZoom = 10f;
	private bool isAiming = false;


    public KeyCode toggleBirdseye = KeyCode.B;

    void Start()
	{
		player = playerObject as IPlayer;
		currentZoom = offset.magnitude;
        targetPosition = player.GetPosition() + offset;
		isAiming = false;
    }

	void LateUpdate()
	{
		HandleAiming();
		HandleZoom();
		HandleCameraMovement();
	}

	void HandleAiming()
	{
        if (player.GetAiming())
        {
            SetAimingTarget(player.GetPosition());
        }
        else
        {
            SetTrackingTarget(player.GetPosition());
        }
    }

	void HandleZoom()
	{
		float scrollInput = Input.GetAxis("Mouse ScrollWheel");
		offset = offset.normalized * Mathf.Clamp(offset.magnitude - scrollInput * zoomSpeed, minZoom, maxZoom);
	}

	void HandleCameraMovement()
	{
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, 1f / cameraSpeed);
    }

	void SetTrackingTarget(Vector3 trackThis)
	{
		targetPosition = trackThis + offset;
	}

	void SetAimingTarget(Vector3 playerPivotTransform)
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
		if (groundPlane.Raycast(ray, out float rayLength))
		{
			Vector3 pointToLook = ray.GetPoint(rayLength);

			Vector3 direction = pointToLook - playerPivotTransform;

			if (direction.magnitude > cameraLimit)
			{
				direction = direction.normalized * cameraLimit;
			}

			targetPosition = playerPivotTransform + offset + direction;
		}
	}

	public void SetOffset(Vector3 newOffset)
	{
		offset = newOffset;
		currentZoom = offset.magnitude;
	}

	public void AllowPlayerControl(bool isEnabled)
	{
		allowPlayerControl = isEnabled;
	}

	public void ResetCamera()
	{
		currentZoom = offset.magnitude;
		transform.position = player.GetPosition() - offset.normalized * currentZoom;
		//transform.LookAt(playerTransform.position);
	}
}
