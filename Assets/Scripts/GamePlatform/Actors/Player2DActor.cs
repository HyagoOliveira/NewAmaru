using UnityEngine;

public class Player2DActor : PlayerActor {

	public Vector3 levelDirection = Vector3.right;
    protected float initialZ = 0f;

    public SideScrollingDirection Direction {
        get {
            return Vector3.Dot(lookingDirection, levelDirection) > 0 ? 
                SideScrollingDirection.RIGHT : SideScrollingDirection.LEFT;
        }
    }

    public override void SetSingleton(){
		Instance = this;
	}

    public override void SetInitialValues()
    {
        base.SetInitialValues();
        initialZ = transform.position.z;
    }

    public override void ApplyAnimation(){
		animator.SetFloat ("hSpeed", hspeedNormalized);
		animator.SetFloat ("vSpeed", vspeed);
		animator.SetBool ("grounded", Grounded);
	}

    public override void ApplyGroundMovement(){

        Vector3 input_direction = input.movementAxis.Value.x * levelDirection;

        if (input.IsMoving && input_direction.x != 0f)
        {
            lookingDirection = input_direction.normalized;
        }

        ApplyMeshRotation();

        moveDirection.x = speed * input_direction.x;
        moveDirection.z = speed * input_direction.z;
    }

    public override void ApplySlopeLimit()
    {
        if (OverSlope && IsMoving && FloorDistance < 0.1f)
        {
            RaycastHit hit2;
            if (Physics.SphereCast(footSphere.Position + Vector3.up, footSphere.Radius, Vector3.down, 
                out hit2, Mathf.Infinity, footSphere.contactLayer))
            {
                #if UNITY_EDITOR_WIN
                DebugDraw.DrawMarker(hit2.point, 1f, Color.black, 0f);
                #endif
                Vector3 pos = transform.position;
                pos.y = hit2.point.y;
                pos.z = initialZ;
                transform.position = pos;
                Grounded = true;
            }
        }
    }



    public override void OnPause(){}
    public override void OnResume(){}

    public override void UpdateActor(){}
}

public enum SideScrollingDirection{
	RIGHT,
	LEFT
}
