using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfinityWagon : MonoBehaviour {

	public Vector3 basePosition;
	public float position;
	public float height;

	void Start()
	{
		position = transform.localPosition.x / BaseRoad.instance.xOffset;
	}
	void Update () {
		
		//Vector3 startCurve = BaseRoad.instance.GetCurveValue(position - height / 2);
		//Vector3 endCurve = BaseRoad.instance.GetCurveValue(position + height / 2);

		//Vector3 midCurve = (endCurve - startCurve) * 0.5f + startCurve;

		gameObject.transform.position = BaseRoad.instance.GetPosition( position );
		Vector3 rotation = BaseRoad.instance.GetPosition( position + 1 );
		//gameObject.transform.
		//Quaternion.LookRotation();
		//Quaternion quat = Quaternion.LookRotation( gameObject.transform.position );
		gameObject.transform.LookAt( rotation );

		//Quaternion.RotateTowards(Vector3.forward, rotation, 999f);
		//Quaternion
	}
}