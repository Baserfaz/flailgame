using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class gamemaster : MonoBehaviour {

	//references
	public GameObject spawnerPrefab;
	public GameObject[] enemyPrefabs;
	public GameObject[] nuggetPrefabs;
	public GameObject bombPrefab;
	public GameObject helpScreenGo;
	public GameObject deadScreenGo;
	public GameObject flailGo;
	private GameObject pointsTextGo;
	
	//enemy info
	public float enemyMass = 25f;

	//timers
	private float Timer = 0f;
	private float colorTimer = 0f;

	//background color variables
	private Color bg = Color.white;

	//other stuffs
	[HideInInspector]
	public float screenHeight;
	[HideInInspector]
	public float screenWidth;
	private int currentScore = 0;
	
	//gamelogic
	private bool paused = true;
	public bool gameOver = false;

	//list
	private List <GameObject> spawners = new List <GameObject> ();
	[HideInInspector]
	public List <GameObject> spawnedEnemies = new List <GameObject> (); 
	[HideInInspector]
	public List <GameObject> spawnedNuggets = new List<GameObject> ();

	void Awake() {
		pointsTextGo = GameObject.Find ("points");
	}

	void Start () {
		initiateEnemySpawnPoints ();
		screenHeight = Camera.main.ScreenToWorldPoint (new Vector3(Screen.width, Screen.height, 0f)).y;
		screenWidth = Camera.main.ScreenToWorldPoint (new Vector3(Screen.width, Screen.height, 0f)).x;
		deadScreenGo.SetActive(false);
	}

	void Update() {
		if (!paused) {
			if(!gameOver) {
				flailGo.GetComponent<Rigidbody2D>().isKinematic = false;
				flailGo.GetComponent<LineRenderer>().enabled = true;
				if (Timer - Time.time <= 0f) {
					spawnEnemy ();
					Timer = Time.time + Random.Range (0, 2f); 
				}
				changeBackgroundColor ();
			} else {
				Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, Color.white, Time.deltaTime);
				DestroyEverything();
				deadScreenGo.SetActive(true);
			}
		} else {
			helpScreenGo.SetActive(true);
			flailGo.GetComponent<Rigidbody2D>().isKinematic = true;
			flailGo.GetComponent<LineRenderer>().enabled = false;
		}
	}

	private void DestroyEverything() {
		flailGo.GetComponent<Rigidbody2D>().isKinematic = true;
		flailGo.GetComponent<LineRenderer>().enabled = false;
		foreach(GameObject enemy in spawnedEnemies) {
			Destroy(enemy);
		}
		foreach(GameObject nugget in spawnedNuggets) {
			Destroy(nugget);
		}
		spawnedEnemies.Clear();
		spawnedNuggets.Clear();
	}

	private void initiateEnemySpawnPoints() {
		Vector3 p = Camera.main.ScreenToWorldPoint (new Vector3 (Screen.width, Screen.height, 0f));
		Vector3 spawnPointLocation;

		spawnPointLocation = new Vector3 (p.x + 1f, p.y + 1f, 0f);
		spawners.Add ((GameObject)Instantiate(spawnerPrefab, spawnPointLocation, Quaternion.identity));

		spawnPointLocation = new Vector3 (-p.x - 1f, -p.y - 1f, 0f);
		spawners.Add ((GameObject)Instantiate(spawnerPrefab, spawnPointLocation, Quaternion.identity));

		spawnPointLocation = new Vector3 (-p.x - 1f, p.y + 1f, 0f);
		spawners.Add ((GameObject)Instantiate(spawnerPrefab, spawnPointLocation, Quaternion.identity));

		spawnPointLocation = new Vector3 (p.x + 1f, -p.y - 1f, 0f);
		spawners.Add ((GameObject)Instantiate(spawnerPrefab, spawnPointLocation, Quaternion.identity));

		spawnPointLocation = new Vector3 (0f, -p.y - 1f, 0f);
		spawners.Add ((GameObject)Instantiate(spawnerPrefab, spawnPointLocation, Quaternion.identity));

		spawnPointLocation = new Vector3 (0f, p.y + 1f, 0f);
		spawners.Add ((GameObject)Instantiate(spawnerPrefab, spawnPointLocation, Quaternion.identity));

		spawnPointLocation = new Vector3 (p.x + 1f, 0f, 0f);
		spawners.Add ((GameObject)Instantiate(spawnerPrefab, spawnPointLocation, Quaternion.identity));

		spawnPointLocation = new Vector3 (-p.x - 1f, 0f, 0f);
		spawners.Add ((GameObject)Instantiate(spawnerPrefab, spawnPointLocation, Quaternion.identity));

	}

	private void changeBackgroundColor() {
		if (colorTimer - Time.time <= 0f) {
			bg = new Color (Random.Range (0f, 1f), Random.Range (0f, 1f), Random.Range (0f, 1f), 0.1f);
			colorTimer = Time.time + Random.Range(5f, 10f);
		}
		Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, bg, Time.deltaTime);
	}

	private void spawnEnemy() {
		GameObject enemy = (GameObject)Instantiate (enemyPrefabs[Random.Range(0, enemyPrefabs.Length)], spawners [Random.Range (0, spawners.Count)].transform.position, Quaternion.identity);

		//modify mass etc.
		enemy.GetComponent<Rigidbody2D> ().mass = enemyMass;

		//push and give some torque.
		enemy.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (0f - enemy.transform.position.x, 0f - enemy.transform.position.y).normalized * Random.Range (100f, 350f), ForceMode2D.Impulse);
		enemy.GetComponent<Rigidbody2D> ().AddTorque (Random.Range(0f, 5f), ForceMode2D.Impulse);

		//add to list of enemies.
		spawnedEnemies.Add (enemy);
	}

	public void Reset() {
		Application.LoadLevel (Application.loadedLevel);
	}

	public void Play() {
		paused = false;
		helpScreenGo.SetActive (false);
	}

	public void spawnNuggets(Collision2D coll) {
		for (int i = 0; i < Random.Range(1, 5); i++) {
			GameObject nugget = (GameObject)Instantiate (nuggetPrefabs [Random.Range (0, nuggetPrefabs.Length)], coll.transform.position, Quaternion.identity);
			nugget.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-5f, 5f), Random.Range(-5f, 5f)), ForceMode2D.Impulse);
			nugget.GetComponent<Rigidbody2D>().AddTorque(Random.Range(0f, 1f), ForceMode2D.Impulse);
			spawnedNuggets.Add(nugget);
		}
	}

	public void addScore(int value) {
		int totalScore = currentScore + value;
		currentScore = totalScore;
		pointsTextGo.GetComponent<Text> ().text = totalScore.ToString ("n0");
	}

}
