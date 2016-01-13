using UnityEngine;
using GamePlatform.Physics;

public class Amaru : Player2DActor {
    public Transform jetpackPoint;
    public Transform gunPoint;
    public float acceleration = 0.2f;
    private float originalSpeed;

    public SphereCollision headSphere;
    

    public override void SetInitialValues()
    {
        base.SetInitialValues();
        originalSpeed = speed;
    }

    public override void LoadPlatformStates()
    {
        base.LoadPlatformStates();

        StateController.GetState("Running").RegisterOnState(OnRunning);
    }    

    public override void UpdateActor()
    {
        if (input.GetButton("Run") && input.IsMoving)
        {
            Accelerate();
        }
        else if (speed > originalSpeed)
        {
            Desaccelerate();
        }


        if (headSphere.IsColliding())
        {
            IJumpableItem cube = headSphere.CollidingWith<IJumpableItem>();
            if (cube != null)
            {
                cube.OnHead();
                StopVelocity();
            }
        }

        if (input.GetButtonDown("Start"))
        {
            Debug.Break();
        }
    }

    private void Accelerate()
    {
        speed = Mathf.Clamp(speed + acceleration * Time.deltaTime, originalSpeed, maxSpeed);
    }

    private void Desaccelerate()
    {
        speed = Mathf.Clamp(speed - acceleration * 0.8f * Time.deltaTime, originalSpeed, maxSpeed);
    }

    protected override void ApplyLandLogic()
    {
        base.ApplyLandLogic();
        audioProvider.Play("land");
        prefabProvider.InstanciateDecal("SmokeWave");
        input.SetVibration(0.1f);
    }
        

    protected override void ApplyJumpImpulse()
    {
        base.ApplyJumpImpulse();
        audioProvider.Play("jump");
    }

    private void OnRunning()
    {
        //running in full speed
        if (speed > maxSpeed * 0.5f)
        {
            float frame = Time.frameCount % 25;
            if (frame == 0)
            {
                //TODO: Get a better footstep audio
                //PlayAudio("footsteps_leaves");
                InstanciatePrefab("SmokeUp");
            }            
        }
        //else
        //{
        //    float frame = Time.frameCount % 30;
        //    if (frame == 0)
        //    {
        //        PlayAudio("footsteps_leaves");
        //    }
        //}
    }

}
