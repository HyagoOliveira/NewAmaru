
using UnityEngine;

public class FlipBehaviour : GameplayItemComponent
{
    public const float FLIP_SPEED = 720f;
    public const int FLIP_TURNS = 1;
    private float angle = 0f;

    public override void OnAnimation()
    {
        angle += FLIP_SPEED * Time.deltaTime;
        transform.eulerAngles = Vector3.up * angle;

        if(angle > FLIP_TURNS * 360f)
        {
            StopAnimation();
            OnAnimationFinish();
        }
    }

    public override void StopAnimation()
    {
        angle = 0f;
        transform.rotation = Quaternion.identity;
        DoingAnimation = false;
    }

    public override void OnAnimationFinish()
    {
        OnConfirmAction();
    }
}
