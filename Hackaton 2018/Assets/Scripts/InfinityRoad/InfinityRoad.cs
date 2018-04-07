using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour{
	public GameObject entity;
	public Vector3 basePosition;
	public Vector3 offset;
	public float position;
	public float maxPosition;
	public InfinityRoad road;

	public void Move( float dPosition )
	{
		position = position + dPosition;
		if (position >= maxPosition) {
			position = position - maxPosition;
		}

		entity.transform.position = basePosition + position * offset;
	}

	// Update is called once per frame
	void Update () {
		Move (Time.deltaTime * road.speed);
	}
}

public class InfinityRoad : MonoBehaviour {

	public GameObject[] blockPrefab;
	public List<Block> blocks;
	public Vector3 startPosition;
	public Vector3 offset;
	public float speed;
	public float maxcount;

	// Use this for initialization
	void Start () {
		Generate ();
	}

	// Use this for initialization
	void Generate () {
		for (int i = 0; i < maxcount; i++) {
			Block newBlock = AddBlock ();
			newBlock.Move (i);
		}
	}

	Block AddBlock()
	{
		GameObject randomBlock = blockPrefab [Random.Range (0, blockPrefab.GetLength (0))];
		GameObject entity = Instantiate (randomBlock);
		Block newBlock = entity.AddComponent<Block> ();
		newBlock.entity = entity;
		newBlock.basePosition = startPosition;
		newBlock.offset = offset;
		newBlock.maxPosition = maxcount;
		newBlock.road = this;

		return newBlock;
	}
}
