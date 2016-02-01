using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StarController : MonoBehaviour
{
	[Header("Light Colours")]
	[SerializeField]
	protected Color _selectedColor;
	[SerializeField]
	protected Color _connectedColor;
	[SerializeField]
	protected Color _defaultColor;
	[SerializeField]
	protected Color _lookedAtColor;
	[SerializeField]
	protected float _lightPopIntensity;
	[SerializeField]
	protected float _lightDefaultIntensity;
	[Header("Audio Clips")]
	[SerializeField]
	protected List<AudioClip> _sfxPulses;
	protected AudioSource _audioSource;

	private Coroutine _popCoroutine;
	private Coroutine _scaleCoroutine;
	private Vector3 _originalScale;
	private Light _light;
	private bool _selected = false;
	private bool _connected = false;

	private void Awake()
	{
		_light = GetComponent<Light>();
		_light.color = _defaultColor;
		_originalScale = transform.localScale;
		_audioSource = GetComponent<AudioSource>();
		GazeManager.lookingAtStarted += HandleLookingAtStartedEvent;
		GazeManager.lookingAtStopped += HandleLookingAtStoppedEvent;
	}

	private void HandleLookingAtStartedEvent(Transform transform)
	{
		if (transform != null && this.transform == transform) {
			if (_popCoroutine != null) {
				transform.localScale = _originalScale; 
				StopCoroutine(_popCoroutine);
			} 
			_popCoroutine = StartCoroutine(Pop());
			if (!_selected) {
				_light.color = _lookedAtColor;
			}
		} else {
		/*	if (!_selected) {
				if (!_connected) {
					_light.color = _defaultColor;
				} else {
					_light.color = _connectedColor;
				} 
			} */    
		}
	}

	private void HandleLookingAtStoppedEvent(Transform transform)
	{
		if (transform != null && this.transform == transform) {
			if (_popCoroutine != null) {
				StopCoroutine(_popCoroutine);
			} 
			_popCoroutine = StartCoroutine(Shrink());
			if (!_selected) {
				_light.color = _defaultColor;
			}
		}
	}

	private IEnumerator Pop()
	{
		if (_scaleCoroutine != null) {
			StopCoroutine(_scaleCoroutine);
		}
		_light.range = _lightPopIntensity;
		_audioSource.clip = _sfxPulses[Random.Range(0, _sfxPulses.Count)];
		_audioSource.Play();
		yield return _scaleCoroutine = StartCoroutine(transform.ScaleTo(new Vector3(_originalScale.x * 1.5f, _originalScale.y * 1.5f, _originalScale.z * 1.5f), 2f, EaseType.BackOut));
	}

	private IEnumerator Shrink()
	{
		if (_scaleCoroutine != null) {
			StopCoroutine(_scaleCoroutine);
		}
		_light.range = _lightDefaultIntensity;
		yield return _scaleCoroutine = StartCoroutine(transform.ScaleTo(_originalScale, 2f, EaseType.BackOut));
	}

	public void Select()
	{
		if (!_selected) {
			_light.color = _selectedColor;
		}
		_selected = true;
	}

	public void Unselect()
	{
		if (_connected) {
			_light.color = _connectedColor;
		} else {
			_light.color = _defaultColor;
		}
		_selected = false;
	}

	public void Connect()
	{
		_light.color = _connectedColor;
		_connected = true;
		_selected = false;
	}

	public void Unconnect()
	{
		_light.color = _defaultColor;
		_connected = false;
		_selected = false;
	}

	public void DestroyThis()
	{
		GazeManager.lookingAtStarted -= HandleLookingAtStartedEvent;
		GazeManager.lookingAtStopped -= HandleLookingAtStoppedEvent;
		if (_scaleCoroutine != null) {
			StopCoroutine(_scaleCoroutine);
		}
		if (_popCoroutine != null) {
			StopCoroutine(_popCoroutine);
		}
		Destroy(this.gameObject);
	}
}