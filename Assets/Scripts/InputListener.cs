﻿using UnityEngine;
using System.Collections;

public class InputListener : MonoBehaviour
{
	public delegate void InputDown(KeyCode key);
	public static event InputDown inputDownEvent;
	public delegate void InputUp(KeyCode key);
	public static event InputUp inputUpEvent;
	public delegate void InputHeld(KeyCode key);
	public static event InputHeld inputHeldEvent;
	
	private KeyCode[] keys;
	
	private void Start()
	{
		keys = new KeyCode[1];
		
		keys[0] = KeyCode.Escape;
	}
	
	private void Update()
	{
		int i;
	
		for (i = 0; i < keys.Length; ++i) {
			if (Input.GetKeyDown(keys[i])) {
				if (inputDownEvent != null) inputDownEvent(keys[i]);
			}
		}
		
		for (i = 0; i < keys.Length; ++i) {
			if (Input.GetKeyUp(keys[i])) {
				if (inputUpEvent != null) inputUpEvent(keys[i]);
			}
		}
		
		for (i = 0; i < keys.Length; ++i) {
			if (Input.GetKey(keys[i])) {
				if (inputHeldEvent != null) inputHeldEvent(keys[i]);
			}
		}
	}
}