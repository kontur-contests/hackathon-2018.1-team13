using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour{
	public GameObject entity;
	public Vector3 offset;
	public float position;
	public float maxPosition;
	public InfinityRoad road;
	public bool notMoveByZ;

	public void Move( float dPosition )
	{
		position = position + dPosition;
		if (position >= maxPosition) {
			position = position - maxPosition;

			road.AddBlock (position);
			Destroy (gameObject);
			return;
		}
		if (notMoveByZ) {
			gameObject.transform.position = road.basePosition + BaseRoad.instance.GetPosition (position, true);
		} else {
			gameObject.transform.position = road.basePosition + BaseRoad.instance.GetPosition (position);
		}
	}

	// Update is called once per frame
	void Update () {
		Move (Time.deltaTime * BaseRoad.instance.speed);
	}
}
