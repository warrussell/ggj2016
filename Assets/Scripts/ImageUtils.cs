using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class ImageUtils : MonoBehaviour 
{
	/// <summary>
	/// Gets the instance of ImageUtils.
	/// </summary>
	public static ImageUtils Instance
	{
		get
		{
			if(_instance == null) {
				_instance = GameObject.FindObjectOfType<ImageUtils>();
			}
			
			return _instance;
		}
	}
	private static ImageUtils _instance;
	
	public static Sprite CreateSprite(Texture2D texture)
	{
		return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
	}
	
	public void CaptureScreenshot(Action<Texture2D> OnComplete)
	{
		StartCoroutine(CaptureScreenshotCoroutine(OnComplete));
	}
	
	public static IEnumerator CaptureScreenshotCoroutine(Action<Texture2D> OnComplete)
	{
		yield return new WaitForEndOfFrame();
		
		int width = Screen.width;
		int height = Screen.height;
		Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
		
		texture.ReadPixels( new Rect(0, 0, width, height), 0, 0 );
		texture.Apply();
		
		OnComplete(texture);
	}	
	
	public static Color SetColorAlpha(Color color, float alpha)
	{
		return new Color(color.r, color.g, color.b, alpha);
	}
	
	//TODO: fix fadeRate to be time dependant, not by frame
	public static IEnumerator FadeImage(Image image, float fadeRate, float targetAlpha)
	{
		float currentAlpha = image.color.a;
		
		if (currentAlpha < targetAlpha) {
			while (currentAlpha < targetAlpha) {
				image.color = SetColorAlpha(image.color, currentAlpha + fadeRate);
				currentAlpha = image.color.a;
				yield return null;
			}
		} else if (currentAlpha > targetAlpha) {
			while (currentAlpha > targetAlpha) {
				image.color = SetColorAlpha(image.color, currentAlpha - fadeRate);
				currentAlpha = image.color.a;
				yield return null;
			}
		}
	}
	
	public static IEnumerator FadeText(Text text, float fadeRate, float targetAlpha)
	{
		float currentAlpha = text.color.a;
		
		if (currentAlpha < targetAlpha) {
			while (currentAlpha < targetAlpha) {
				text.color = SetColorAlpha(text.color, currentAlpha + fadeRate);
				currentAlpha = text.color.a;
				yield return null;
			}
		} else if (currentAlpha > targetAlpha) {
			while (currentAlpha > targetAlpha) {
				text.color = SetColorAlpha(text.color, currentAlpha - fadeRate);
				currentAlpha = text.color.a;
				yield return null;
			}
		}
	}
	
	public static IEnumerator FadeSpriteRenderer(SpriteRenderer sprite, float fadeRate, float targetAlpha)
	{
		float currentAlpha = sprite.color.a;
		
		if (currentAlpha < targetAlpha) {
			while (currentAlpha < targetAlpha) {
				sprite.color = SetColorAlpha(sprite.color, currentAlpha + fadeRate);
				currentAlpha = sprite.color.a;
				yield return null;
			}
		} else if (currentAlpha > targetAlpha) {
			while (currentAlpha > targetAlpha) {
				sprite.color = SetColorAlpha(sprite.color, currentAlpha - fadeRate);
				currentAlpha = sprite.color.a;
				yield return null;
			}
		}
	}
	
	public IEnumerator FadeAllChildrenUI(Transform obj, float startingAlpha, float targetAlpha, float fadeRate, Coroutine[] listToHoldCoroutines)
	{
		Image[] images = obj.GetComponentsInChildren<Image>();
		Text[] texts = obj.GetComponentsInChildren<Text>();
		listToHoldCoroutines = new Coroutine[images.Length + texts.Length];
		int i;
		int j;
		
		for (i = 0; i < images.Length; ++i)
		{
			images[i].color = ImageUtils.SetColorAlpha(images[i].color, startingAlpha);
		}
		
		for (j = 0; i < texts.Length; ++i)
		{
			texts[j].color = ImageUtils.SetColorAlpha(texts[j].color, startingAlpha);
		}
		
		if (images.Length > 0 && texts.Length > 0)
		{
			for (i = 0; i < images.Length; ++i)
			{
				listToHoldCoroutines[i] = StartCoroutine(ImageUtils.FadeImage(images[i], fadeRate, targetAlpha));
			}
			
			for (j = 0; i < texts.Length - 1; ++i)
			{
				listToHoldCoroutines[i + j] = StartCoroutine(ImageUtils.FadeText(texts[j], fadeRate, targetAlpha));
			}
			
			yield return listToHoldCoroutines[i + j] = StartCoroutine(ImageUtils.FadeText(texts[j], fadeRate, targetAlpha));
		}
		else if (images.Length > 0)
		{
			for (i = 0; i < images.Length - 1; ++i)
			{
				listToHoldCoroutines[i] = StartCoroutine(ImageUtils.FadeImage(images[i], fadeRate, targetAlpha));
			}
			
			yield return listToHoldCoroutines[i] = StartCoroutine(ImageUtils.FadeImage(images[i], fadeRate, targetAlpha));
		}
		else if (texts.Length > 0)
		{
			for (j = 0; i < texts.Length - 1; ++i)
			{
				listToHoldCoroutines[j] = StartCoroutine(ImageUtils.FadeText(texts[j], fadeRate, targetAlpha));
			}
			
			yield return listToHoldCoroutines[j] = StartCoroutine(ImageUtils.FadeText(texts[j], fadeRate, targetAlpha));
		}	
	}
}