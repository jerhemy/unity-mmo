using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.MMO.Client;
using UnityEngine;
using UnityEngine.SceneManagement;

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
		
		var e = new Entity();
		e.name = "Test";

		var data = StructTools.RawSerialize(e);
		var type = BitConverter.GetBytes((short) MessageType.Entity);
		
		var payload = type.Concat(data).ToArray();
		
		var x = StructTools.RawDeserialize<Entity>(payload, 2);
		
		//Debug.Log(x.name);
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
				go = GameObject.CreatePrimitive(PrimitiveType.Cube);
				go.AddComponent<EntityName>();
				go.GetComponent<EntityName>().id = (ulong) e.Value.id;
				go.GetComponent<EntityName>().name = e.Value.name;			
				go.transform.parent = npcContainer.transform;
			}		
			go.transform.position = new Vector3((float) e.Value.x, (float) e.Value.y, (float) e.Value.z);
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
