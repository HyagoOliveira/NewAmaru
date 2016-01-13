using UnityEngine;
using System;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(SphereCollision))]
[RequireComponent(typeof(AudioProvider))]
[RequireComponent(typeof(PlayerPrefabProvider))]
public abstract class PlayerActor : MonoBehaviour, IDynamicActor, IPauseableEntity, ISingletonManager {

    #region variables

    public float speed = 4f;
    public float maxSpeed = 6f;
    public float rotateSpeed = 15f;
    public float jumpHeight = 12f;
    public float jumpExtendedMult = .5f;
    public bool canControl = true;
    public bool canJump = true;
    public bool doubleJump = true;
    public float gravity = 20f;
    public Transform animatedMesh;
    public Transform centralBone;

    protected int lastLandFrame = 0;
    //Player can jump again 4 frames after landed
    protected readonly int MIN_ALLOWED_FRAME_JUMP_COUNT = 4; 

    
    protected Vector3 moveDirection = Vector3.zero;

    public PlayerInput input { get; protected set; }
    public Animator animator { get; protected set; }
    public CharacterController characterController { get; protected set; }

    public Vector3 lookingDirection { get; protected set; }

    public bool Grounded { get; protected set; }
    public bool IsMoving { get { return hspeed > 0f; } }
    public bool Jumping { get; protected set; }


    protected SphereCollision footSphere;

    public ActorStateController StateController { get; protected set; }

    public static PlayerActor Instance { get; protected set; }

    public AudioProvider audioProvider { get; protected set; }
    public PlayerPrefabProvider prefabProvider { get; protected set; }

    public static Vector3 CentralPosition { get { return Instance.centralBone.position; } }

    public Vector3 RespawnPosition { get; set; }

    public RaycastHit FarBottomHit
    {
        get { return _farBottomHit; }
        set { _farBottomHit = value; }
    }
    private RaycastHit _farBottomHit;

    public float FloorAngle
    {
        get { return Vector3.Angle(_farBottomHit.normal, Vector3.up); }
    }

    public float FloorDistance
    {
        get { return Vector3.Distance(transform.position, _farBottomHit.point); }
    }

    public Vector3 GetBoundsSize
    {
        get { return animatedMesh.GetComponent<Renderer>().bounds.size; }
    }

    public float GetWidth
    {
        get { return GetBoundsSize.x; }
    }

    public float GetHeight
    {
        get { return GetBoundsSize.y; }
    }

    public float vspeed { get; private set;}
    public float hspeed { get; private set;}
    public float overallSpeed { get; private set; }

    public float hspeedNormalized { get { return hspeed / maxSpeed; } }
    public bool OverSlope { get { return FloorAngle > 0.5f; } }

    protected bool canDoDoubleJump;

    #endregion

    protected virtual void Awake()
    {
        SetSingleton();
    }

    // Use this for initialization
    protected virtual void Start()
    {
        SetInitialValues();
    }

    // Update is called once per frame
    protected virtual void Update()
    {        
        if (canControl)
        {
            UpdateActor();
            ApplyHorizontalMovement();
        }

        ApplyVerticalMovement();

        UpdatePhysics();

        ApplyMovement();
        ApplyUpdateStateMachine();
        ApplyAnimation();        
    }

    public abstract void SetSingleton();
    public abstract void ApplyAnimation();    
    public abstract void ApplyGroundMovement();
    public abstract void UpdateActor();
    public abstract void OnPause();
    public abstract void OnResume();

    public virtual void ApplyHorizontalMovement()
    {        
        ApplyGroundMovement ();
        if (canJump)
            ApplyJump ();        
    }

    public virtual void ApplyJump()
    {
        if (input.GetButtonDown ("Jump") && Grounded && isJumpFrameTimeOk()) {
            Jumping = true;
            ApplyJumpImpulse ();
        }
        
        ApplyDoubleJump();

        if (input.GetButton ("Jump") && !Grounded && vspeed > 0f) {
            moveDirection.y += jumpHeight * jumpExtendedMult * Time.deltaTime;
        }
    }

    private bool isJumpFrameTimeOk()
    {
        return Time.frameCount - lastLandFrame > MIN_ALLOWED_FRAME_JUMP_COUNT;
    }

    protected virtual void ApplyJumpImpulse ()
    {
        ApplyJumpAnimation ();
        moveDirection.y = jumpHeight;        
    }

    protected virtual void ApplyJumpAnimation ()
    {
        animator.SetTrigger ("jump");
    }
    

    public virtual void ApplyMovement()
    {
        if (gameObject.activeInHierarchy)
        {
            characterController.Move(moveDirection * Time.deltaTime);
            Vector3 horizontalVelocity = new Vector3(characterController.velocity.x, 0f, characterController.velocity.z);
            hspeed = horizontalVelocity.magnitude;
            vspeed = characterController.velocity.y;
            overallSpeed = characterController.velocity.magnitude;
        }
    }

    public void ApplyMovement(Vector3 mov)
    {
        characterController.Move(mov * Time.deltaTime);
    }

