﻿using EasyCharacterMovement;
using System.Collections;
using UnityEngine;

namespace EasyCharacterMovement.CharacterMovementWalkthrough.CinemachineFirstPersonController
{
    public class FirstPersonController : MonoBehaviour
    {
        #region EDITOR EXPOSED FIELDS
        
        [Space(15f)]
        [Tooltip("Change in rotation per second (Deg / s).")]
        public float rotationRate = 540.0f;

        [Space(15f)]
        [Tooltip("The character's maximum speed.")]
        public float maxSpeed = 5.0f;

        [Tooltip("Max Acceleration (rate of change of velocity).")]
        public float maxAcceleration = 20.0f;

        [Tooltip("Setting that affects movement control. Higher values allow faster changes in direction.")]
        public float groundFriction = 8.0f;

        [Space(15f)]
        [Tooltip("Initial velocity (instantaneous vertical velocity) when jumping.")]
        public float jumpImpulse = 6.5f;

        [Tooltip("Friction to apply when falling.")]
        public float airFriction = 0.1f;

        [Range(0.0f, 1.0f)]
        [Tooltip("When falling, amount of horizontal movement control available to the character.\n" +
                 "0 = no control, 1 = full control at max acceleration.")]
        public float airControl = 0.3f;

        [Tooltip("The character's gravity.")]
        public Vector3 gravity = Vector3.down * 9.81f;

        [Space(15f)]
        [Tooltip("Character's height when standing.")]
        public float standingHeight = 2.0f;

        [Tooltip("Character's height when crouching.")]
        public float crouchingHeight = 1.25f;

        [Tooltip("The max speed modifier while crouching.")]
        [Range(0.0f, 1.0f)]
        public float crouchingSpeedModifier = 0.5f;

        [Header("Cinemachine")]
        [Tooltip("The first person rig camera pivot. This handles the Pitch rotation.")]
        public Transform cameraPivotTransform;

        [Tooltip("Cinemachine camera used while standing")]
        public GameObject standingCamera;

        [Tooltip("Cinemachine camera used while crouching")]
        public GameObject crouchingCamera;

        [Tooltip("Min pitch angle in degrees.")]
        public float minPitch = -89.0f;

        [Tooltip("Max pitch angle in degrees.")]
        public float maxPitch = +89.0f;

        [Tooltip("Mouse look horizontal sensitivity.")]
        public float lookHorizontalSensitivity = 1.0f;

        [Tooltip("Mouse look vertical sensitivity.")]
        public float lookVerticalSensitivity = 1.0f;

        #endregion

        #region FIELDS

        private Coroutine _lateFixedUpdateCoroutine;

        private float _yawInput;
        private float _pitchInput;

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Cached CharacterMovement component.
        /// </summary>

        public CharacterMovement characterMovement { get; private set; }

        /// <summary>
        /// Desired movement direction vector in world-space.
        /// </summary>

        public Vector3 movementDirection { get; set; }

        /// <summary>
        /// Jump input.
        /// </summary>

        public bool jump { get; set; }

        /// <summary>
        /// Crouch input command.
        /// </summary>

        public bool crouch { get; set; }

        /// <summary>
        /// Is the character crouching?
        /// </summary>

        public bool isCrouching { get; protected set; }

        #endregion

        #region EVENT HANDLERS

        /// <summary>
        /// Collided event handler.
        /// </summary>

        private void OnCollided(ref CollisionResult inHit)
        {
            Debug.Log($"{name} collided with: {inHit.collider.name}");
        }

        /// <summary>
        /// FoundGround event handler.
        /// </summary>

