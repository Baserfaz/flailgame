using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class flail : MonoBehaviour {

	//references
	private SpringJoint2D myJoint;
	private LineRenderer myLine;
	private GameObject gamemaster;
	private GameObject canvasGo;
	public GameObject textGo;

	//location
	private Vector3 mousePos;

	void Awake() {
		myJoint = GetComponent<SpringJoint2D> (); 
		myLine = GetComponent<LineRenderer> ();
		gamemaster = GameObject.FindGameObjectWithTag ("GAMEMASTER");
		canvasGo = GameObject.Find ("GUI");
	}

	void Update () {
		mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);

		StartCoroutine("DragObject");

		myLine.SetPosition(0, gameObject.transform.position);
		myLine.SetPosition(1, new Vector3 (mousePos.x, mousePos.y, 0f));
	}
	
	IEnumerator DragObject () {  
		myJoint.connectedAnchor = new Vector2 (mousePos.x, mousePos.y);
		yield return null;
	}

	private void DestroyAndSpawnNuggets(Collision2D coll) {
		gamemaster.GetComponent<gamemaster>().spawnedEnemies.Remove(coll.gameObject);
		gamemaster.GetComponent<gamemaster>().spawnNuggets(coll);
		Destroy(coll.gameObject);
	}

	private void InstantiateFloatingText(Collision2D coll, int baseValue) {
		GameObject goInst = (GameObject)Instantiate (textGo, Camera.main.WorldToScreenPoint(coll.transform.position), Quaternion.identity);
		goInst.transform.SetParent (canvasGo.transform);
		int curValue = 0;
		int.TryParse (goInst.GetComponentInChildren<Text>().text, out curValue);
		goInst.GetComponentInChildren<Text>().text = "+" + (baseValue + curValue).ToString() + ".0";
		Destroy (goInst, 1f);
	}

	void OnCollisionEnter2D(Collision2D coll) {
		float hitVelocity = GetComponent<Rigidbody2D>().velocity.magnitude;

		if (coll.transform.tag == "BOMB") {
			gamemaster.GetComponent<gamemaster>().gameOver = true;
		}

		if (coll.transform.tag == "ENEMY") {
			if(hitVelocity >= 40f) {
				gamemaster.GetComponent<gamemaster>().addScore(4 * coll.gameObject.GetComponent<enemy>().baseValue);
				InstantiateFloatingText(coll, 4 * coll.gameObject.GetComponent<enemy>().baseValue);
				DestroyAndSpawnNuggets(coll);
			} else if(hitVelocity >= 30f) {
				gamemaster.GetComponent<gamemaster>().addScore(3 * coll.gameObject.GetComponent<enemy>().baseValue);
				InstantiateFloatingText(coll, 3 * coll.gameObject.GetComponent<enemy>().baseValue);
				DestroyAndSpawnNuggets(coll);
			} else if(hitVelocity >= 20f) {
				gamemaster.GetComponent<gamemaster>().addScore(2 * coll.gameObject.GetComponent<enemy>().baseValue);
				InstantiateFloatingText(coll, 2 * coll.gameObject.GetComponent<enemy>().baseValue);
				DestroyAndSpawnNuggets(coll);
			} else if(hitVelocity >= 10f) {
				gamemaster.GetComponent<gamemaster>().addScore(coll.gameObject.GetComponent<enemy>().baseValue);
				InstantiateFloatingText(coll, coll.gameObject.GetComponent<enemy>().baseValue);
				DestroyAndSpawnNuggets(coll);
			}
		} else if (coll.transform.tag == "NUGGET") {
			if(hitVelocity >= 10f) {
				gamemaster.GetComponent<gamemaster>().addScore(coll.gameObject.GetComponent<nugget>().baseValue);
				InstantiateFloatingText(coll, coll.gameObject.GetComponent<nugget>().baseValue);
				gamemaster.GetComponent<gamemaster>().spawnedNuggets.Remove(coll.gameObject);
				Destroy(coll.gameObject);
			}
		}
	}
}
