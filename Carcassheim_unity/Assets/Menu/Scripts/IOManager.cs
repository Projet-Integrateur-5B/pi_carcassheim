using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Reflection;


public class IOManager : Miscellaneous/* , IPointerEnterHandler, IPointerExitHandler */
{

	private Texture2D _cursorTexture;
	private CursorMode _cursorMode = CursorMode.Auto;
	private Vector2 _cursorHotspot = Vector2.zero;

    	void Start()
	{
		// Cursor Texture :
		_cursorTexture = Resources.Load("basic_01 BLUE") as Texture2D;
		Cursor.SetCursor(_cursorTexture, _cursorHotspot, _cursorMode);
	}
}