        private void OnFoundGround(ref FindGroundResult foundGround)
        {
            Debug.Log("Found ground...");

            // Determine if the character has landed

            if (!characterMovement.wasOnGround && foundGround.isWalkableGround)
            {
                Debug.Log("Landed!");
            }
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Clamps given angle within min - max range.
        /// </summary>

        private static float ClampAngle(float a, float min, float max)
        {
            while (max < min)
                max += 360.0f;

            while (a > max)
                a -= 360.0f;

            while (a < min)
                a += 360.0f;

            return a > max ? a - (max + min) * 0.5f < 180.0f ? max : min : a;
        }

        /// <summary>
        /// Update current yaw angle (along character's y-axis).
        /// </summary>
        
        public void AddYawInput(float value)
        {
            _yawInput = ClampAngle(_yawInput + value, 0.0f, 360.0f);
        }

        /// <summary>
        /// Update current pitch angle (along cameraPivot x-axis).
        /// </summary>

        public void AddPitchInput(float value)
        {
            _pitchInput = ClampAngle(_pitchInput + value, minPitch, maxPitch);
        }

        /// <summary>
        /// Handle Player input.
        /// </summary>

        private void HandleInput()
        {
            // Read Input values

            float horizontal = Input.GetAxisRaw($"Horizontal");
            float vertical = Input.GetAxisRaw($"Vertical");

            // Create a Movement direction vector relative to us

            movementDirection = Vector3.zero;

            movementDirection += transform.forward * vertical;
            movementDirection += transform.right * horizontal;

            // Make Sure it won't move faster diagonally

            movementDirection = Vector3.ClampMagnitude(movementDirection, 1.0f);
            
            // Jump input

            jump = Input.GetButton($"Jump");

            // Crouch input

            crouch = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C);

            // Look input

            float mouseX = Input.GetAxis($"Mouse X");
            float mouseY = Input.GetAxis($"Mouse Y");

            AddYawInput(mouseX * lookHorizontalSensitivity);
            AddPitchInput(mouseY * lookVerticalSensitivity);

            // Lock / Unlock mouse cursor

            if (Input.GetMouseButtonUp(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

            } else if (Input.GetKeyUp(KeyCode.Escape))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        /// <summary>
        /// Update character's rotation.
        /// </summary>

        private void UpdateRotation()
        {
            // Character handle left / right rotation (yaw)

            if (_yawInput != 0.0f)
                characterMovement.rotation *= Quaternion.Euler(0.0f, _yawInput, 0.0f);

            _yawInput = 0.0f;
        }

        /// <summary>
        /// Update camera's rotation.
        /// </summary>

        private void UpdateCameraRotation()
        {
            // Camera handle up / down rotation (pitch)

            if (_pitchInput != 0.0f)
                cameraPivotTransform.localRotation = Quaternion.Euler(-_pitchInput, 0.0f, 0.0f);
        }

        /// <summary>
        /// Move the character when on walkable ground.
        /// </summary>

        private void GroundedMovement(Vector3 desiredVelocity)
        {
            characterMovement.velocity = Vector3.Lerp(characterMovement.velocity, desiredVelocity,
                1f - Mathf.Exp(-groundFriction * Time.deltaTime));
        }

        /// <summary>
        /// Move the character when falling or on not-walkable ground.
        /// </summary>

        private void NotGroundedMovement(Vector3 desiredVelocity)
        {
            // Current character's velocity

            Vector3 velocity = characterMovement.velocity;

            // If moving into non-walkable ground, limit its contribution.
            // Allow movement parallel, but not into it because that may push us up.
            
            if (characterMovement.isOnGround && Vector3.Dot(desiredVelocity, characterMovement.groundNormal) < 0.0f)
            {
                Vector3 groundNormal = characterMovement.groundNormal;
                Vector3 groundNormal2D = groundNormal.onlyXZ().normalized;

                desiredVelocity = desiredVelocity.projectedOnPlane(groundNormal2D);
            }

            // If moving...

            if (desiredVelocity != Vector3.zero)
            {
                // Accelerate horizontal velocity towards desired velocity

                Vector3 horizontalVelocity = Vector3.MoveTowards(velocity.onlyXZ(), desiredVelocity,
                    maxAcceleration * airControl * Time.deltaTime);

                // Update velocity preserving gravity effects (vertical velocity)
                
                velocity = horizontalVelocity + velocity.onlyY();
            }

            // Apply gravity

            velocity += gravity * Time.deltaTime;

            // Apply Air friction (Drag)

            velocity -= velocity * airFriction * Time.deltaTime;

            // Update character's velocity

            characterMovement.velocity = velocity;
        }

        /// <summary>
        /// Handle character's Crouch / UnCrouch.
        /// </summary>

        private void Crouching()
        {
            // Process crouch input command

            if (crouch)
            {
                // If already crouching, return

                if (isCrouching)
                    return;

                // Set capsule crouching height

                characterMovement.SetHeight(crouchingHeight);

                // Update Crouching state

                isCrouching = true;

                // Toggle standing / crouching cameras

                standingCamera.SetActive(false);
                crouchingCamera.SetActive(true);
            }
            else
            {
                // If not crouching, return

                if (!isCrouching)
                    return;

                // Check if character can safely stand up

                if (!characterMovement.CheckHeight(standingHeight))
                {
                    // Character can safely stand up, set capsule standing height

                    characterMovement.SetHeight(standingHeight);

                    // Update crouching state

                    isCrouching = false;

                    // Toggle standing / crouching cameras

                    crouchingCamera.SetActive(false);
                    standingCamera.SetActive(true);
                }
            }
        }

        /// <summary>
        /// Handle jumping state.
        /// </summary>

        private void Jumping()
        {
            if (jump && characterMovement.isGrounded)
            {
                // Pause ground constraint so character can jump off ground

                characterMovement.PauseGroundConstraint();

                // perform the jump

                Vector3 jumpVelocity = Vector3.up * jumpImpulse;

                characterMovement.LaunchCharacter(jumpVelocity, true);
            }
        }

        /// <summary>
        /// Perform character movement.
        /// </summary>

        private void Move()
        {
            // Create our desired velocity using the previously created movement direction vector

            float targetSpeed = isCrouching ? maxSpeed * crouchingSpeedModifier : maxSpeed;

            Vector3 desiredVelocity = movementDirection * targetSpeed;

            // Update character’s velocity based on its grounding status

            if (characterMovement.isGrounded)
                GroundedMovement(desiredVelocity);
            else
                NotGroundedMovement(desiredVelocity);

            // Handle jumping state

            Jumping();

            // Handle crouching state

            Crouching();
            
            // Perform movement using character's current velocity

            characterMovement.Move();
        }

        /// <summary>
        /// Post-Physics update, used to sync our character with physics.
        /// </summary>

        private void OnLateFixedUpdate()
        {
            UpdateRotation();

            Move();
        }

        #endregion

        #region MONOBEHAVIOR

        private void Awake()
        {
            // Cache CharacterMovement component

            characterMovement = GetComponent<CharacterMovement>();

            // Enable default physic interactions

            characterMovement.enablePhysicsInteraction = true;

            // Lock and hide cursor

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void OnEnable()
        {
            // Start LateFixedUpdate coroutine

            if (_lateFixedUpdateCoroutine != null)
                StopCoroutine(_lateFixedUpdateCoroutine);

            _lateFixedUpdateCoroutine = StartCoroutine(LateFixedUpdate());

            // Subscribe to CharacterMovement events

            characterMovement.FoundGround += OnFoundGround;
            characterMovement.Collided += OnCollided;
        }

        private void OnDisable()
        {
            // Ends LateFixedUpdate coroutine

            if (_lateFixedUpdateCoroutine != null)
                StopCoroutine(_lateFixedUpdateCoroutine);

            // Un-Subscribe from CharacterMovement events

            characterMovement.FoundGround -= OnFoundGround;
            characterMovement.Collided -= OnCollided;
        }

        private IEnumerator LateFixedUpdate()
        {
            WaitForFixedUpdate waitTime = new WaitForFixedUpdate();

            while (true)
            {
                yield return waitTime;

                OnLateFixedUpdate();
            }
        }

        private void Update()
        {
            HandleInput();
        }

        private void LateUpdate()
        {
            UpdateCameraRotation();
        }

        #endregion
    }
}
