using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlideShowFader : MonoBehaviour {

	public Sprite backGround;
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
	}

	void Update()
	{
//		Color curColor = _image.color;
//		float alphaDiff = Mathf.Abs(curColor.a - targetAlpha);
//		if (alphaDiff > 0.0001f)
//		{
//			curColor.a = Mathf.Lerp(curColor.a, targetAlpha, FadeRate * Time.deltaTime);
//			_image.color = curColor;
//			Debug.Log(_image.color.a);
//		}
	}

}
