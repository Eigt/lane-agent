using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	public GameObject agent;
	public GameObject pedestrian;
	public int population;

	// initialization: spawn agent and pedestrians
	void Start () {
		GameObject[] starts = GameObject.FindGameObjectsWithTag ("Destination");
		int rnd = Random.Range (0, 8);
		Vector3 aStart = new Vector3 (starts [rnd].transform.parent.position.x, 1, starts [rnd].transform.parent.position.z);
		Instantiate (agent, aStart, Quaternion.identity);
		for (int i = 0; i < population; i++) {
			spawnPedestrian ();
		}
	}

	// public so it can be called by end trigger
	public void spawnPedestrian () {
		int lane = Random.Range (0, 4);
		Vector3 start = new Vector3 (-9.5f, 1f, (float) lane);
		GameObject obj = Instantiate (pedestrian, start, Quaternion.identity);
		obj.GetComponent<PedestrianBehaviour> ().setLane (lane);
	}
}
