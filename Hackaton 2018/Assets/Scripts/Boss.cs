﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : EnemyController
{
	// Use this for initialization
	void Start ()
	{
		animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Wake()
	{
		animator.SetTrigger("wake");
	}
}
