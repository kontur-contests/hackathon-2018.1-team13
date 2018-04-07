using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour{
	public GameObject entity;
	public Vector3 offset;
	public float position;
	public float maxPosition;
	public InfinityRoad road;

	public void Move( float dPosition )
	{
		position = position + dPosition;
		if (position >= maxPosition) {
			position = position - maxPosition;

			road.AddBlock (position);
			Destroy (gameObject);
			return;
		}
		entity.transform.position = road.basePosition + BaseRoad.instance.GetPosition( position );
	}

	// Update is called once per frame
	void Update () {
		Move (Time.deltaTime * BaseRoad.instance.speed);
	}
}
	
public class InfinityRoad : MonoBehaviour {
	public GameObject[] blockPrefab;
	public Vector3 basePosition;
	public float maxLenght = 20;

	// Use this for initialization
	void Start () {
		basePosition = this.transform.localPosition;
		Generate ();
	}

	// Use this for initialization
	void Generate () {
		for (int i = 0; i < maxLenght; i++) {
			 AddBlock (i);
		}
	}

	public void AddBlock( float position )
	{
		GameObject randomBlock = blockPrefab [Random.Range (0, blockPrefab.GetLength (0))];
		GameObject entity = Instantiate (randomBlock);
		Block newBlock = entity.AddComponent<Block> ();
		newBlock.entity = entity;
		newBlock.maxPosition = maxLenght;
		newBlock.road = this;
		newBlock.position = position;
		newBlock.Move (0);

		newBlock.transform.SetParent (gameObject.transform);
	}
}
