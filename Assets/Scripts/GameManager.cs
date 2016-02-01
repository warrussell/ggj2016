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

 	public HologramManager hologramManager;

	public delegate void GameManagerReadyEvent();
	public static GameManagerReadyEvent gameManagerReadyEvent;

	[SerializeField]
	protected GameObject _level1ActiveMesh;
	[SerializeField]
	protected GameObject _level2ActiveMesh;
	[SerializeField]
	protected GameObject _level3ActiveMesh;
	[SerializeField]
	protected GameObject _level1InactiveMesh;
	[SerializeField]
	protected GameObject _level2InactiveMesh;
	[SerializeField]
	protected GameObject _level3InactiveMesh;

	private List<StarController> _selectedStars;
	private List<StarController> _starConnectionObjects;
	private Dictionary<StarController, List<Connection>> _starConnections;
	private StarController _selectedStar;
	private int lastCompletedLevel = 0;

	private void Awake () 
	{
		_selectedStars = new List<StarController>();
		_starConnectionObjects = new List<StarController>();
		_starConnections = new Dictionary<StarController, List<Connection>>();
	}

	private void Start () 
	{
		InputListener.inputUpEvent += HandleInputUpEvent;
		if (gameManagerReadyEvent != null) gameManagerReadyEvent();

		_level1InactiveMesh.SetActive(false);
		_level2InactiveMesh.SetActive(true);
		_level3InactiveMesh.SetActive(true);
		_level1ActiveMesh.SetActive(true);
		_level2ActiveMesh.SetActive(false);
		_level3ActiveMesh.SetActive(false);

		hologramManager.CreateHologram(1);
	}

	void HandleInputUpEvent (KeyCode key)
	{
		if (key == KeyCode.Backspace)
		{
			if (hologramManager.state == HologramManager.State.open) {
				StartCoroutine(TransitionToLevel(hologramManager.currentLevel));
			}
		}
	}

	public void HandleSelected(Transform trans)
	{
		if (trans.CompareTag("Star")) {
			// star selected
			Debug.Log("Selected Star " + trans.name);
			SelectStar(trans.GetComponent<StarController>());
		}
	}

	public void SelectStar(StarController star)
	{
		if (_selectedStar == null) {
			Debug.Log("no previous star selected");
			_selectedStar = star;
			star.Select();
		} else {
			Debug.Log("star already selected");
			if (_selectedStar == star) {
				Debug.Log("selected same star");
				Debug.Log("Unselected star " + star.name);
				star.Unselect();
			} else if (_selectedStars.Contains(star)) {
				Debug.Log("already in connection");
				if (ConnectionExists(_selectedStar, star)) {
					DestroyStarConnection(_selectedStar, star);
					_selectedStar.Unconnect();
					star.Unconnect();
				} else {
					CreateStarConnection(_selectedStar, star);
					_selectedStar.Connect();
					star.Connect();
				}
			} else {
				Debug.Log("not already in connection");
				CreateStarConnection(_selectedStar, star);
				_selectedStars.Add(star);
				if (!_selectedStars.Contains(_selectedStar)) {
					_selectedStars.Add(_selectedStar);
				}
				_selectedStar.Connect();
				star.Connect();
			}
			_selectedStar = null;
		}
		if (CheckAnswer(hologramManager.currentLevel)) {
			lastCompletedLevel = hologramManager.currentLevel;
			StartCoroutine(TransitionToLevel(lastCompletedLevel + 1));
		}
	}

	private IEnumerator TransitionToLevel(int level)
	{
		while (hologramManager.state != HologramManager.State.open) {
			Debug.Log("Waiting for hologram to be open before we can close");
			yield return null;
		}

		_level1InactiveMesh.SetActive(true);
		_level2InactiveMesh.SetActive(true);
		_level3InactiveMesh.SetActive(true);
		_level1ActiveMesh.SetActive(false);
		_level2ActiveMesh.SetActive(false);
		_level3ActiveMesh.SetActive(false);

		hologramManager.DestroyHologram();
		_selectedStar = null;
		_selectedStars.Clear();
		_starConnectionObjects.Clear();
		foreach (StarController star in _starConnections.Keys) {
			_starConnections[star].Clear();
		}
		_starConnections.Clear();

		while (hologramManager.state != HologramManager.State.closed) {
			Debug.Log("Waiting for hologram to close");
			yield return null;
		}

		if (level == 1) {
			_level1InactiveMesh.SetActive(false);
			_level1ActiveMesh.SetActive(true);
		} else if (level == 2) {
			_level2InactiveMesh.SetActive(false);
			_level2ActiveMesh.SetActive(true);
		} else if (level == 3) {
			_level3InactiveMesh.SetActive(false);
			_level3ActiveMesh.SetActive(true);
		}

		hologramManager.CreateHologram(level);
	}

	private bool ConnectionExists(StarController oldStar, StarController newStar)
	{
		bool result = false;

		if (_starConnections.ContainsKey(oldStar)) {
			Debug.Log("Connection between " + oldStar.name + " exists for some connection");
			if (_starConnections[oldStar].Find(x => x.connectedStar.GetInstanceID() == newStar.GetInstanceID()) != null) {
				result = true;
			}
		}

		if (!result) {
			if (_starConnections.ContainsKey(newStar)) {
				if (_starConnections[newStar].Find(x => x.connectedStar.GetInstanceID() == oldStar.GetInstanceID()) != null) {
					result = true;
				}
			}
		}

		return result;
	}

	private int NumOfConnections()
	{
		int result = 0;

		foreach (StarController star in _starConnections.Keys) {
			result += _starConnections[star].Count;
		}

		return result / 2;
	}

	public void CreateStarConnection(StarController oldStar, StarController newStar)
	{
		GameObject connection = hologramManager.CreateConnection(oldStar.transform, newStar.transform);
		Debug.Log("Connection Created between " + newStar.name + " and " + oldStar.name);

		if (_starConnections.ContainsKey(oldStar)) {
			_starConnections[oldStar].Add(new Connection(newStar, connection));
		} else {
			List<Connection> connections = new List<Connection>();
			connections.Add(new Connection(newStar, connection));
			_starConnections.Add(oldStar, connections);
		}
		if (_starConnections.ContainsKey(newStar)) {
			_starConnections[newStar].Add(new Connection(oldStar, connection));
		} else {
			List<Connection> connections = new List<Connection>();
			connections.Add(new Connection(oldStar, connection));
			_starConnections.Add(newStar, connections);
		}
	}

	public void DestroyStarConnection(StarController oldStar, StarController newStar)
	{
		RemoveConnection(_starConnections, oldStar, newStar);
		RemoveConnection(_starConnections, newStar, oldStar);
		Debug.Log("Connection Destroyed between " + newStar.name + " and " + oldStar.name);
	}

	private void RemoveConnection(Dictionary<StarController, List<Connection>> memory, StarController key, StarController value)
	{
		if (memory.ContainsKey(key)) {
			Connection connection = memory[key].Find(x => x.connectedStar == value);
			if (connection != null) {
				Destroy(connection.connectionObject);
				memory[key].Remove(connection);
				Debug.Log("Removed connection to " + value.name);
			}
			if (memory[key].Count == 0) {
				memory.Remove(key);
				_selectedStars.Remove(key);
				Debug.Log("Removed Selected Star " + key.name);
			}
		}
	}

	public bool CheckAnswer(int level)
	{
		bool result = false;
		string baseName = "answerstar";
		switch (level) 
		{
			case 1:
			{
				if (_selectedStars.Count != 6) {
					break;
				} 

				Debug.Log("Number of connections: " + NumOfConnections());

				if (NumOfConnections() != 5) {
					break;
				}

				StarController answerstar1 = _selectedStars.Find(x => x.name == baseName + 1);
				StarController answerstar2 = _selectedStars.Find(x => x.name == baseName + 2);
				StarController answerstar3 = _selectedStars.Find(x => x.name == baseName + 3);
				StarController answerstar4 = _selectedStars.Find(x => x.name == baseName + 4);
				StarController answerstar5 = _selectedStars.Find(x => x.name == baseName + 5);
				StarController answerstar6 = _selectedStars.Find(x => x.name == baseName + 6);

				if (answerstar1 == null ||
					answerstar2 == null ||
					answerstar3 == null ||
					answerstar4 == null ||
					answerstar5 == null ||
					answerstar6 == null) 
				{
					break;
				}

				if (!ConnectionExists(answerstar1, answerstar2) ||
					!ConnectionExists(answerstar2, answerstar5) ||
					!ConnectionExists(answerstar5, answerstar4) ||
					!ConnectionExists(answerstar4, answerstar3) ||
					!ConnectionExists(answerstar3, answerstar6)) 
				{
					break;
				}

				result = true;
				break;
			}
			case 2:
			{
				if (_selectedStars.Count != 6) {
					break;
				}

				Debug.Log("Number of connections: " + NumOfConnections());

				if (NumOfConnections() != 7) {
					break;
				}

				StarController answerstar1 = _selectedStars.Find(x => x.name == baseName + 1);
				StarController answerstar2 = _selectedStars.Find(x => x.name == baseName + 2);
				StarController answerstar3 = _selectedStars.Find(x => x.name == baseName + 3);
				StarController answerstar4 = _selectedStars.Find(x => x.name == baseName + 4);
				StarController answerstar5 = _selectedStars.Find(x => x.name == baseName + 5);
				StarController answerstar6 = _selectedStars.Find(x => x.name == baseName + 6);

				if (answerstar1 == null ||
					answerstar2 == null ||
					answerstar3 == null ||
					answerstar4 == null ||
					answerstar5 == null ||
					answerstar6 == null) 
				{
					break;
				}

				if (!ConnectionExists(answerstar1, answerstar2) ||
					!ConnectionExists(answerstar2, answerstar3) ||
					!ConnectionExists(answerstar3, answerstar1) ||
					!ConnectionExists(answerstar4, answerstar3) ||
					!ConnectionExists(answerstar4, answerstar6) ||
					!ConnectionExists(answerstar6, answerstar5) ||
					!ConnectionExists(answerstar3, answerstar5)) 
				{
					break;
				}

				result = true;

				break;
			}
			case 3:
			{
				if (_selectedStars.Count != 18) {
					break;
				}

				Debug.Log("Number of connections: " + NumOfConnections());

				if (NumOfConnections() != 19) {
					break;
				}

				StarController answerstar1 = _selectedStars.Find(x => x.name == baseName + 1);
				StarController answerstar2 = _selectedStars.Find(x => x.name == baseName + 2);
				StarController answerstar3 = _selectedStars.Find(x => x.name == baseName + 3);
				StarController answerstar4 = _selectedStars.Find(x => x.name == baseName + 4);
				StarController answerstar5 = _selectedStars.Find(x => x.name == baseName + 5);
				StarController answerstar6 = _selectedStars.Find(x => x.name == baseName + 6);
				StarController answerstar7 = _selectedStars.Find(x => x.name == baseName + 7);
				StarController answerstar8 = _selectedStars.Find(x => x.name == baseName + 8);
				StarController answerstar9 = _selectedStars.Find(x => x.name == baseName + 9);
				StarController answerstar10 = _selectedStars.Find(x => x.name == baseName + 10);
				StarController answerstar11 = _selectedStars.Find(x => x.name == baseName + 11);
				StarController answerstar12 = _selectedStars.Find(x => x.name == baseName + 12);
				StarController answerstar13 = _selectedStars.Find(x => x.name == baseName + 13);
				StarController answerstar14 = _selectedStars.Find(x => x.name == baseName + 14);
				StarController answerstar15 = _selectedStars.Find(x => x.name == baseName + 15);
				StarController answerstar16 = _selectedStars.Find(x => x.name == baseName + 16);
				StarController answerstar17 = _selectedStars.Find(x => x.name == baseName + 17);
				StarController answerstar18 = _selectedStars.Find(x => x.name == baseName + 18);

				if (answerstar1 == null ||
					answerstar2 == null ||
					answerstar3 == null ||
					answerstar4 == null ||
					answerstar5 == null ||
					answerstar6 == null ||
					answerstar7 == null ||
					answerstar8 == null ||
					answerstar9 == null ||
					answerstar10 == null ||
					answerstar11 == null ||
					answerstar12 == null ||
					answerstar13 == null ||
					answerstar14 == null ||
					answerstar15 == null ||
					answerstar16 == null ||
					answerstar17 == null ||
					answerstar18 == null) 
				{
					break;
				}

				if (!ConnectionExists(answerstar1, answerstar5) ||
					!ConnectionExists(answerstar2, answerstar5) ||
					!ConnectionExists(answerstar5, answerstar6) ||
					!ConnectionExists(answerstar6, answerstar7) ||
					!ConnectionExists(answerstar7, answerstar3) ||
					!ConnectionExists(answerstar3, answerstar4) ||
					!ConnectionExists(answerstar4, answerstar8) ||
					!ConnectionExists(answerstar8, answerstar9) ||
					!ConnectionExists(answerstar9, answerstar6) ||
					!ConnectionExists(answerstar8, answerstar15) ||
					!ConnectionExists(answerstar9, answerstar14) ||
					!ConnectionExists(answerstar14, answerstar15) ||
					!ConnectionExists(answerstar13, answerstar14) ||
					!ConnectionExists(answerstar13, answerstar12) ||
					!ConnectionExists(answerstar11, answerstar12) ||
					!ConnectionExists(answerstar10, answerstar12) ||
					!ConnectionExists(answerstar15, answerstar16) ||
					!ConnectionExists(answerstar16, answerstar17) ||
					!ConnectionExists(answerstar17, answerstar18)) 
				{
					break;
				}

				result = true;

				break;
			}
		}

		return result;
	}
}

public class Connection
{
	public StarController connectedStar;
	public GameObject connectionObject;

	public Connection(StarController star, GameObject connection)
	{
		this.connectedStar = star;
		this.connectionObject = connection;
	}
}