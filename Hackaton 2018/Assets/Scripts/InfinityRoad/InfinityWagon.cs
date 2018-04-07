using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfinityWagon : MonoBehaviour {

	public Vector3 basePosition;
	public float position;
	public float height;

	public void Update () {

		gameObject.transform.position = BaseRoad.instance.GetPosition( position );
		Vector3 rotation = BaseRoad.instance.GetPosition( position - 1 );
		gameObject.transform.LookAt( rotation );

	}
}