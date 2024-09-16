using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public float baseMoveSpeed = 10f;

	public float moveSpeed = 10f;
	public float acceleration = 10f;
	public float deceleration = 10f;
	public float rotationSpeed = 10f;

    public float characterOriginToFeetLength = 1.0f;

    public float runSpeedScalar;
    public float sneakSpeedScalar;

	public Animator animator;
    public int moveAnimState;
	public Rigidbody rb;

    [SerializeField]
	private Vector3 appliedVelocity = Vector3.zero;

    [SerializeField]
    private Vector3 targetVelocity = Vector3.zero;

	private Vector3 inputDirection;

    [SerializeField]
    private bool isGrounded;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

    }

    void Update()
    {
        HandleInput();


        ManageAnimator();
    }

    void FixedUpdate()
    {
        HandleMovement();

    }

    void LateUpdate()
    {
        HandleRotation();
    }
    
    void ManageAnimator()
    {
        animator.SetInteger("Movement State", moveAnimState);
        animator.SetFloat("Movement AnimSpeed", appliedVelocity.magnitude);
    }

    void HandleInput()
    {
        moveSpeed = baseMoveSpeed;
        moveAnimState = 0;

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        inputDirection = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        if (inputDirection != Vector3.zero)
        {
            moveAnimState = 2; //walking
        }

        if (Input.GetKey(KeyCode.Space))
        {
            moveSpeed = moveSpeed * sneakSpeedScalar;
            moveAnimState = 1; //focus walking
        } 
        else if (Input.GetKey(KeyCode.LeftShift)) 
        {
            moveSpeed = moveSpeed * runSpeedScalar;
            if (inputDirection != Vector3.zero)
            {
                moveAnimState = 3;
            }
        }
    }

    void HandleMovement()
    {
        targetVelocity = inputDirection * moveSpeed;

        // Smooth transition towards target velocity
        appliedVelocity = Vector3.Lerp(appliedVelocity, targetVelocity, (targetVelocity.magnitude > 0.1f ? acceleration : deceleration) * Time.fixedDeltaTime);



        // Move the player
        Vector3 newPosition = rb.position + appliedVelocity * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);

    }

    void HandleRotation()
    {
        if (inputDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(inputDirection);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

}