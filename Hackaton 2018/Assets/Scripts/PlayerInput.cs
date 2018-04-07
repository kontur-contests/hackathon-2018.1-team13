using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput
{
	public float mouse_sense = 2;

	public Vector2 keyAxis = new Vector2();
	public Vector2 mouseAxis = new Vector2();
	public bool b_Jump = false;
	public bool b_Sprint = false;
	public bool b_Crouch = false;
	public bool b_LMB = false;
	public bool b_RMB = false;
	public bool b_Reload = false;
	KeyCode[] weaponKeyCodes = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6 };
	public int b_SwitchWeapon = -1;

	public void Update()
	{
		keyAxis.x = Input.GetAxis("Horizontal");
		keyAxis.y = Input.GetAxis("Vertical");

		mouseAxis.x = Input.GetAxis("Mouse X") * mouse_sense;
		mouseAxis.y = Input.GetAxis("Mouse Y") * mouse_sense;

		b_Jump = Input.GetAxis("Jump") > 0;
		b_Sprint = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
		b_Crouch = Input.GetKey(KeyCode.LeftControl);
		b_LMB = Input.GetMouseButton(0);
		b_RMB = Input.GetMouseButton(1);
		b_Reload = Input.GetKey(KeyCode.R);

		b_SwitchWeapon = -1;
		for (int i = 0; i < weaponKeyCodes.Length; i++)
			if (Input.GetKey(weaponKeyCodes[i]))
			{
				b_SwitchWeapon = i;
				break;
			}
	}

	public PlayerInput()
	{
		Update();
	}
}
