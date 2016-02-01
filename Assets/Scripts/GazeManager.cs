using UnityEngine;
using System.Collections;

public class GazeManager : MonoBehaviour 
{
	[SerializeField] 
	protected float _gazeThickness = 1f;

	public delegate void LookingAtStarted(Transform transform);
	public static event LookingAtStarted lookingAtStarted;
	public delegate void LookingAtStopped(Transform transform);
	public static event LookingAtStopped lookingAtStopped;
	public delegate void LookingAtHeld(Transform transform);
	public static event LookingAtHeld lookingAtHeld;

	private Transform _targetObject;

	private void Start()
	{
		InputListener.inputUpEvent += HandleInputUpEvent;
	}

	private void HandleInputUpEvent (KeyCode key)
	{
		if (key == KeyCode.Space) {
			if (_targetObject != null) {
				GameManager.Instance.HandleSelected(_targetObject);
			}
		}
	}

	private void Update()
	{
		UpdateGaze();
	}

	private void UpdateGaze()
	{
		//cast sphere to see what we are looking at
		RaycastHit hit;
		Vector3 origin = Camera.main.transform.position;
		Vector3 direction = Camera.main.transform.TransformDirection(Vector3.forward);
		if (Physics.SphereCast(origin, _gazeThickness, direction, out hit)) {
		    if (hit.transform != null) {
				if (_targetObject == null || (_targetObject != null && _targetObject.GetInstanceID() != hit.transform.GetInstanceID())) {
		    		if (_targetObject != null) {
		    			//stopped looking at old object
						if (lookingAtStopped != null) lookingAtStopped(_targetObject);
		    		}
					// looking at new object
					_targetObject = hit.transform;
					if (lookingAtStarted != null) {
						lookingAtStarted(_targetObject);
						Debug.DrawRay(origin, direction * 2);
					}
					_targetObject = hit.transform;
				} else {
					if (lookingAtHeld != null) lookingAtHeld(_targetObject);
					Debug.DrawRay(origin, direction * 2);
		    	}
		    }
		} else {
			if (_targetObject != null) {
    			//stopped looking at old object
    			if (lookingAtStopped != null) lookingAtStopped(_targetObject);
    		}
			_targetObject = null;
		}
	}
}