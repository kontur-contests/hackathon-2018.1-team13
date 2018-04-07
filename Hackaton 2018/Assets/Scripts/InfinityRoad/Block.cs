using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour{
	public GameObject entity;
	public float position;
	public Vector3 offsetPosition = new Vector3(0,0,0);
	public float maxPosition;
	public InfinityRoad road;
	public bool notMoveByZ;

	public void Move( float dPosition )
	{
		position = position + dPosition;
		if (position <= -maxPosition + BaseRoad.instance.playerPosition) {
			position = position + maxPosition * 2 + BaseRoad.instance.playerPosition;

			road.AddBlock (position);
			Destroy (gameObject);
			return;
		}

		gameObject.transform.position = offsetPosition + road.basePosition + BaseRoad.instance.GetPosition (position, true);
		
	}

	// Update is called once per frame
	void Update () {
		Move (-Time.deltaTime * BaseRoad.instance.speed);
	}
}
