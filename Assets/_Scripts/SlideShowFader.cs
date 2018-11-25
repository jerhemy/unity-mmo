using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SlideShowFader : MonoBehaviour {

	public Sprite backgroundImage;
	public List<Sprite> spashScreenImages = new List<Sprite>();

	public float Delay;
	private Image _image;
	public Image Panel;
	
	private int index = 0;
	
	// Use this for initialization
	void Start ()
	{
		_image = Panel.gameObject.GetComponentsInChildren<Image>()[1];
		InvokeRepeating("StartSlideShow", 1.0f, Delay);
	}
	
	void StartSlideShow()
	{

		if (index < spashScreenImages.Count)
		{
			_image.sprite = spashScreenImages[index];
			index++;
		}
		else
		{
			CancelInvoke();
			var loginScene = AssetManager.BaseScenePaths.Find(scene => scene.Contains("Login"));
			FindObjectOfType<GameSceneManager>().LoadScene(loginScene);
			//SceneManager.LoadSceneAsync(AssetManager.BaseScenePaths[1], LoadSceneMode.Single);
		}
	}
}
