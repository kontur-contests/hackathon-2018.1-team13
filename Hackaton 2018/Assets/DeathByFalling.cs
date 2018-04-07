using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathByFalling : MonoBehaviour {

	GameObject deathPlayer;

	void OnTriggerEnter(Collider coll) {
		if (coll.gameObject.tag == "Player") {
			deathPlayer = coll.gameObject;
		}
	}

	void Update() {
		if (deathPlayer != null) {
			deathPlayer.transform.position = deathPlayer.transform.position + new Vector3( Time.deltaTime * BaseRoad.instance.speed * BaseRoad.instance.xOffset, Time.deltaTime * 20, 0);
		}
	}
}
