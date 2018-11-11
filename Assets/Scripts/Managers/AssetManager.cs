using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AssetManager : MonoBehaviour {

	private static AssetManager instance = null;
	
	public static AssetBundle BaseSceneBundle;
	public static List<string> BaseScenePaths;

	private AssetBundle MusicBundle;
	public static List<AudioClip> Music;
	
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
	void Start () {
		BaseSceneBundle = AssetBundle.LoadFromFile("Assets/AssetBundles/basescenes");
		BaseScenePaths = BaseSceneBundle.GetAllScenePaths().ToList();

		MusicBundle = AssetBundle.LoadFromFile("Assets/AssetBundles/music");
		Music = MusicBundle.LoadAllAssets<AudioClip>().ToList();
	}

	void loadBundle(string bundle)
	{
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
