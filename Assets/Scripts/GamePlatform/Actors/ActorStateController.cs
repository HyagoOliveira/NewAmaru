using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Actor State Controller.
/// This script allows a control actors' states like Running, Jumping Standing etc.
/// Put this script inside an Actor.
/// </summary>
public class ActorStateController
{
	public Dictionary<string, ActorState> States{ get; private set; }
	public string CurrentStateKey { get; private set; }
	public string LastStateKey { get; private set; }

	public ActorState CurrentState { get { return States [CurrentStateKey]; } }
	public string CurrentAnimationName { get { return States [CurrentStateKey].AnimationName; } }

	private int lastIndex;

	public ActorStateController()
	{
		States = new Dictionary<string, ActorState> ();
		Clear ();
	}

	public ActorStateController(params ActorState[] states) : this()
	{
		AddStates (states);		

		//get first State's name
		CurrentStateKey = States.GetEnumerator ().Current.Value.Name;
	}
	

	public void UpdateStates ()
	{
		States [CurrentStateKey].Update ();
		States [CurrentStateKey].CurrentTime += Time.deltaTime;
	}


	public void ChangeState (string toStateName)
	{
		if (lastIndex == States [toStateName].Index)
			return;

		if (!string.IsNullOrEmpty (CurrentStateKey)) {
			if (!States.ContainsKey (toStateName))
				throw new Exception ("State " + toStateName + " was not registered...");

			States [CurrentStateKey].OnExitState ();
		}

		LastStateKey = CurrentStateKey;
		CurrentStateKey = toStateName;
		lastIndex = States [CurrentStateKey].Index;

		States [CurrentStateKey].OnEnterState ();
	}


	public void AddState (ActorState state, bool current = false)
	{
		States.Add (state.Name, state);

		if (current)
			CurrentStateKey = state.Name;
	}

	public void AddStates (params ActorState[] states)
	{
		for (int i=0; i<states.Length; i++) {
			AddState (states [i]);
		}
	}

	public void RemoveState (ActorState state)
	{
		States.Remove (state.Name);
	}

	public void RemoveState (string stateName)
	{
		States.Remove (stateName);
	}

	public ActorState GetState (string stateName)
	{
		if (!States.ContainsKey (stateName))
			throw new Exception ("State " + stateName + " was not registered...");
		
		return States [stateName];
	}

	public string GetAnimationName (string stateName)
	{
		return GetState (stateName).AnimationName;
	}

	public void Clear ()
	{
		LastStateKey = string.Empty;
		lastIndex = 0;
		CurrentStateKey = null;
		States.Clear ();		
	}


}