    public virtual void ApplyRespawn()
    {
        transform.position = RespawnPosition;
    }

    public virtual void ApplySlopeLimit()
    {
        if (OverSlope && IsMoving && FloorDistance < 0.1f)
        {
            RaycastHit hit2;
            if (Physics.SphereCast(footSphere.Position + Vector3.up, footSphere.Radius, Vector3.down, out hit2, Mathf.Infinity,
            footSphere.contactLayer))
            {
                #if UNITY_EDITOR_WIN
                DebugDraw.DrawMarker(hit2.point, 1f, Color.black, 0f);
                #endif
                Vector3 pos = transform.position;
                pos.y = hit2.point.y;
                transform.position = pos;
                Grounded = true;
            }
        }
    }

    public virtual void ApplyUpdateStateMachine()
    {
        StateController.UpdateStates();

        if (Grounded)
        {
            ApplyUpdateGroundStateMachine();
        }
        else
        {
            ApplyUpdateAirStateMachine();
        }
    }

    public virtual void ApplyUpdateGroundStateMachine()
    {
        if (hspeed > 0.1f)
            StateController.ChangeState("Running");
        else
            StateController.ChangeState("Idle");
    }

    public virtual void ApplyUpdateAirStateMachine()
    {
        if (vspeed > 0.1f)
            StateController.ChangeState("Jumping");
        else
            StateController.ChangeState("Falling");
    }

    public virtual void ApplyVerticalMovement()
    {
        //if actor is on air
        if (!Grounded)
        {
            // and it has reach max vertical air speed (gravity)
            if (moveDirection.y > -gravity)
            {
                moveDirection.y -= gravity * Time.deltaTime;
            }

            //and it is falling and very close to floor (has landed)
            if(footSphere.IsColliding())
            {
                ApplyLandLogic();
            }
        }
        
    }

    public virtual void LoadPlatformStates()
    {
        StateController = new ActorStateController();

        StateController.AddState(new ActorState("Idle"), true);
        StateController.AddState(new ActorState("Running"));
        StateController.AddState(new ActorState("Jumping"));
        StateController.AddState(new ActorState("Falling"));
    }

    public virtual void OnFellOffWorld()
    {
        ApplyRespawn ();
    }

    public virtual void SetInitialValues()
    {
        input = GetComponent<PlayerInput> ();
		animator = GetComponent<Animator> ();
		characterController = GetComponent<CharacterController> ();
		footSphere = GetComponent<SphereCollision> ();
        footSphere.Type = SphereCollisionType.Foot;
        audioProvider = GetComponent<AudioProvider>();
        prefabProvider = GetComponent<PlayerPrefabProvider>();        
        lookingDirection = transform.forward;
		canDoDoubleJump = doubleJump;
		LoadPlatformStates ();
		FixInGround ();
		RespawnPosition = transform.position;
    }
        

    public virtual void UpdatePhysics()
    {
        if (!Physics.Raycast(footSphere.Position, Vector3.down, out _farBottomHit, 
            Mathf.Infinity, footSphere.contactLayer) && 
            vspeed <= -gravity)
        {
            OnFellOffWorld();
        }


        Grounded = footSphere.IsColliding();
        ApplySlopeLimit();

        //#if UNITY_EDITOR_WIN
        //DebugDraw.DrawVector (footSphere.Position, Vector3.down, FarBottomHit.distance, .2f, Color.red, 0f, false);
        //#endif
    }

    public virtual void ApplyMeshRotation(){
        // Rotate our mesh to face where we are "looking"
        transform.rotation = Quaternion.Slerp (transform.rotation, 
                                                  Quaternion.LookRotation (lookingDirection), 
                                                  Time.deltaTime * rotateSpeed);
        animatedMesh.rotation = transform.rotation;
    }
    

    

    protected virtual void FixInGround()
    {
        UpdatePhysics();
        transform.position = FarBottomHit.point;
        Grounded = true;
    }

    protected virtual void ApplyLandLogic()
    {
        //StopVelocity();
        Jumping = false;
        animator.ResetTrigger("jump");
        canDoDoubleJump = doubleJump;
        moveDirection.y = 0;
        lastLandFrame = Time.frameCount;
    }

    public virtual void StopVelocity()
    {
        moveDirection = Vector3.zero;
        characterController.Move(moveDirection);
        hspeed = vspeed = 0f;
        ApplyAnimation();
    }

    protected virtual void ApplyDoubleJump()
    {
        if (canDoDoubleJump && input.GetButtonDown("Jump") && !Grounded)
        {
            canDoDoubleJump = false;
            ApplyJumpImpulse();
        }
    }

    public virtual void TurnOff()
    {
        canControl = false;
        StopAllCoroutines();
        StopVelocity();
        input.Stop();
        enabled = canControl;
    }

    public virtual void TurnOn()
    {
        canControl = true;
        enabled = canControl;
    }

    public void PlayAudio(string name)
    {
        audioProvider.Play(name);
    }

    public void InstanciatePrefab(string name)
    {
        prefabProvider.InstanciateDecal(name);
    }
}
