using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRoadPosition : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		if (BaseRoad.instance == null)
		{
			return;
		}

		BaseRoad.instance.PlayerPosition (gameObject.transform.position);
	}
}
