using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameScreen : MonoBehaviour 
{	
	public enum AnimationType{None, Pop, PopAndFade, Fade, SlideFromLeft, SlideFromRight, SlideFromTop, SlideFromBottom}
	
	public ScreenManager.ScreenType type { get; protected set; }
	public ScreenManager.ScreenType source { get; protected set; }
	
	private RectTransform _rectTransform;
	private Canvas _canvas;
	
	private AnimationType _animationType;

	private Coroutine _scaleCoroutine;
	private Coroutine _moveCoroutine;
	private Coroutine _fadeCoroutine;
	private Coroutine[] _currentFadingCoroutines;
	
	public virtual void Setup(AnimationType animType)
	{
		_rectTransform = GetComponent<RectTransform>();
		_canvas = GetComponent<Canvas>();
		
		_animationType = animType;
		gameObject.SetActive(false);
	}
	
	public void Open()
	{
		gameObject.SetActive(true);
	
		switch (_animationType)
		{
			case AnimationType.None:
			{
				break;
			}
			case AnimationType.Pop:
			{
				Pop();
				break;
			}
			case AnimationType.PopAndFade:
			{
				Pop();
				FadeIn();
				break;
			}
			case AnimationType.Fade:
			{
				FadeIn();
				break;
			}
			case AnimationType.SlideFromLeft:
			{
				SlideFrom(-1080, 0);
				break;
			}
			case AnimationType.SlideFromRight:
			{
				SlideFrom(1080, 0);
				break;
			}
			case AnimationType.SlideFromTop:
			{
				SlideFrom(0, 1920);
				break;
			}
			case AnimationType.SlideFromBottom:
			{
				SlideFrom(0, -1920);
				break;
			}
		}
	}
	
	public virtual void SetSource(ScreenManager.ScreenType source)
	{
		this.source = source;
	}
	
	public virtual void Refresh()
	{
	
	}
	
	public virtual void Destroy()
	{
		DestroyImmediate(this.gameObject);
	}
	
	public void SetPosition(float x, float y)
	{
		_rectTransform.anchoredPosition = new Vector2(x, y);
	}

	public void Pop()
	{
		if (_scaleCoroutine != null) {
			StopCoroutine(_scaleCoroutine);
		}
		
		if (transform.GetChild(0).name == "Content") {
			transform.GetChild(0).localScale = Vector3.zero;
			_scaleCoroutine = StartCoroutine(transform.GetChild(0).ScaleTo(Vector3.one, 0.5f, EaseType.BackOut));
		}
	}
	
	public void FadeIn()
	{
		transform.localScale = Vector3.zero;
		
		if (_fadeCoroutine != null) {
			for (int i = 0; i < _currentFadingCoroutines.Length; ++i)
			{
				if (_currentFadingCoroutines[i] != null)
				{
					StopCoroutine(_currentFadingCoroutines[i]);
				}
			}
		
			StopCoroutine(_fadeCoroutine);
		}
		
		_fadeCoroutine = StartCoroutine(ImageUtils.Instance.FadeAllChildrenUI(transform, 0, 1, 0.02f, _currentFadingCoroutines));
	}
	
	
	
	public void SlideFrom(float x, float y)
	{
		SetPosition(x, y);
		
		if (_moveCoroutine != null) {
			StopCoroutine(_moveCoroutine);
		}
		
		_moveCoroutine = StartCoroutine(_rectTransform.MoveAnchorTo(Vector3.zero, 0.5f, EaseType.SineOut));
	}	
}