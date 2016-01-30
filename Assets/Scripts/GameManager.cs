using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{	
	/// <summary>
	/// Gets the instance of GameManager.
	/// </summary>
	public static GameManager Instance
	{
		get
		{
			if(_instance == null) {
				_instance = GameObject.FindObjectOfType<GameManager>();
			}
			
			return _instance;
		}
	}
 	private static GameManager _instance;

	public delegate void GameManagerReadyEvent();
	public static GameManagerReadyEvent gameManagerReadyEvent;

	private Coroutine _popCoroutine;

	void Awake () 
	{
		
	}

	void Start () 
	{
		if (gameManagerReadyEvent != null) gameManagerReadyEvent();
		_popCoroutine = StartCoroutine(Pop(new Vector3(3,3,3)));
		BoxCollider collider = GetComponent<BoxCollider>();
		collider.isTrigger = true;
	}

	private IEnumerator Pop(Vector3 scaleTo)
	{
		Vector3 test = scaleTo;
		test.x = 5;
		yield return new WaitForSeconds(1f);
		StartCoroutine(transform.ScaleTo(test, 1f, EaseType.BackOut));
	}

	void Update () 
	{

	}
}