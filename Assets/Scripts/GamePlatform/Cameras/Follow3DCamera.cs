using UnityEngine;

/// <summary>
/// Free Follow 3D camera.
/// This script allows a free camera rotation around a target.
/// The GO hirarquy must fallow: Camera3DGO -> MainCamera.
/// Camera3DGO is a gameobject with a transform position where you want the camera's focus be.
/// Put this script inside Camera3DGO.
/// </summary>
public class Follow3DCamera : FollowCamera {

    public string resetCameraButtonName = "Reset_Camera";
    public float rotationSpeed = 1.5f;
    public float rotationSmoothing = 0.1f;
    public float maxHorizontalAxisRotation = 75f;       // The maximum value of the x axis rotation of the pivot.
    public float minHorizontalAxisRotation = 45f;

    protected Vector2 axisInput;

    protected bool resetCamera = false;
    protected Vector3 resetTargetForward;

    protected Vector2 smoothVelocity = Vector2.zero;
    protected Vector2 smooth = Vector2.zero;    

    private float lookAngle;                            // The rig's y axis rotation.
    private float tiltAngle;                            // The pivot's x axis rotation.

    protected override void ApplyCameraBehaviour()
    {
        axisInput += playerInput.lookAxis.Value;

        //reset camera position
        if (playerInput.GetButtonDown(resetCameraButtonName))
        {
            ResetCameraPosition();
        }

        if (resetCamera)
        {
            ResetingCamera();
        }
        else
            HandleRotationMovement();
    }

    private void ResetCameraPosition()
    {
        resetTargetForward = target.forward;
        resetCamera = true;
    }


    protected override void ResetingCamera()
    {
        Quaternion newrotation = Quaternion.Slerp(transform.rotation,
                                                   Quaternion.LookRotation(resetTargetForward),
                                                  Time.deltaTime * rotationSpeed * 10f);
        transform.rotation = newrotation;
        transform.RotateAround(target.position, Vector3.up, newrotation.x);
        if (Quaternion.Angle(newrotation, Quaternion.LookRotation(resetTargetForward)) < 1.5f)
        {
            lookAngle = transform.eulerAngles.y;
            tiltAngle = transform.localRotation.x;
            resetCamera = false;
        }
    }


    protected void HandleRotationMovement() {
        float speed = Time.deltaTime * rotationSmoothing;

        // smooth the user input
        smooth.x = Mathf.SmoothDamp(smooth.x, axisInput.x, ref smoothVelocity.x, speed);
        smooth.y = Mathf.SmoothDamp(smooth.y, axisInput.y, ref smoothVelocity.y, speed);


        // Adjust the look angle by an amount proportional to the turn speed and horizontal input.
        lookAngle += smooth.x * rotationSpeed;


        tiltAngle -= smooth.y * rotationSpeed;
        tiltAngle = Mathf.Clamp(tiltAngle, -minHorizontalAxisRotation, maxHorizontalAxisRotation);

        transform.rotation = Quaternion.Euler(tiltAngle, lookAngle, 0f);
    }
    
}
