using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScreenManager : MonoBehaviour 
{
	public enum ScreenType{	
							Prototype, 
							Null
						  }

	/// <summary>
	/// Gets the instance of ScreenManager.
	/// </summary>
	public static ScreenManager Instance
	{
		get
		{
			if(_instance == null) {
				_instance = GameObject.FindObjectOfType<ScreenManager>();
			}
			
			return _instance;
		}
	}
	private static ScreenManager _instance;
	
	private Stack<GameScreen> _screenLayers;
	private GameScreen _nextScreen;
	
	private void Start()
	{
		DontDestroyOnLoad(this.gameObject);
		_screenLayers = new Stack<GameScreen>();
	}
	
	private void OnApplicationFocus(bool focusStatus)
	{
		if (focusStatus) {
			RefreshScreen();
		}
	}
	
	public void RefreshScreen()
	{
		if (_screenLayers != null && _screenLayers.Count > 0) _screenLayers.Peek().Refresh();
	}
	
	private GameScreen CreateScreen(ScreenType screenType)
	{
		GameObject screenPrefab = Instantiate(Resources.Load(screenType.ToString())) as GameObject;
		return screenPrefab.GetComponent<GameScreen>();
	}
	
	public void PushScreen(ScreenType screenType, GameScreen.AnimationType animType)
	{
		ScreenType source = GetCurrentScreenType();
		PushScreenWithSource(screenType, animType, source);
	}
	
	public void ReplaceScreen(ScreenType screenType, GameScreen.AnimationType animType)
	{
		ScreenType source = GetCurrentScreenType();
		CloseScreen();
		PushScreenWithSource(screenType, animType, source);
	}
	
	private void PushScreenWithSource(ScreenType screenType, GameScreen.AnimationType animType, ScreenType source)
	{
		if (screenType == ScreenType.Null) {
			throw new System.Exception("Trying to create null screen type");
		}	
		
		GameScreen screen = CreateScreen(screenType);
		screen.Setup(animType);
		screen.Open();
		screen.SetSource(source);
		_screenLayers.Push(screen);
	}
	
	public void NextScreen(ScreenType screenType, GameScreen.AnimationType animType)
	{
		GameScreen screen = CreateScreen(screenType);
		screen.Setup(animType);
		_nextScreen = screen;
	}
	
	public void CloseScreen(ScreenType screenType)
	{
		if (_screenLayers.Peek().type == screenType) {
			CloseScreen();
		}
	}
	
	private void CloseScreen()
	{
		if (_screenLayers.Count > 0) {
			GameScreen screen = _screenLayers.Pop();
			screen.Destroy();
			
			if (_nextScreen != null) {
				_nextScreen.Open();
				_screenLayers.Push(_nextScreen);
				_nextScreen = null;
			}		
		}
	}
	
	public ScreenType GetCurrentScreenType()
	{
		if (_screenLayers.Count > 0) return _screenLayers.Peek().type;
		else return ScreenType.Null;
	}
}