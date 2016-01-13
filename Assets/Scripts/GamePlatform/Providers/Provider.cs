using UnityEngine;
using System.Collections.Generic;

public abstract class Provider<T> : MonoBehaviour
{
	protected Dictionary<string, T> _cache;

	public virtual void Start ()
	{
		Register ();
	}

	public abstract void Register ();

	public abstract T Get (string name);
}
