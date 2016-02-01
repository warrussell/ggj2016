using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HologramManager : MonoBehaviour 
{
	public enum direction { up,down,left,right }
	public enum State {closed, open, closing, opening}
	public State state = State.closed; 

	public float connectionUnitOfMeasure;
    [Header("Spawning Collections")]
	public int numberOfCollections;
	public GameObject HorizontalRotationObject;
	public List<Vector3> PresetLocations;
	[Header("Rotating")]
	public float RotationSpeed = 4f;
	[Header("Audio Clips")]
	public AudioSource audioSource;
	public AudioClip sfxPowerOn;
	public AudioClip sfxPowerDown;
	public AudioClip sfxHum;
	[Header("Lights")]
	public Light hologramHalo;
	public Vector2 intensityRange;

	private Vector3 XRotation = Vector3.zero;
	private Vector3 YRotation = Vector3.zero;
	private Coroutine _pulseCoroutine;
	private Vector3 _originalScale;
	private Vector3 _originalPosition;
	public int currentLevel { get; private set; }

	// Use this for initialization
	void Start () {
		XRotation = transform.localEulerAngles;
		YRotation = HorizontalRotationObject.transform.localEulerAngles;
		_originalScale = transform.localScale;
		_originalPosition = transform.position;
	}

    private void handleInputHeld(KeyCode key)
    {
		//player input changes MyRotation
		if (key == KeyCode.W)
			XRotation.x += RotationSpeed;
		if (key == KeyCode.S)
			XRotation.x -= RotationSpeed;
		if (key == KeyCode.A)
			YRotation.y += RotationSpeed;
		if (key == KeyCode.D)
			YRotation.y -= RotationSpeed;

		//make sure MyRotation does not get too crazy huge
		if (XRotation.x > 360)
			XRotation.x -= 360f;
		else if (XRotation.x < 0)
			XRotation.x += 360f;
		if (YRotation.y > 360)
			YRotation.y -= 360f;
		else if (YRotation.y < 0)
			YRotation.y += 360f;

		//apply MyRotation to the object's transform
		transform.localEulerAngles = XRotation;
		HorizontalRotationObject.transform.localEulerAngles = YRotation;
    }

	public void CreateHologram(int level)
	{
		InputListener.inputHeldEvent += handleInputHeld;
		Debug.Log("Opening!");
		currentLevel = level;
		StartCoroutine(CreateHologramCoroutine(currentLevel));
	}

	private IEnumerator CreateHologramCoroutine(int level)
	{
		state = State.opening;

		audioSource.loop = false;
		audioSource.clip = sfxPowerOn;
		audioSource.Play();

		List<Vector3> AvailableLocations = new List<Vector3>();
		for (int i = 0; i < PresetLocations.Count; ++i) {
			AvailableLocations.Add(PresetLocations[i]);
		}

		transform.localScale = Vector3.zero;
		transform.position = new Vector3(_originalPosition.x, _originalPosition.y-1, _originalPosition.z);

		for (int i = 1; i <= numberOfCollections; i++) 
		{
			GameObject starCollection = Instantiate(Resources.Load("starCollection "+Random.Range(1,6))) as GameObject;
			Vector3 starCollectionOriginalScale = starCollection.transform.localScale;
			starCollection.transform.SetParent(HorizontalRotationObject.transform);
			starCollection.transform.localScale = starCollectionOriginalScale;

			int chosen = Random.Range (0, AvailableLocations.Count);
			starCollection.transform.localPosition = AvailableLocations [chosen];
			AvailableLocations.RemoveAt (chosen);
		}

		GameObject answer = Instantiate(Resources.Load("Answer" + level)) as GameObject;
		Vector3 answerOriginalScale = answer.transform.localScale;
		answer.transform.SetParent(HorizontalRotationObject.transform);
		answer.transform.localScale = answerOriginalScale;
		answer.transform.position = Vector3.zero;

		StartCoroutine(transform.MoveTo(_originalPosition, 2f, EaseType.BackOut));
		yield return StartCoroutine(transform.ScaleTo(_originalScale, 2f, EaseType.BackOut));

		audioSource.loop = true;
		audioSource.clip = sfxHum;
		audioSource.Play();

		_pulseCoroutine = StartCoroutine(PulseTo(hologramHalo, 3f, intensityRange.x, intensityRange.y));

		state = State.open;
	}

	public GameObject CreateConnection(Transform firstStar, Transform secondStar)
	{
		GameObject connection = Instantiate(Resources.Load("Connection")) as GameObject;
		Vector3 connectionOriginalScale = connection.transform.localScale;
		connection.transform.SetParent(HorizontalRotationObject.transform);
		connection.transform.localScale = connectionOriginalScale;
		connection.transform.position = firstStar.position;
		connection.transform.LookAt(secondStar.position);
		float distance = Vector3.Distance(firstStar.position, secondStar.position);
		connection.transform.localScale = new Vector3(connectionOriginalScale.x, connectionOriginalScale.y, connectionUnitOfMeasure * distance);
		return connection; 
	}

	public void DestroyHologram()
	{
		InputListener.inputHeldEvent -= handleInputHeld;
		Debug.Log("Closing!");
		StartCoroutine(DestroyHologramCoroutine());
	}

	private IEnumerator DestroyHologramCoroutine()
	{
		state = State.closing;

		audioSource.loop = false;
		audioSource.clip = sfxPowerDown;
		audioSource.Play();

		if (_pulseCoroutine != null) {
			StopCoroutine(_pulseCoroutine);
			_pulseCoroutine = null;
		}

		foreach (Transform child in HorizontalRotationObject.transform) 
		{
			foreach(Transform collectionChild in child) {
				StarController star = collectionChild.GetComponent<StarController>();
				if (star != null) {
					star.DestroyThis();
				} else {
					Destroy(collectionChild.gameObject);
				}
			}
			Destroy(child.gameObject);
		}

		Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y-1, transform.position.z);
		StartCoroutine(transform.MoveTo(targetPosition, 1f, EaseType.BackOut));
		yield return StartCoroutine(transform.ScaleTo(Vector3.zero, 1f, EaseType.BackOut));

		state = State.closed;
	}

	private IEnumerator PulseTo(Light light, float duration, float startIntensity, float targetIntensity)
	{
		float elapsed = 0;
		float start = startIntensity;
		float range = targetIntensity - start;
		while (elapsed < duration)
		{
			elapsed = Mathf.MoveTowards(elapsed, duration, Time.deltaTime);
			light.intensity = start + range * (elapsed / duration);
			yield return 0;
		}
		light.intensity = targetIntensity;

		_pulseCoroutine = StartCoroutine(PulseTo(light, duration, targetIntensity, startIntensity));
	}
}