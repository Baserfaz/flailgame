using UnityEngine;
using System.Collections;

public class enemy : MonoBehaviour {

	//references
	private GameObject gamemaster;

	//enemy settings
	public int baseValue = 1000;
	
	void Awake() {
		gamemaster = GameObject.FindGameObjectWithTag ("GAMEMASTER");
	}

	void Update() {
		if (transform.position.x < -gamemaster.GetComponent<gamemaster> ().screenWidth - 2f || transform.position.x > gamemaster.GetComponent<gamemaster> ().screenWidth + 2f) {
			Destroy (gameObject);
		} else if (transform.position.y < -gamemaster.GetComponent<gamemaster> ().screenHeight - 2f || transform.position.y > gamemaster.GetComponent<gamemaster> ().screenHeight + 2f) {
			Destroy (gameObject);
		}
	}
}
