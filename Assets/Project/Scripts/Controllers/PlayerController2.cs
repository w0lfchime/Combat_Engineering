using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{


	[Header("Movement Settings")]
	public float baseMoveForce = 3f;
	public float beginReductionSpeed = 14.0f;
	public float approxMaxSpeed = 15f;
	public float jumpForce = 5f;          
	public float sprintForceFactor = 2f;
	public float groundCheckingDistance = 4f;

	[Header("Movement Variables")]
	public float targetMoveForce;
	public float moveForce;
	private Vector3 inputDirection;
	[SerializeField]
	private bool isGrounded;
	[SerializeField]
	private float playerDrag;
	[SerializeField]
	private float playerCurrentSpeed;
	private float playerCurrentSpeedY;
	public float distanceToGround;
	private bool isSneaking;
	private bool isSprinting;

	[Header("Physics Settings")]
	public float baseFlatDrag = 100;
	public float groundDrag = 5f; 
	public float airDrag = 1f; 
	public float gravityMultiplier = 2f;
    public float groundedDistance = 0.1f;
	public float bonusGravity = 5.0f;

    [Header("Physics Variables")]
	private Rigidbody rb;
	private CapsuleCollider capsuleCollider;
	private Vector3 capsuleBottom;

	[Header("Animation Settings")]
	public float rotationSpeed = 5.0f;

	[Header("Animation Variables")]
	public Animator animator;







	[Header("Controls")]
	public KeyCode jumpKey = KeyCode.Space;
	public KeyCode sprintKey = KeyCode.LeftShift;
	public KeyCode sneakKey = KeyCode.LeftControl;



	void Start()
	{
		rb = GetComponent<Rigidbody>();
		rb.drag = 0;
		capsuleCollider = GetComponent<CapsuleCollider>();
		animator = GetComponent<Animator>();

		moveForce = baseMoveForce;

		isGrounded = false;

		//HACK:
		playerCurrentSpeed = 0f;
		isSneaking = false;
	}

	void Update()
	{
		HandleInputs();
		MovePlayer();
		HandleRotation();
		CheckGrounded();
		HandleJump();
		AnimatorUpdates();

    }


    void AnimatorUpdates()
	{
        float targetAnimSpeedFactor = Mathf.Clamp01(playerCurrentSpeed / approxMaxSpeed);


		float ab_blendFactor = distanceToGround / groundCheckingDistance;
		animator.SetFloat("AirborneBlendFactor", ab_blendFactor * .5f);

		animator.SetFloat("AnimSpeedFactor", targetAnimSpeedFactor);

		animator.SetBool("Grounded", isGrounded);
    }

	void HandleInputs()
	{
		if (Input.GetKeyDown(sneakKey))
		{
			isSneaking = !isSneaking;
		}

		targetMoveForce = baseMoveForce;
		isSprinting = false;
        if (Input.GetKey(sprintKey))
		{
			isSneaking = false;
			isSprinting = true;
		}



    }
	private void MovePlayer()
	{
		float moveX = Input.GetAxisRaw("Horizontal");
		float moveZ = Input.GetAxisRaw("Vertical");

		Vector3 moveDirection = Vector3.forward * moveZ + Vector3.right * moveX;
		moveDirection.Normalize();

		inputDirection = moveDirection;

		float targetDrag = isGrounded ? groundDrag : airDrag;

		float flatDrag = baseFlatDrag;

		float speedLimitingDragFactor = 1.0f;

		if (isSprinting)
		{
			targetMoveForce = baseMoveForce * 2;
		}
		if (isSneaking)
		{
			targetMoveForce = baseMoveForce / 2;
		}


		if (isGrounded)
		{
			playerDrag = Mathf.Lerp(playerDrag, groundDrag, Time.deltaTime * 1.0f);

			moveForce = Mathf.Lerp(moveForce, targetMoveForce, Time.deltaTime * 3.0f);

            if (playerCurrentSpeed > beginReductionSpeed)
            {
                float excess = playerCurrentSpeed - beginReductionSpeed;
                speedLimitingDragFactor *= Mathf.Pow(1 + excess, 2);
            }
        } 
		else
		{
			playerDrag = airDrag;
			moveForce = baseMoveForce / 20;
			flatDrag = flatDrag / 50;
		}

		Vector3 appliedforce = moveDirection * moveForce;

        Vector3 dragForce = rb.velocity * (playerDrag + flatDrag) * Time.fixedDeltaTime;

		dragForce *= speedLimitingDragFactor;

        appliedforce -= dragForce;

		appliedforce.y -= bonusGravity;

        rb.AddForce(appliedforce, ForceMode.Force);

		Vector3 speedOnZXPlane = rb.velocity;
		playerCurrentSpeedY = speedOnZXPlane.y;
        speedOnZXPlane.y = 0;

		playerCurrentSpeed = speedOnZXPlane.magnitude;
	}

	void HandleRotation()
	{
		if (playerCurrentSpeed > 0.1f)
		{
			Vector3 directionToPointIn = rb.velocity.normalized * 3 + inputDirection;

			//TODO: INVESTIGATE DYNAMIC AIRBONE ROTATION BY REMOVING
			directionToPointIn.y = 0;
			
			Quaternion targetRotation = Quaternion.LookRotation(directionToPointIn);
			Quaternion smoothedRotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.deltaTime);
			rb.MoveRotation(smoothedRotation);
		}
	}



    private float groundedSwitchCooldown = 0.2f; 
    private float lastGroundedCheckTime = 0f; 

    private void CheckGrounded()
    {
        capsuleBottom = transform.position;
        capsuleBottom.y += groundedDistance;

        Debug.DrawRay(capsuleBottom, Vector3.down * groundCheckingDistance, Color.red);


        RaycastHit hit;
		if (Physics.Raycast(capsuleBottom, Vector3.down, out hit, groundCheckingDistance, LayerMask.GetMask("Ground")))
        {
            distanceToGround = hit.distance;
        }
 
        bool newGroundedState = distanceToGround < groundedDistance * 2;


        if (Time.time - lastGroundedCheckTime >= groundedSwitchCooldown && newGroundedState != isGrounded)
        {
            isGrounded = newGroundedState;
            lastGroundedCheckTime = Time.time; 
        }
    }

    private void HandleJump()
	{
        if (isGrounded && Input.GetKeyDown(jumpKey))
		{
			// Apply upward force for jumping
			rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // Reset vertical velocity before jump
			rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Jump 
		}
	}
}
