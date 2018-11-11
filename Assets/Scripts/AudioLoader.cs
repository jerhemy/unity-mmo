using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioLoader : MonoBehaviour
{
	public string playFile;
	// Use this for initialization
	private AudioSource audio;

	void Awake()
	{
		audio = GetComponent<AudioSource>();
	}
	
	
	void Start ()
	{
		if (!String.IsNullOrWhiteSpace(playFile))
		{
			audio.clip = AssetManager.Music.FirstOrDefault(m => m.name == playFile);
			audio.Play();
		}
	}
}
