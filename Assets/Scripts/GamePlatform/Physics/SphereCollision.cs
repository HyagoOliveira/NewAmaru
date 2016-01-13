using UnityEngine;
using System.Collections.Generic;

public enum SphereCollisionType
{
	Foot,
	Head,
	Center,
	Other
}

public class SphereCollision : MonoBehaviour
{
	public float Radius = 0.2f;
	public float Height = 0f;
	public SphereCollisionType Type = SphereCollisionType.Foot;
	public Transform targetTransform;
	public LayerMask contactLayer;
	public Collider[] Colliders = new Collider[0];

	private int currentCollisionColliderIndex = 0;

	public Collider CollisionCollider {
		get{ return Colliders [currentCollisionColliderIndex];}
	}

	public Vector3 Position {
		get {
			if (targetTransform == null)
				return transform.position + transform.up * Height;

			return targetTransform.position + Vector3.up * Height;
		}
	}

	public readonly Dictionary<SphereCollisionType, Color> ColorDict = new Dictionary<SphereCollisionType, Color>
	{
		{SphereCollisionType.Foot, Color.green},
		{SphereCollisionType.Head, Color.blue},
		{SphereCollisionType.Center, Color.red},
		{SphereCollisionType.Other, Color.cyan}
	};

	public void Update ()
	{
		Colliders = Physics.OverlapSphere (Position, Radius, contactLayer);
	}

	public bool IsColliding ()
	{
		return Colliders.Length > 0;
	}

	public int CollidingCount ()
	{
		return Colliders.Length;
	}

	public bool IsCollidingWith<T> ()
	{
		for (int i = 0; i < Colliders.Length; i++) {
			T component = Colliders [i].gameObject.GetComponent<T> ();
			if (component != null) {
				currentCollisionColliderIndex = i;
				return true;
			}
		}

		return false;
	}

	public T CollidingWith<T> ()
	{
		for (int i = 0; i < Colliders.Length; i++) {
			if (Colliders [i] != null) {
				T component = Colliders [i].gameObject.GetComponent<T> ();
				if (component != null) {
					currentCollisionColliderIndex = i;
					return component;
				}
			}
		}
		
		return default (T);
	}

	void OnDrawGizmosSelected ()
	{
		Gizmos.color = ColorDict [Type];
		Gizmos.DrawWireSphere (Position, Radius);
	}
}
