﻿using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Texture Provider.
/// This script allows you to store a pool of textures and use them after.
/// Put this script inside a game object manager.
/// </summary>
public class TextureProvider : Provider<Texture2D>
{
	public Texture2D[] textures;
	public static TextureProvider Instance { get; private set; }

	public override void Start ()
	{
		base.Start ();
		
		if (Instance == null)
			Instance = this;
	}

	#region implemented abstract members of IProvider
	public override void Register ()
	{
		_cache = new Dictionary<string, Texture2D> ();
		
		for (int i = 0; i < textures.Length; i++) {
			_cache.Add (textures [i].name, textures [i]);
		}
	}

	public override Texture2D Get (string name)
	{
		if (_cache.ContainsKey (name))
			return _cache [name];
		
		throw new UnityException ("No " + name + " registered!");
	}
	#endregion
}
