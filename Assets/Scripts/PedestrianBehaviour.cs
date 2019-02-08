using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianBehaviour : MonoBehaviour {

	public float interval;
	public float speed;

	List<Action> actions;
	float nextAct;
	int current;
	float laneChangeTime;
	int lane;
	int oldLane;

	// initialization
	void Awake () {
		laneChangeTime = 1 / speed;
		actions = applyGrammar (interval, 0);

		float total = 0;
		foreach (Action a in actions) {
			total = total + a.time;
		}
		if (total != interval) {
			Debug.Log ("Total of subinterval times does not match interval time.");
		}
		current = 0;
		nextAct = Time.time + actions [current].time;
		if (actions [current].key == 2) {
			actions [current] = new Action(0, actions[current].time);
		}
	}
	
	// Update is called once per frame
	void Update () {
		// avoid possibility of going off the lanes
		if (lane < 0) {
			oldLane = 0;
			lane = 1;
		} else if (lane > 3) {
			oldLane = 3;
			lane = 2;
		}
		// check if subinterval time is up
		if (Time.time > nextAct) {
			current++;
			if (current >= actions.Count) {
				current = 0;
			}
			nextAct = Time.time + actions [current].time;
			if (actions [current].key == 2) {
				// if next action is lane change, determine new lane
				oldLane = lane;
				if (lane < 1) {
					lane = 1;
				} else if (lane > 2) {
					lane = 2;
				} else {
					if (Random.value < 0.5f) {
						lane++;
					} else {
						lane--;
					}
				}
			}
		}
		doAction (actions [current].key);
	}
	// implements different types of movement
	void doAction (int key) {
		if (key == 0) {
			// walk forward
			transform.Translate (transform.right * speed * Time.deltaTime, Space.World);
		} else if (key == 1) {
			// move at half speed
			transform.Translate (transform.right * speed * 0.5f * Time.deltaTime, Space.World);
		} else if (key == 2) {
			// change lanes
			if (lane > oldLane) {
				// go up
				transform.Translate (new Vector3(0, 0, 1) * speed * Time.deltaTime, Space.World);
			} else {
				// go down
				transform.Translate (new Vector3(0, 0, -1) * speed * Time.deltaTime, Space.World);
			}
		} else if (key == 3) {
			// walk backwards at half speed
			transform.Translate (transform.right * speed * -0.5f * Time.deltaTime, Space.World);
		} else if (key == 4) {
			// pause
		}
	}
	// splits interval (or subintervals) according to grammar
	List<Action> applyGrammar (float inter, int step) {
		List<Action> left = new List<Action>();
		List<Action> right = new List<Action>();
		float subInter = inter * 0.5f;
		// for each new subinterval, choose between splitting it further or making it a "terminal" move
		// biased towards splitting at step 0, increasingly biased towards terminals afterwards
		if (Random.value * step < 0.25f) {
			left = applyGrammar (subInter, step+1);
		} else {
			float rnd = Random.value;
			if (rnd < 0.35f) {
				// 35% chance of simple move
				left.Add (new Action(0, subInter));
			} else if (rnd < 0.7f) {
				// 35% chance of half-speed move
				left.Add (new Action(1, subInter));
			} else {
				// 30% chance of lane change
				if (subInter > laneChangeTime) {
					// can only do lane change if enough time in subinterval
					left.Add (new Action (2, laneChangeTime));
					left.Add (new Action (0, subInter - laneChangeTime));
				} else {
					// if lane change not possible, default to simple move
					left.Add (new Action(0, subInter));
				}
			}
		}
		if (Random.value * step < 0.25f) {
			right = applyGrammar (subInter, step+1);
		} else {
			float rnd = Random.value;
			if (rnd < 0.40f) {
				// 40% chance of simple move
				right.Add (new Action(0, subInter));
			} else if (rnd < 0.60f) {
				// 30% of chance of reverse-and-return
				right.Add (new Action(3, subInter*0.5f));
				right.Add (new Action(0, subInter*0.5f));
			} else {
				// 30% chance of pause
				right.Add (new Action(4, subInter));
			}
		}

		List<Action> subList = new List<Action>();
		subList.AddRange(left);
		subList.AddRange(right);
		return subList;
	}

	struct Action {
		public int key;
		public float time;

		public Action(int k, float t){
			key = k;
			time = t;
		}
	}

	public void setLane (int l) {
		lane = l;
	}
}
