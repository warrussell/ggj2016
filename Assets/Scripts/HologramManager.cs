using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HologramManager : MonoBehaviour {
    public enum direction { up,down,left,right }
	public List<Vector3> PresetLocations;
	public int numberOfCollections;

	public GameObject HorizontalRotationObject;

	private Vector3 XRotation = Vector3.zero;
	private Vector3 YRotation = Vector3.zero;

	public float RotationSpeed = 1f;

	// Use this for initialization
	void Start () {
		XRotation = HorizontalRotationObject.transform.localEulerAngles;
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

    private void HologramRotate(direction dir)
    {

    }

	public void CreateHologram(int level)
	{
		InputListener.inputHeldEvent += handleInputHeld;

		List<Vector3> AvailableLocations = PresetLocations;
		for (int i = 1; i <= numberOfCollections; i++) 
		{
			GameObject starCollection = Instantiate(Resources.Load("starCollection "+Random.Range(1,6))) as GameObject;
			starCollection.transform.SetParent(HorizontalRotationObject.transform);

			int chosen = Random.Range (0, AvailableLocations.Count);
			starCollection.transform.localPosition = AvailableLocations [chosen];
			AvailableLocations.RemoveAt (chosen);
		}
	}
	public void DestroyHologram()
	{
		InputListener.inputHeldEvent -= handleInputHeld;
		GameObject star = Instantiate(Resources.Load("star")) as GameObject;
		star.transform.SetParent(this.transform);
		star.transform.position = Vector3.zero;
	}
    // Update is called once per frame
    void Update () {
	
	}
}
