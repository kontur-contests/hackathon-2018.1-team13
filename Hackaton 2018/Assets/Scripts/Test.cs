using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

	public int height = 10;
	public int width = 10;
	public float force = 10;
	public float torque = 10;

	// Use this for initialization
	void Start () {
		GameObject[,] tower = new GameObject[ height, width ];
		for ( int j = 0; j < width;j++)
		for ( int i = 0; i< height;i++)
		{
			GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
			tower[ i,j ] = go;
			go.transform.position = new Vector3( j , i +1f, 0f);
			go.AddComponent<Rigidbody>();
			FixedJoint jointX = go.AddComponent<FixedJoint>();
			jointX.breakForce = force;
			jointX.breakTorque = torque;
			FixedJoint jointY = go.AddComponent<FixedJoint>();
			jointY.breakForce = force * 1.5f;
			jointY.breakTorque = torque * 1.5f;
			if ( i> 0 )
			{
				jointX.connectedBody = tower[ i - 1, j].GetComponent<Rigidbody>();
			}
			if ( j > 0)
			{
				jointY.connectedBody = tower[i, j - 1].GetComponent<Rigidbody>();
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
