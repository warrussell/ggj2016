using UnityEngine;
using System.Collections;

public class StarController : MonoBehaviour
{
	[SerializeField]
	protected Color _selectedColor;
	[SerializeField]
	protected Color _connectedColor;
	[SerializeField]
	protected Color _defaultColor;
	[SerializeField]
	protected Color _lookedAtColor;


	private Coroutine _popCoroutine;
	private Vector3 _originalScale;
	private Light _light;
	private bool _selected = false;
	private bool _connected = false;

	private void Awake()
	{
		_light = GetComponent<Light>();
		_light.color = _defaultColor;
		_originalScale = transform.localScale;
		GazeManager.lookingAtStarted += HandleLookingAtStartedEvent;
	}

	private void HandleLookingAtStartedEvent (Transform transform)
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
			if (!_selected) {
				if (!_connected) {
					_light.color = _defaultColor;
				} else {
					_light.color = _connectedColor;
				} 
			}     
		}
	}

	private IEnumerator Pop()
	{
		transform.localScale = new Vector3(_originalScale.x * 2f, _originalScale.y * 2f, _originalScale.z * 2f);
		yield return StartCoroutine(transform.ScaleTo(_originalScale, 2f, EaseType.BackOut));
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
}