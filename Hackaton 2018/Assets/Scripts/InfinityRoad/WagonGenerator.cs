using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WagonGenerator : MonoBehaviour {

	public GameObject wagonPrefab;
	public int wagonQuantity = 20;

	// Use this for initialization
	void Start () {
		for (int i = 1; i < wagonQuantity; i++) {
			InfinityWagon wagon = Instantiate (wagonPrefab).GetComponentInChildren<InfinityWagon> ();
			wagon.position = i * 4f;
			wagon.Update();

			wagon.transform.SetParent (gameObject.transform);
		}
	}
}
