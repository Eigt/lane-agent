using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentBehaviour : MonoBehaviour {

	UnityEngine.AI.NavMeshAgent nav;
	UnityEngine.AI.NavMeshPath path;
	GameObject[] destinations;
	bool roomsCleared;

	// initialization
	void Start () {
		nav = GetComponent<UnityEngine.AI.NavMeshAgent> ();
		path = new UnityEngine.AI.NavMeshPath ();
		destinations = GameObject.FindGameObjectsWithTag ("Destination");
		roomsCleared = false;
	}
	
	// implements the agent's behaviour tree
	void Update () {
		// is agent at a destination?
		for (int i = 0; i < destinations.Length; i++) {
			Vector3 diff = destinations [i].transform.position - this.transform.position;
			float dist = diff.sqrMagnitude;
			if (dist < 1.1f) {
				// yes -> remove from list
				GameObject[] newDests = new GameObject[destinations.Length - 1];
				for (int j = 0; j < destinations.Length-1; j++) {
					if (j < i) {
						newDests [j] = destinations [j];
					} else {
						newDests [j] = destinations [j + 1];
					}
				}
				destinations = newDests;
				return;
			}
		}
		// have all destinations been visited?
		if (destinations.Length < 1) {
			// yes -> is there a path to the end?
			if (roomsCleared) {
				return;
			}
			roomsCleared = true;
			// find the closer part of the end strip
			GameObject north = GameObject.Find ("North");
			GameObject south = GameObject.Find ("South");
			Vector3 diffN = north.transform.position - this.transform.position;
			Vector3 diffS = south.transform.position - this.transform.position;
			float distN = diffN.sqrMagnitude;
			float distS = diffS.sqrMagnitude;
			if (distN < distS) {
				if (nav.CalculatePath (north.transform.position, path)) {
					// yes -> head there
					nav.enabled = true;
					nav.SetPath (path);
					return;
				}
			} else {
				if (nav.CalculatePath (south.transform.position, path)) {
					// yes -> head there
					nav.enabled = true;
					nav.SetPath (path);
					return;
				}
			}
		} else { // some destination remains
			// is there a path to the closest room?
			GameObject closest = null;
			float closestDist = Mathf.Infinity;
			foreach (GameObject dest in destinations) {
				Vector3 diff = dest.transform.position - this.transform.position;
				float dist = diff.sqrMagnitude;
				if (dist < closestDist) {
					closestDist = dist;
					closest = dest;
				}
			}
			if (UnityEngine.AI.NavMesh.CalculatePath (this.transform.position, closest.transform.parent.position, UnityEngine.AI.NavMesh.AllAreas, path)) {
				// yes -> head there
				nav.enabled = true;
				nav.SetDestination (closest.transform.position);
				return;
			}
		}
		// if no action taken, idle
		nav.enabled = false;
	}
}
