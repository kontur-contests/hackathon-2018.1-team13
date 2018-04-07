using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfinityRoad : MonoBehaviour {
	public GameObject[] blockPrefab;
	public Vector3 basePosition;
	public float distance = 10f;
	public float entropy = 0.001f;
	public float maxLenght = 100f;
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

		float maximumRotation = Mathf.Clamp( ( position + BaseRoad.instance.realPosition ) * 0.04f, 0, 45f);
		float maximumMove = Mathf.Clamp( distance + ( position + BaseRoad.instance.realPosition ) * entropy, 0, 100f);

		newBlock.transform.Rotate (new Vector3( Random.Range (0, maximumRotation), Random.Range (0, maximumRotation * 6), 0));
		newBlock.offsetPosition = new Vector3 (Random.Range (0, maximumMove), 0, Random.Range (0, maximumMove));

		newBlock.transform.SetParent (gameObject.transform);
	}
}
