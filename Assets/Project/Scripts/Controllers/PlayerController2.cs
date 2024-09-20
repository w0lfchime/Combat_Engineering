using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour, IPlayer
{
	[Header("Movement Settings")]
	public float baseMoveForce = 3f;
	public float beginReductionSpeed = 12.0f;
	public float approxMaxSpeed = 12f;
	public float equippedRifleSpeedModifer = -5.0f;
	public float jumpForce = 10f;          
	public float sprintForceFactor = 2f;
	public float groundCheckingDistance = 4f;

	[Header("Movement Variables")]
	public float targetMoveForce;
	public float moveForce;
	private Vector3 inputDirection;
	[SerializeField]
	private Vector3 lookDirection;
	[SerializeField]
	private bool isGrounded;
	[SerializeField]
	private float playerDrag;
	[SerializeField]
	private float playerCurrentSpeed;
	private float playerCurrentSpeedY;
	public float distanceToGround;

	//Character States
	private bool isSneaking;
	private bool isSprinting;
	private bool isAiming;
	private bool isFiring;
	private bool canFire;

	[Header("Physics Settings")]
	public float baseFlatDrag = 10f;
	public float groundDrag = 10f; 
	public float airDrag = 1f; 
	public float gravityMultiplier = 0.3f;
    public float groundedDistance = 0.1f;
	public float bonusGravity = 2.0f;

    [Header("Physics Variables")]
	private Rigidbody rb;
	private CapsuleCollider capsuleCollider;
	private Vector3 capsuleBottom;

	[Header("Animation Settings")]
	public float rotationSpeed = 10.0f;

	[Header("Animation Variables")]
	public Animator animator;
	public Transform upperBodyBone;

	[Header("Item")]
	public GameObject equippedItem;
    public Transform itemTransform;
	private Vector3 rifleDirection;
	private int equipState;
	//1 = nothing
	//2 = melee
	//3 = rifle
	public float fireDelay = 0.1f;
	public GunFire gunFiringEffect;
	public GameObject bulletPrefab;
	public float bulletSpeed = 100f;
	public float bulletLifeTime = 2f;
	

    [Header("Camera")]
    public Camera mainCamera;

	[Header("Controls")]
	public KeyCode jumpKey = KeyCode.Space;
	public KeyCode sprintKey = KeyCode.LeftShift;
	public KeyCode sneakKey = KeyCode.LeftControl;

	public KeyCode aimKey = KeyCode.Mouse1;
	public KeyCode fireKey = KeyCode.Mouse0;

	public KeyCode equipSlot1 = KeyCode.Alpha1;
	public KeyCode equipSlot2 = KeyCode.Alpha2;
    public KeyCode equipSlot3 = KeyCode.Alpha3;

    //IPlayer:
    public bool GetAiming()
	{
		return isAiming;
	}

	public Vector3 GetPosition()
	{
		return transform.position;
	}

	void Start()
	{
		rb = GetComponent<Rigidbody>();
		rb.drag = 0;
		capsuleCollider = GetComponent<CapsuleCollider>();
		animator = GetComponent<Animator>();

		moveForce = baseMoveForce;
		isFiring = false;
		//HACK:
		playerCurrentSpeed = 0f;
		isSneaking = false;
		isGrounded = false;
		canFire = true;
		
	}



	void Update()
	{
		HandleInputs();
		ManageItem();
		MovePlayer();
		HandleRotation();
		CheckGrounded();
		HandleJump();
		BasicInventory();
		AnimatorUpdates();
    }

	void ManageItem()
	{
		equippedItem.SetActive(isAiming);

		if (isFiring && canFire) 
		{
			StartCoroutine(Fire());
		}

	}

	void FireBullet()
	{
		float randomizedBulletSpeed = bulletSpeed + Random.Range(-12f, 12f);

		Quaternion bulletRotation = equippedItem.transform.rotation;



		GameObject bullet = Instantiate(bulletPrefab, equippedItem.transform.position, bulletRotation);

		Rigidbody brb = bullet.GetComponent<Rigidbody>();

		Vector3 bulletForward = bulletRotation * Vector3.forward;

		if (rb != null)
		{
			brb.velocity = -bulletForward * randomizedBulletSpeed;
		}
		Destroy(bullet, bulletLifeTime);
	}

	IEnumerator Fire()
	{
		canFire = false;

        if (gunFiringEffect != null)
        {
            gunFiringEffect.Fire();  // Trigger the firing effects in the gun script
        }
		FireBullet();

        yield return new WaitForSeconds(fireDelay);

		canFire = true;
	}

    private float upperBodyAnimatorWeight = 0f;
	private float maxAimingWalkSpeed = 4.5f;
    void AnimatorUpdates()
	{
		float upperBodyAnimatorTargetWeight = 0f;

		//if (isAiming)
		//{
		//	upperBodyAnimatorTargetWeight = 1f;
		//}

		upperBodyAnimatorWeight = Mathf.Lerp(upperBodyAnimatorWeight, upperBodyAnimatorTargetWeight, Time.deltaTime * 3.0f);

		animator.SetLayerWeight(1, upperBodyAnimatorWeight);

        float targetAnimSpeedFactor = Mathf.Clamp01(playerCurrentSpeed / approxMaxSpeed);


		float ab_blendFactor = distanceToGround / groundCheckingDistance;
		animator.SetFloat("AirborneBlendFactor", ab_blendFactor * .5f);

		animator.SetFloat("AnimSpeedFactor", targetAnimSpeedFactor);

		animator.SetBool("Grounded", isGrounded);
		animator.SetBool("Aiming", isAiming);


		if (isAiming)
		{
			float angle = Vector3.SignedAngle(rb.velocity.normalized, lookDirection, Vector3.up);

			Vector2 blendTreeInput = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

			blendTreeInput.Normalize();

			blendTreeInput = (blendTreeInput * playerCurrentSpeed) / maxAimingWalkSpeed;

			animator.SetFloat("8W_WalkX", blendTreeInput.x);
			animator.SetFloat("8W_WalkY", blendTreeInput.y);
		}
    }

	void HandleInputs()
	{
		targetMoveForce = baseMoveForce;
		isSprinting = false;
        if (Input.GetKey(sprintKey))
		{
			isSneaking = false;
			isSprinting = true;
		}

		isAiming = false;
		if (Input.GetKeyDown(aimKey))
		{
			gunFiringEffect.OnAim();
		}
		if (Input.GetKey(aimKey))
		{
			isSneaking = true;
			isAiming = true;
		}
		isFiring = false;
		if (isAiming && Input.GetKey(fireKey))
		{
			isFiring = true;
		}
		if (Input.GetKeyUp(aimKey))
		{
			isSneaking = false;
		}

        if (Input.GetKeyDown(sneakKey))
        {
            isSneaking = !isSneaking;
        }


		//Equip Slots
		if (Input.GetKeyDown(equipSlot1))
		{
			//Nothing
			equipState = 1;
		}
        if (Input.GetKeyDown(equipSlot2))
        {
			//Melee
			//TODO: Implement
        }
        if (Input.GetKeyDown(equipSlot3))
        {
			//Rifle
			equipState = 3;
        }



    }

	private void BasicInventory()
	{

	}

	private void MovePlayer()
	{
		//float beginReductionSpeed_local = beginReductionSpeed;

		//switch(equipState)
		//{
		//	case 1:
		//		//nothing
		//		break;
		//	case 2:
		//		//nothing
		//		break;

		//	case 3:

		//		break;

		//}

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
			targetMoveForce = baseMoveForce / 1.8f;
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


    //Use rb.rotation to for projectiles, hitboxes, etc
    void HandleRotation()
    {
        if (isAiming)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, transform.position);

            if (groundPlane.Raycast(ray, out float distance))
            {
                Vector3 mouseWorldPosition = ray.GetPoint(distance);
                Vector3 playerPosition = transform.position;

                // Calculate the direction from the player to the mouse world position
                lookDirection = (mouseWorldPosition - playerPosition);
            }
        }
        else if (playerCurrentSpeed > 0.1f)
        {
            lookDirection = rb.velocity.normalized * 3 + inputDirection;
        }

        lookDirection.y = 0;  // Keep the direction on the horizontal plane
		lookDirection.Normalize();
		if (lookDirection != Vector3.zero)
		{
			Quaternion targetRotation = Quaternion.LookRotation(lookDirection);

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
