using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class IOManager : Miscellaneous, IPointerEnterHandler
{
	private OptionsMenu _option;
	private AccountMenu _acc;
	private HomeMenu _home;
	private ConnectionMenu _co;
	private CreditsMenu _cred;
	private StatistiquesMenu _stat;
	private RoomSelectionMenu _sroom;
	private JoinByIdMenu _jid;
	private PublicRoomMenu _proom;
	private RoomParameters _rparam;
	private CreateRoomMenu _croom;
	private RoomIsCreated _rcreated;
	private Color _previousColor, FCcolor, colHover;
	private Text _btnText;
	static private GameObject previousGo, nextGo, TridentGo;
	private Texture2D _cursorTexture;
	private CursorMode _cursorMode = CursorMode.Auto;
	private Vector2 _cursorHotspot = Vector2.zero;
	private EventSystem eventSystem;
	private bool boolSelectionChange = true;
	private bool cooldown = false;
	private InputField IF = null;

	// MOBILE : 

	public Vector2 startPos;
	public Vector2 direction;
	private Touch touch;

	public Text m_Text = null;
	string message = null;
	PointerEventData m_PointerEventData;




	public bool showDebugGUI = false;

	Touchscreen touchScreen;
	TouchState primaryTouchState = default;
	TouchState[] touchStates = new TouchState[10];

	private void OnEnable()
	{
		touchScreen = InputSystem.AddDevice<Touchscreen>("Injected TouchScreen");
		InputSystem.onBeforeUpdate += Inject;
	}

	private void OnDisable()
	{
		InputSystem.onBeforeUpdate -= Inject;
		if (touchScreen != null)
			InputSystem.RemoveDevice(touchScreen);
	}

	unsafe private void Inject()
	{
		//InputState.currentTime
		for (int i = 0; i < 10; i++)
		{
			Touch t = default;

			if (i < Input.touchCount)
			{
				t = Input.GetTouch(i);
				var ts = touchStates[t.fingerId];
				ts.position = t.position;
				ts.delta = t.deltaPosition;
				ts.pressure = t.pressure;
				ts.touchId = t.fingerId;
				ts.radius = Vector2.one * t.radius;

				//Tap Count is done inside TouchScreen controldevice implementation with new InputSystem and is kinda funky.
				//Typically this is exposed as an Accessibility Setting on the end device itself (ie: the iPhone settings menus)

				//IMPORTANT NOTE:  If you want TapCount to actually function, you need to use the latest  Input System Preview Package (1.1.0+)
				//OR!  If you continue to use 1.0.2 (verified stable) Modify InputState.cs
				//WRONG        public static double currentTime => InputRuntime.s_Instance.currentTime + InputRuntime.s_CurrentTimeOffsetToRealtimeSinceStartup;
				//RIGHT        public static double currentTime => InputRuntime.s_Instance.currentTime - InputRuntime.s_CurrentTimeOffsetToRealtimeSinceStartup;
				//                                                                                      ^ zigged when supposed to zag

				switch (t.phase)
				{
					case UnityEngine.TouchPhase.Began:
						ts.startPosition = t.position;
						ts.startTime = InputState.currentTime;
						ts.phase = UnityEngine.InputSystem.TouchPhase.Began;
						break;
					case UnityEngine.TouchPhase.Moved:
						ts.phase = UnityEngine.InputSystem.TouchPhase.Moved;
						break;
					case UnityEngine.TouchPhase.Stationary:
						ts.phase = UnityEngine.InputSystem.TouchPhase.Stationary;
						break;
					case UnityEngine.TouchPhase.Ended:
						ts.phase = UnityEngine.InputSystem.TouchPhase.Ended;
						break;
					case UnityEngine.TouchPhase.Canceled:
						ts.phase = UnityEngine.InputSystem.TouchPhase.Canceled;
						break;
				}

				touchStates[t.fingerId] = ts;
			}
		}

		foreach (var ts in touchStates)
		{
			if (ts.phase != UnityEngine.InputSystem.TouchPhase.None)
				InputSystem.QueueStateEvent(touchScreen, ts);
		}

		for (int i = 0; i < touchStates.Length; i++)
		{
			if (touchStates[i].phase == UnityEngine.InputSystem.TouchPhase.Ended)
			{
				touchStates[i].phase = UnityEngine.InputSystem.TouchPhase.None;
				InputSystem.QueueStateEvent(touchScreen, touchStates[i]);
			}
		}
	}

	private void OnGUI()
	{
		if (!showDebugGUI)
			return;

		foreach (var t in Input.touches)
		{
			var pos = new Rect(t.position, Vector2.one * 300);
			pos.y = Screen.height - pos.y;
			GUI.Label(pos, $"OG\t{t.fingerId}\t{t.phase}\t{t.tapCount}");
		}

		foreach (var touchControl in touchScreen.touches)
		{
			var touchState = touchControl.ReadValue();
			if (touchState.phase == UnityEngine.InputSystem.TouchPhase.None)
				continue;

			var pos = new Rect(touchState.position, Vector2.one * 300);
			pos.y = Screen.height - pos.y - 20;
			GUI.Label(pos, $"IS\t{touchState.touchId}\t{touchState.phase}\t{touchState.tapCount}\t{touchState.isPrimaryTouch}");
			GUILayout.Label(touchState.touchId.ToString() + "  " + touchState.phase + "  " + touchState.isTap);
		}
	}



void Start()
	{
		// SCRIPT : (nécessaire pour SendMessage) => chercher un moyen de l'enlever.
		// ---------------------------------- PATCH : ------------------------------------
		/* 		Debug.Log("Liste des scripts : ");
		GetScripts(); */
		// à enlever : 
		_option = gameObject.AddComponent(typeof(OptionsMenu)) as OptionsMenu;
		_acc = gameObject.AddComponent(typeof(AccountMenu)) as AccountMenu;
		_home = gameObject.AddComponent(typeof(HomeMenu)) as HomeMenu;
		_co = gameObject.AddComponent(typeof(ConnectionMenu)) as ConnectionMenu;
		_cred = gameObject.AddComponent(typeof(CreditsMenu)) as CreditsMenu;
		_stat = gameObject.AddComponent(typeof(StatistiquesMenu)) as StatistiquesMenu;
		_sroom = gameObject.AddComponent(typeof(RoomSelectionMenu)) as RoomSelectionMenu;
		_jid = gameObject.AddComponent(typeof(JoinByIdMenu)) as JoinByIdMenu;
		_proom = gameObject.AddComponent(typeof(PublicRoomMenu)) as PublicRoomMenu;
		_rparam = gameObject.AddComponent(typeof(RoomParameters)) as RoomParameters;
		_croom = gameObject.AddComponent(typeof(CreateRoomMenu)) as CreateRoomMenu;
		_rcreated = gameObject.AddComponent(typeof(RoomIsCreated)) as RoomIsCreated;
		// ---------------------------------- FIN PATCH : --------------------------------
		//Cursor Texture :
		_cursorTexture = Resources.Load("Miscellaneous/Cursors/BlueCursor") as Texture2D; // Texture Type = Cursor
		Cursor.SetCursor(_cursorTexture, _cursorHotspot, _cursorMode);
		//Fetch the current EventSystem. Make sure your Scene has one.
		eventSystem = EventSystem.current;
		nextGo = FirstActiveChild(GameObject.Find("Buttons"));
		eventSystem.SetSelectedGameObject(nextGo);
		ColorUtility.TryParseHtmlString("#1e90ff", out colHover);
		ColorUtility.TryParseHtmlString("#FFA500", out FCcolor);
		TridentGo = GameObject.Find("Other").transform.Find("Trident").gameObject;
		changeHover();
		// Cherche chaque menu -> liste chaque boutons par menu -> assignation de la fonction respectivement
		foreach (Transform menu in GameObject.Find("SubMenus").transform)
		{
			foreach (Transform btn in menu.Find("Buttons").transform)
				if (btn.GetComponent<Button>())
					btn.GetComponent<Button>().onClick.AddListener(delegate
					{
						MethodCall(btn.name, null, null);
					});
			if (menu.Find("Toggle Group"))
				foreach (Transform tog in menu.Find("Toggle Group").transform.GetChild(0).transform)
					if (tog.GetComponent<Toggle>())
						tog.GetComponent<Toggle>().onValueChanged.AddListener(delegate
						{
							MethodCall(menu.Find("Toggle Group").transform.GetChild(0).name, tog.GetComponent<Toggle>(), null);
						});
			if (menu.Find("InputField"))
				foreach (Transform inp in menu.Find("InputField").transform.GetChild(0).transform)
					if (inp.GetComponent<InputField>())
						inp.GetComponent<InputField>().onEndEdit.AddListener(delegate
						{
							MethodCall(menu.Find("InputField").transform.GetChild(0).name, null, inp.GetComponent<InputField>());
						});
		}
	}

	public void Update()
	{
		if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow)) // résout probleme souris/clavier avec GetKey 
		{
			/* lockMouse(true); */
			previousGo = nextGo;
			nextGo = eventSystem.currentSelectedGameObject;
			changeHover();
		}

		// Dans version finale utiliser ESCAPE à la place de space (escape quitte preview unity)
		/* 		if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2) || Input.GetKey(KeyCode.Space)) && Cursor.lockState == CursorLockMode.Locked && cooldown == false)
				{ */
		/* lockMouse(false); */
		/* 			nextGo = eventSystem.currentSelectedGameObject;
					resetHoverPreviousGo(); */
		// EVITE SPAM CLIC
		/* 			Invoke("ResetCooldown", 5.0f);
					cooldown = true; */
		/* } */

		/* --------------------- PATCH INPUTFIELD --------------------- */
		// il faut mieux gérer l'inputfield pour la saisie (entree et escape)
		/* 		if (IF!=null) 
					if(IF.isFocused)
						{
							previousGo = nextGo;
							nextGo = eventSystem.currentSelectedGameObject;
							changeHover();
							lockMouse(true);
							if(Input.GetKey(KeyCode.Return)) // touche enter
								lockMouse(false); 
						} */
		/* ------------------ FIN PATCH INPUTFIELD -------------------- */

	}

	private void lockMouse(bool b)
	{
		Cursor.lockState = b ? CursorLockMode.Locked : CursorLockMode.None;
		Cursor.visible = !b;
	}

	private void ResetCooldown() => cooldown = false; // EVITE SPAM CLIC

	public void OnPointerEnter(PointerEventData eventData)
	{
		Debug.Log(eventData.pointerCurrentRaycast);
		// A DECOMMENTER quand pointerCurrentRaycast fonctionne 

		IF = eventData.pointerCurrentRaycast.gameObject.GetComponent<InputField>(); // PATCH INPUTFIELD 
		if (!IF)
		{
			nextGo = eventData.pointerCurrentRaycast.gameObject.transform.parent.gameObject;
			selectionChange();
		} 
	}


	/* 	void OnGUI() // TROP LENT (a gardé pour détecter une touche quelconque)
{
	if(Input.anyKeyDown &&  Event.current.isKey)
			switch(Event.current.keyCode.ToString()){
			case "UpArrow" : case "DownArrow" : case "LeftArrow" : case "RightArrow" :
			break;
			}
} */

	public void selectionChange()
	{
		// Aparté : (Les inpufield : le "Text" doit avoir du raycast pour fonctionner donc à ne pas désactiver)
		// nextGo.GetComponent<Button>() est testé d'abord donc si false la partie gauche du ET non testé donc pas d'erreur
		bool btn = nextGo.GetComponent<Button>() && nextGo.GetComponent<Button>().interactable;
		bool slider = nextGo.transform.GetChild(0).name == "Handle";
		bool inputfd = nextGo.transform.parent.name == "InputField";
		// RAYCAST NECESSAIRE INPUTFIELD (sur 1 des 3 composante, actuellement sur texte) => petit bug de hover
		// Si nextGo != currentSelected ET (selection de : slider ou bouton ou toggle)
		if (nextGo != eventSystem.currentSelectedGameObject && (slider || btn || nextGo.GetComponent<Toggle>()))
		{
			previousGo = eventSystem.currentSelectedGameObject;
			eventSystem.SetSelectedGameObject(nextGo);
			boolSelectionChange = true;
		}
		else
			boolSelectionChange = false;
		changeHover();
	}

	public void textColor(Color c, int s, GameObject go)
	{
		_btnText = go.GetComponentInChildren<Text>();
		_btnText.color = c;
		_btnText.fontSize += s;
	}

	public void colorImage(GameObject go, byte r, byte g, byte b, byte f, bool changeColor)
	{
		Image image = go.transform.GetChild(0).gameObject.GetComponent<Image>();
		if (changeColor == false) // transparence 
			image.color = new Color(image.color.r, image.color.g, image.color.b, (float)f / 255);
		else
			image.color = new Color32(r, g, b, f); // transparence et couleur
	}

	public void tridentHover(Component c)
	{
		if (!(c.transform.parent.name == "ForgottenPwdUser" || c.transform.parent.name == "CGU"))
		{
			TridentGo.SetActive(true);
			GameObject curBtn = c.gameObject;
			float width = curBtn.GetComponent<RectTransform>().rect.width;
			float height = curBtn.GetComponent<RectTransform>().rect.height;
			GameObject TF = TridentGo.transform.Find("TridentFront").gameObject;
			GameObject TB = TridentGo.transform.Find("TridentBack").gameObject;
			TF.transform.position = curBtn.transform.position + new Vector3((int)(width / 2.7)/* 2 + 90 */, 0, 0);
			TB.transform.position = curBtn.transform.position - new Vector3((int)(width / 3.4) /* 2 + 20 */, 0, 0);
		}
	}

	public void resetHoverPreviousGo()
	{
		if (previousGo != null)
		{
			//GameObject.Find("Trident").SetActive(false);
			Component previousTarget = previousGo.transform.GetChild(0).GetComponent<Component>();
			bool FC = previousTarget.transform.parent.name == "ForgottenPwdUser" || previousTarget.transform.parent.name == "CGU";
			switch (previousTarget.name)
			{ // previousGO
				case "RawImage": // GIF : A changer (mettre autre chose que dezoom)
					previousGo.GetComponentInChildren<RawImage>().rectTransform.sizeDelta = new Vector2(50, 50);
					break;
				case "Unselected": // IMAGE
					colorImage(previousGo, 0, 0, 0, 255, false);
					break;
				case "Text": // BOUTON
					_previousColor = FC ? FCcolor : new Color(1, 1, 1, 1); // COULEUR PAR DEFAUT (RESET COLOR)
					textColor(_previousColor, -3, previousGo);
					break;
				case "Background": // TOGGLE
					colorImage(previousGo, 0, 0, 0, 255, false); // (à changer)
					break;
				case "Handle": // SLIDER
					colorImage(previousGo, 255, 255, 255, 255, true); // COULEUR PAR DEFAUT (RESET COLOR)
					break;
				default:
					break;
			}
		}
	}

	public void changeHover()
	{
		if (boolSelectionChange == true)
		{
			if (TridentGo.activeSelf == true) // TRIDENT
				TridentGo.SetActive(false);
			resetHoverPreviousGo();
			Component nextTarget = nextGo.transform.GetChild(0).GetComponent<Component>();
			//Debug.Log("next" + nextTarget);
			switch (nextTarget.name)
			{
				case "RawImage": // GIF : A changer (mettre autre chose que zoom)
					nextGo.GetComponentInChildren<RawImage>().rectTransform.sizeDelta = new Vector2(70, 70);
					break;
				case "Unselected": // IMAGE
					colorImage(nextGo, 0, 0, 0, 0, false);
					break;
				case "Text": // BOUTON
					tridentHover(nextTarget); // TRIDENT
					textColor(colHover, 3, nextGo);
					break;
				case "Background": // TOGGLE
					colorImage(nextGo, 0, 0, 0, 125, false); // semi transparent (à changer)
					break;
				case "Handle": // SLIDER
					colorImage(nextGo, 47, 79, 79, 255, true); // équivalent à #FF4500
					break;
				default:
					break;
			}
		}
	}

	public void NewMenuSelectButton()
	{
		if (HasMenuChanged() == true)
		{
			string previousMenu = GetPreviousMenu().name.Remove(GetPreviousMenu().name.Length - 4);
			foreach (Transform child in GameObject.Find("Buttons").transform)
			{
				nextGo = FirstActiveChild(GameObject.Find("Buttons"));
				if (child.name.Contains(previousMenu) && child.gameObject.activeSelf)
				{
					nextGo = child.gameObject;
					break;
				}
			}

			SetMenuChanged(false);
			selectionChange();
		}
	}


	public void MethodCall(string methode, Toggle tog, InputField inp)
	{
		GameObject.Find("SoundController").GetComponent<AudioSource>().Play();
		if (inp == null)
			gameObject.SendMessage(methode, tog);
		else gameObject.SendMessage(methode, inp);
		if (tog == null || inp == null)
			NewMenuSelectButton();
	}
}
