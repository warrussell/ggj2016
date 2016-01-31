using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HologramManager : MonoBehaviour 
{
	public enum direction { up,down,left,right }

	public float connectionUnitOfMeasure;
    [Header("Spawning Collections")]
	public int numberOfCollections;
	public GameObject HorizontalRotationObject;
	public List<Vector3> PresetLocations;
	[Header("Rotating")]
	public float RotationSpeed = 1f;

	private Vector3 XRotation = Vector3.zero;
	private Vector3 YRotation = Vector3.zero;

	// Use this for initialization
	void Start () {
		XRotation = transform.localEulerAngles;
		YRotation = HorizontalRotationObject.transform.localEulerAngles;
		CreateHologram(1);
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
		StartCoroutine(CreateHologramCoroutine());
	}

	private IEnumerator CreateHologramCoroutine()
	{
		List<Vector3> AvailableLocations = new List<Vector3>();
		for (int i = 0; i < PresetLocations.Count; ++i) {
			AvailableLocations.Add(PresetLocations[i]);
		}

		Vector3 originalScale = transform.localScale;
		transform.localScale = Vector3.zero;
		Vector3 originalPosition = transform.position;
		transform.position = new Vector3(originalPosition.x, originalPosition.y-1, originalPosition.z);

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

		StartCoroutine(transform.MoveTo(originalPosition, 2f, EaseType.BackOut));
		yield return StartCoroutine(transform.ScaleTo(originalScale, 2f, EaseType.BackOut));
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
	}
}