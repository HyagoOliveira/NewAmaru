using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Player Provider.
/// This script allows you to store a pool of go prefabs and use them after.
/// Put this script inside your player.
/// </summary>
public class PlayerPrefabProvider : Provider<GameObject>
{
	public int defaultDestroyTime = 2;
    public GameObject[] prefabs;

    private PlayerActor actor;

	public override void Start ()
	{
		base.Start ();
		actor = GetComponent<PlayerActor> ();
	}


	public override void Register ()
	{
		_cache = new Dictionary<string, GameObject> ();
		
		for (int i = 0; i < prefabs.Length; i++) {
			_cache.Add (prefabs [i].name, prefabs [i]);
		}
	}
	public override GameObject Get (string prefabname)
	{
		if (_cache.ContainsKey (prefabname))
			return _cache [prefabname];
		
		throw new UnityException ("No " + prefabname + " prefab registered!");
	}

	public void InstanciateAnimation (string prefabname)
	{
		GameObject prefab = Get (prefabname);
		GameObject.Instantiate (prefab, transform.position, prefab.transform.rotation);
	}

	public void Instanciate (string prefabname)
	{
		Instanciate (prefabname, defaultDestroyTime);
	}

	public void Instanciate (string prefabname, float destroyTime)
	{
		GameObject prefab = Get (prefabname);
		GameObject instance = GameObject.Instantiate (prefab, transform.position + prefab.transform.position, 
		                                              prefab.transform.rotation) as GameObject;

		Destroy (instance, destroyTime);
	}

	public void Instanciate (string prefabname, Vector3 position, float destroyTime = 1f)
	{
		GameObject prefab = Get (prefabname);
		GameObject instance = GameObject.Instantiate (prefab, position, 
		                                              prefab.transform.rotation) as GameObject;
		
		Destroy (instance, destroyTime);
	}

	public void InstanciateDecal (string prefabname)
	{
		GameObject prefab = Get (prefabname);
		Vector3 position = transform.position + prefab.transform.position;
		Quaternion rot = prefab.transform.rotation;

		if (actor != null) {
			position.y = actor.FarBottomHit.point.y + prefab.transform.position.y;
			rot = Quaternion.LookRotation (actor.FarBottomHit.normal);
		}

		GameObject instance = GameObject.Instantiate (prefab, position, rot) as GameObject;

		Destroy (instance, defaultDestroyTime);
	}
}
