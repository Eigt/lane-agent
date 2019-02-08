using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyByContact : MonoBehaviour {

	public GameObject gc;

	void OnTriggerEnter (Collider other) {
		if (other.tag == "Player") {
			// agents & pedestrians
			Destroy (other.gameObject);
			if (other.name != "Agent") {
				// if a pedestrian, spawn new pedestrian
				gc.GetComponent<GameController> ().spawnPedestrian ();
			}
		}
	}
}
