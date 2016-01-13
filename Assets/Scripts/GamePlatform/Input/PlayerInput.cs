using UnityEngine;
using System;
using System.Collections.Generic;
using XInputDotNetPure;
using System.Collections;

/// <summary>
/// Player Input.
/// Receives inputs for ther player.
/// </summary>
public class PlayerInput : MonoBehaviour
{
	public Axis	movementAxis;
	public Axis lookAxis;

    public GamepadButtonNames[] buttonsNames;

    private Dictionary<string, string> _buttonsNames;

    void Awake()
    {
        RegisterButtons();
    }

    private void RegisterButtons()
    {
        _buttonsNames = new Dictionary<string, string>();
        for (int i = 0; i < buttonsNames.Length; i++)
        {
            _buttonsNames.Add(buttonsNames[i].Virtual, buttonsNames[i].Real);
        }
    }
    
	public void Stop ()
	{
		movementAxis.Value = Vector2.zero;
		lookAxis.Value = Vector2.zero;
	}
    
	public bool IsMoving {
		get{ return movementAxis.Value.sqrMagnitude > 0f;}
	}
	public bool IsMovingCamera {
		get{ return lookAxis.Value.sqrMagnitude > 0f;}
	}
	
	public float MovementIntensity {
		get {
			return Mathf.Abs (Mathf.Clamp (movementAxis.Value.sqrMagnitude, 0f, 1f));
		}
	}
	
	public float LookIntensity {
		get {
			return Mathf.Abs (Mathf.Clamp (lookAxis.Value.sqrMagnitude, 0f, 1f));
		}
	}
	
	// Update is called once per frame
	void Update ()
	{        
		movementAxis.Value = new Vector2 (Input.GetAxis (movementAxis.Horizontal), Input.GetAxis (movementAxis.Vertical));
		lookAxis.Value = new Vector2 (Input.GetAxis (lookAxis.Horizontal), Input.GetAxis (lookAxis.Vertical));
	}

    
    /// <summary>
    /// </summary>
    /// <param name="name">Button's name</param>
    /// <returns>True during the frame the player pressed down the virtual button identified by buttonName</returns>
    public bool GetButtonDown (string name)
	{
		return Input.GetButtonDown(name) || Input.GetButtonDown (_buttonsNames[name]);
	}

    /// <summary> 
    /// </summary>
    /// <param name="name">Virtual button's name</param>
    /// <returns>True the first frame the player releases the virtual button identified</returns>
    public bool GetButtonUp (string name)
	{
		return Input.GetButtonUp(name) || Input.GetButtonUp (_buttonsNames[name]);
	}
    
    /// <summary> 
    /// </summary>
    /// <param name="name">Virtual button's name</param>
    /// <returns>True while the virtual button identified by buttonName is held down by the player.</returns>
    public bool GetButton (string name)
	{
		return Input.GetButton(name) || Input.GetButton (_buttonsNames[name]);
	}

    /// <summary>
    /// </summary>
    /// <param name="name">Virtual axis's name</param>
    /// <returns>The value of the virtual axis identified by axisName.</returns>
    public float GetAxis(string name)
    {
        return Input.GetAxis(name);
    }

    public void SetVibration(float motor, float time = 0.5f)
    {
        GamePad.SetVibration(PlayerIndex.One, motor, motor);
        StartCoroutine(StopVibrationCoroutine(time));
    }

    public void SetVibration(float leftMotor, float rightMotor, float time = 0.5f)
    {
        GamePad.SetVibration(PlayerIndex.One, leftMotor, rightMotor);
        StartCoroutine(StopVibrationCoroutine(time));
    }

    public void StopVibration()
    {
        GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
    }

    private IEnumerator StopVibrationCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        StopVibration();
    }

    private void OnDisable()
    {
        StopVibration();
    }
}

[Serializable]
public struct Axis
{
	public string Horizontal;
	public string Vertical;
	
	public Vector2 Value;
}

[Serializable]
public class GamepadButtonNames
{
    public string Virtual;
    public string Real;
}
