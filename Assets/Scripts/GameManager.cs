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

	private List<StarController> _selectedStars;
	private List<StarController> _starConnectionObjects;
	private Dictionary<StarController, List<Connection>> _starConnections;
	private Dictionary<StarController, List<Connection>> _reverseStarConnections;
	private StarController _selectedStar;

	private void Awake () 
	{
		_selectedStars = new List<StarController>();
		_starConnectionObjects = new List<StarController>();
		_starConnections = new Dictionary<StarController, List<Connection>>();
		_reverseStarConnections = new Dictionary<StarController, List<Connection>>();
	}

	private void Start () 
	{
		if (gameManagerReadyEvent != null) gameManagerReadyEvent();
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
		if (_reverseStarConnections.ContainsKey(newStar)) {
			_reverseStarConnections[newStar].Add(new Connection(oldStar, connection));
		} else {
			List<Connection> connections = new List<Connection>();
			connections.Add(new Connection(oldStar, connection));
			_reverseStarConnections.Add(newStar, connections);
		}
	}

	public void DestroyStarConnection(StarController oldStar, StarController newStar)
	{
		RemoveConnection(_starConnections, oldStar, newStar);
		/*if (_starConnections.ContainsKey(oldStar)) {
			Connection connection = _starConnections[oldStar].Find(x => x.connectedStar == newStar);
			Destroy(connection.connectionObject);
			if (_starConnections[oldStar].Count == 1) {
				_starConnections[oldStar].Clear();
				_starConnections.Remove(oldStar);
			} else {
				_starConnections[oldStar].Remove(connection);
			}
		}*/
		RemoveConnection(_starConnections, newStar, oldStar);
		/*if (_starConnections.ContainsKey(newStar)) {
			Connection connection = _starConnections[newStar].Find(x => x.connectedStar == oldStar);
			Destroy(connection.connectionObject);
			if (_starConnections[newStar].Count == 1) {
				_starConnections[newStar].Clear();
				_starConnections.Remove(newStar);
			} else {
				_starConnections[newStar].Remove(connection);
			}
		}*/
		RemoveConnection(_reverseStarConnections, newStar, oldStar);
		/*if (_reverseStarConnections.ContainsKey(newStar)) {
			Connection connection = _reverseStarConnections[newStar].Find(x => x.connectedStar == oldStar);
			Destroy(connection.connectionObject);
			if (_reverseStarConnections[newStar].Count == 1) {
				_reverseStarConnections[newStar].Clear();
				_reverseStarConnections.Remove(newStar);
			} else {
				_reverseStarConnections[newStar].Remove(connection);
			}
		}*/
		RemoveConnection(_reverseStarConnections, oldStar, newStar);
		/*if (_reverseStarConnections.ContainsKey(oldStar)) {
			Connection connection = _reverseStarConnections[oldStar].Find(x => x.connectedStar == newStar);
			Destroy(connection.connectionObject);
			if (_reverseStarConnections[oldStar].Count == 1) {
				_reverseStarConnections[oldStar].Clear();
				_reverseStarConnections.Remove(oldStar);
			} else {
				_reverseStarConnections[oldStar].Remove(connection);
			}
		}*/

		Debug.Log("Connection Destroyed between " + newStar.name + " and " + oldStar.name);
	}

	private void RemoveConnection(Dictionary<StarController, List<Connection>> memory, StarController key, StarController value)
	{
		if (memory.ContainsKey(key)) {
			Connection connection = memory[key].Find(x => x.connectedStar == value);
			Destroy(connection.connectionObject);
			if (memory[key].Count == 1) {
				memory[key].Clear();
				memory.Remove(key);
			} else {
				memory[key].Remove(connection);
			}
		}
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