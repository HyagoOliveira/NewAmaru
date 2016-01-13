using UnityEngine;

[RequireComponent(typeof(PlayerActor))]
public class PlatformStateDebug : MonoBehaviour
{
    private PlayerActor playerActor;

    private float timeScale = 1.0f;
	private float deltaTime;


	void Start ()
	{
        playerActor = GetComponent<PlayerActor>();
        deltaTime = Time.deltaTime;        
    }

	void Update ()
	{
        playerActor.enabled = !playerActor.input.GetButton("DEBUG_BUTTON");
        if (!playerActor.enabled)
        {
            if (playerActor.vspeed < 0)
                playerActor.StopVelocity();

            Vector2 DPADinput = new Vector2(playerActor.input.GetAxis("XBOX_DPAD_Horizontal"),
                                    playerActor.input.GetAxis("XBOX_DPAD_Vertical"));

            if (DPADinput.sqrMagnitude > 0)
            {
                Vector3 direction = DPADinput.x *
                    Camera.main.transform.right + DPADinput.y * Vector3.up;

                playerActor.ApplyMovement(direction * playerActor.speed);
            }
            else
            {
                playerActor.ApplyGroundMovement();
                playerActor.ApplyMovement();
            }
        }

        Vector2 triggerInput = new Vector2 (Input.GetAxis ("XBOX_Left_Trigger"), 
		                                   Input.GetAxis ("XBOX_Right_Trigger"));

		if (triggerInput.sqrMagnitude > 0f) {
			timeScale -= triggerInput.x * deltaTime;
			timeScale += triggerInput.y * deltaTime;
		}

		timeScale = Mathf.Clamp (timeScale, 0.01f, 1f);
		Time.timeScale = timeScale;
	}



	void OnGUI ()
	{
		float topY = Screen.height - 100;

		GUI.Box (new Rect (10, topY + 10, 200, 80), "Player Machine");

		GUI.TextField (new Rect (20, topY + 40, 180, 20), 
		               string.Format ("State: {0}", playerActor.StateController.CurrentState));
		timeScale = GUI.HorizontalSlider (new Rect (20, topY + 70, 180, 20), timeScale, 0.01f, 1.0f);
	}
}
