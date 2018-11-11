using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour {
	
	private static GameSceneManager instance = null;

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
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void LoadScene(string scenePath)
	{
		StartCoroutine(LoadSceneAsync(scenePath));
	}
	
	IEnumerator LoadSceneAsync(string scenePath)
	{
		//LoadingScene.SetActive (true);
     
		AsyncOperation async = SceneManager.LoadSceneAsync(scenePath);
 
		while (!async.isDone) {
			//LoadingBar.fillAmount = async.progress / 0.9f; //Async progress returns always 0 here    
			Debug.Log(async.progress);
			//textPourcentage.text = LoadingBar.fillAmount + "%"; //I have always 0% because he fillAmount is always 0
			yield return null;
 
		}
	}


	public void PlayMusic(string musicFile)
	{
		
	}
}
