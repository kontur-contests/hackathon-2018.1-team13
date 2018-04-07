using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGet : MonoBehaviour {

	void OnCollisionEnter3D(Collision coll) {
		if (coll.gameObject.tag == "Player") {
			print("Test");
		}
	}
}
