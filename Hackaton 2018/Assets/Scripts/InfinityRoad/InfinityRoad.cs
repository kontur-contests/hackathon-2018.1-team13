﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfinityRoad : MonoBehaviour {
	public GameObject[] blockPrefab;
	public Vector3 basePosition;
	public float distance = 10;
	public float maxLenght = 100;
	public bool notMoveByZ = false;

	// Use this for initialization
	void Start () {
		basePosition = this.transform.localPosition;
		Generate ();
	}

	// Use this for initialization
	void Generate () {
		for (float i = -maxLenght; i < maxLenght; i = i + distance) {
			 AddBlock (i);
		}
	}

	public void AddBlock( float position )
	{
		GameObject randomBlock = blockPrefab [Random.Range (0, blockPrefab.GetLength (0))];
		GameObject entity = Instantiate (randomBlock);
		Block newBlock = entity.AddComponent<Block> ();
		newBlock.maxPosition = maxLenght;
		newBlock.road = this;
		newBlock.position = position;
		newBlock.notMoveByZ = notMoveByZ;
		newBlock.Move (0);

		newBlock.transform.SetParent (gameObject.transform);
	}
}
