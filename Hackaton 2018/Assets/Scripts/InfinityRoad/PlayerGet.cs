using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGet : MonoBehaviour {

	void OnTriggerEnter(Collider coll) {
		if (coll.gameObject.tag == "Player") {
			coll.gameObject.transform.SetParent (gameObject.transform.parent);
		}
	}
}
