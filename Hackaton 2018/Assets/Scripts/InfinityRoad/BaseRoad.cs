using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseRoad : MonoBehaviour {

	public static BaseRoad instance = null;

	public AnimationCurve roadCurve;
	public float speed = 1;
	public float xOffset = 5f;
	public float nullPosCurve;
	public float realPosition = 0;
	public float curvePower = 0.1f;
	public float zOffset = 50f;
	public float playerPosition = 0f;

	public void PlayerPosition(Vector3 position)
	{
		playerPosition = position.x / xOffset;
	}

	public Vector3 GetPosition(float deltaPosition, bool notUseCurve)
	{
		if (notUseCurve) {
			return new Vector3 (deltaPosition * xOffset, 0, 0);
		} else {
			return GetPosition ( deltaPosition);
		}
	}

	public Vector3 GetPosition(float deltaPosition)
	{
		return new Vector3 (deltaPosition * xOffset, 0, 0) + GetCurve ( deltaPosition );
	}

	// Update is called once per frame
	void Update () {
		realPosition = realPosition + Time.deltaTime * speed;
		nullPosCurve = GetCurveValue(realPosition);
	}
	public float GetCurveValue(float position) {
		return roadCurve.Evaluate (position * curvePower) - 1;
	}

	public Vector3 GetCurve(float deltaPosition) {
		float currentCurve = GetCurveValue(realPosition + deltaPosition);
		float curveOffset = nullPosCurve - currentCurve;

		return curveOffset * new Vector3 (0, 0, zOffset);
	}

	void Awake() {
		if (instance == null)
			instance = this;
		else
			Destroy (gameObject);
		DontDestroyOnLoad (this);
	}
}