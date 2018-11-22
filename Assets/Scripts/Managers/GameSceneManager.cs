using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityMMO.Manager;
using UnityMMO.Models;

public class GameSceneManager : MonoBehaviour {
	
	private static GameSceneManager instance = null;
	private static ConcurrentDictionary<ulong, Entity> entities = new ConcurrentDictionary<ulong, Entity>();
	private GameObject npcContainer;
	
	void Awake()
	{
		//Check if instance already exists
		if (instance == null)
                
			//if not, set instance to this
			instance = this;
            
		//If instance already exists and it's not this:
		else if (instance != this)
                
			//Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
			Destroy(gameObject);    
            
		//Sets this to not be destroyed when reloading scene
		DontDestroyOnLoad(gameObject);
	}
	
	// Use this for initialization
	void Start ()
	{
		npcContainer = new GameObject("NPCs");
		UnityConnectionManager.OnEntityReceived += EntityReceived;
	}
	
	// Update is called once per frame
	void Update ()
	{
		//entities.Where(e => e.Key != UnityConnectionManager.getClientId());
		var foundObjects = FindObjectsOfType<EntityName>().ToList();
		
		foreach (KeyValuePair<ulong,Entity> e in entities)
		{
			var go = foundObjects.FirstOrDefault(x => x.id == (ulong) e.Value.id)?.gameObject;
			if (go == null)
			{
				//Create Entity
				go = GameObject.CreatePrimitive(PrimitiveType.Cube);
				go.AddComponent<EntityName>();
				go.AddComponent<CharacterController>();
				var eName = go.GetComponent<EntityName>();
				eName.id = (ulong) e.Value.id;
				eName.name = e.Value.name;
				
				go.transform.parent = npcContainer.transform;
			}

			var v3 = e.Value.loc.ToVector3();
			v3.y += 1;
			go.transform.position = v3;
			//var CC = go.GetComponent<CharacterController>();
			//CC.transform.TransformDirection()
		}
	}

	public void LoadScene(string scenePath)
	{
		StartCoroutine(LoadSceneAsync(scenePath));
	}
	
	IEnumerator LoadSceneAsync(string scenePath)
	{
		//LoadingScene.SetActive (true);
		UnityConnectionManager.OnEntityReceived -= EntityReceived;
		AsyncOperation async = SceneManager.LoadSceneAsync(scenePath);
 
		while (!async.isDone) {
			//LoadingBar.fillAmount = async.progress / 0.9f; //Async progress returns always 0 here    
			Debug.Log(async.progress);
			//textPourcentage.text = LoadingBar.fillAmount + "%"; //I have always 0% because he fillAmount is always 0
			UnityConnectionManager.OnEntityReceived += EntityReceived;
			yield return null;
		}
	}


	public void PlayMusic(string musicFile)
	{
		
	}

	private void EntityReceived(Entity entity)
	{
		try
		{
			entities.TryAdd((ulong) entity.id, entity);
		}
		catch (Exception e)
		{
			Console.WriteLine(e.Message);
		}
	}
}
