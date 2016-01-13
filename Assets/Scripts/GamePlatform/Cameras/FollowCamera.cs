using UnityEngine;

/// <summary>
/// Abstract class for a fallow camera.
/// Camera may be controlled using mouse, gamepad or both
/// </summary>
public abstract class FollowCamera : MonoBehaviour {

    public Transform target;
    
    public bool followTarget = true;
    //public bool freezeWhenJump = true;
    public float dampingSpeed = 5f;   // speed when follow the target.    

    protected PlayerActor playerActor;
    protected Vector3 centerOffset = Vector3.zero;
    protected Vector3 originalcenterOffset;
    protected PlayerInput playerInput;

    // Use this for initialization
    protected virtual void Start () {
        centerOffset = transform.position - target.position;
        originalcenterOffset = centerOffset;
        playerActor = target.root.GetComponent<PlayerActor>();
        playerInput = target.root.GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    protected virtual void LateUpdate()
    {
        if (!followTarget)
            return;

        ApplyCameraBehaviour();
        FollowTarget();        
    }

    
    protected abstract void ResetingCamera();
    protected abstract void ApplyCameraBehaviour();

    protected virtual void FollowTarget()
    {
        // Move the rig towards target position.
        transform.position = Vector3.Lerp(transform.position, target.position + centerOffset,
                                          Time.deltaTime * dampingSpeed);
    }
}
