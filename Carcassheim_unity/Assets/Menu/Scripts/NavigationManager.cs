using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Reflection;

// include fonctions du script via la classe HomeMenu incluant elle meme (ConnectionMenu + Miscellaneous + Monobehaviour)
public class NavigationManager : Miscellaneous, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler 
{
	private OptionsMenu _option;
	private AccountMenu _acc;
	private HomeMenu _home;
	private ConnectionMenu _co;
	private CreditsMenu _cred;
	private StatistiquesMenu _stat;
	private RoomSelectionMenu _sroom;
	private Texture2D _cursorTexture;
	private CursorMode _cursorMode = CursorMode.Auto;
	private Vector2 _cursorHotspot = Vector2.zero;
	private Color _previousColor;
	private Text _btnText;
	private bool _tmpBool;
	private static int s_i = 0;
	public static bool s_ibool = true;
	public static bool s_hbool = false;
	public static bool s_pbool = false;
	public static bool s_keyboardOn = false;
	private string _pname;
	private string _hname;
	private static bool clickEnter = false;
	private EventSystem m_EventSystem;
	private GameObject currentGo;
	// Start is called before the first frame update
	void Start()
	{
		// Cursor Texture :
		_cursorTexture = Resources.Load("basic_01 BLUE") as Texture2D;
		Cursor.SetCursor(_cursorTexture, _cursorHotspot, _cursorMode);
		// SCRIPT :
		_option = gameObject.AddComponent(typeof(OptionsMenu)) as OptionsMenu;
		_acc = gameObject.AddComponent(typeof(AccountMenu)) as AccountMenu;
		_home = gameObject.AddComponent(typeof(HomeMenu)) as HomeMenu;
		_co = gameObject.AddComponent(typeof(ConnectionMenu)) as ConnectionMenu;
		_cred = gameObject.AddComponent(typeof(CreditsMenu)) as CreditsMenu;
		_stat = gameObject.AddComponent(typeof(StatistiquesMenu)) as StatistiquesMenu;
		_sroom = gameObject.AddComponent(typeof(RoomSelectionMenu)) as RoomSelectionMenu;
	/* SetCursorVisible(false); */
	}

	void OnEnable()
	{
		//Fetch the current EventSystem. Make sure your Scene has one.
		m_EventSystem = EventSystem.current;
	}

	// Update is called once per frame
	void Update()
	{
		if (Keyboard.current.anyKey.wasPressedThisFrame)
			Debug.Log("ahhhhhhhhhhhhhhhhhhhhh");
		// A AMELIORER
		if (clickEnter)
		{
			if (Input.GetMouseButtonDown(0))
			{
				Debug.Log(m_EventSystem.currentSelectedGameObject);
				onPress(m_EventSystem.currentSelectedGameObject.name);
			}
		}
	}

	// PARTIE COMMUNE : 
	////////////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////// FONCTIONNEL //////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////////////
	Dictionary<string, string> matching = new Dictionary<string, string>()
	{// HomeMenu :
	{"Btn Connexion", "ShowConnection"}, {"Btn Jouer", "Jouer"}, {"Btn Statistiques", "ShowStatistiques"}, {"Btn Options", "ShowOptions"}, {"Btn Quitter le jeu", "Quitter"}, // StatistiquesMenu :
	{"Btn Retour Stat", "HideStatistiques"}, // OptionsMenu :
	{"Btn Retour Opt", "HideOptions"}, {"Btn Son", "SwitchSound"}, {"Toggle French", "FlagsToggle"}, {"Toggle English", "FlagsToggle"}, {"Toggle German", "FlagsToggle"}, {"Btn Musique", "SwitchMusic"}, {"Btn Fenêtré", "FullScreen"}, {"Btn Aide", "Help"}, {"Btn Credits", "ShowCredits"}, // CreditsMenu :
	{"Btn Retour Credits", "HideCredits"}, // ConnectionMenu :
	{"Btn Retour Co", "HideConnection"}, {"Btn ForgottenPwdUser", "ForgottenPwdUser"}, {"Btn Se Connecter", "Connect"}, {"Btn Creer un compte", "CreateAccount"}, {"Toggle AfficherMdp", "HideShowPwd"}, // AccountMenu :
	{"Btn Retour Crea CA", "HideAccount"}, {"Btn Creer votre compte", "CreateAccountConnected"}, {"Toggle AfficherMdp CA", "HideShowPwdConf"}, // RoomSelectionMenu :
	{"Btn Retour RoomSelection", "HideRoomSelection"}, {"Btn Options Pop-Up", "ShowPopUpOptions"}};
	public void MethodeCall(string myString)
	{
		string methode;
		bool TGV = this.matching.TryGetValue(myString, out methode);
		// PATCH pour que ça fonctionne 
		// A AMELIORER (rendre ça plus propre que 50 if avec les X.invoke)
		if (TGV)
		{
			if (GetCurrentMenu().name == "HomeMenu")
				_home.Invoke(methode, 0);
			if (GetCurrentMenu().name == "StatistiquesMenu")
				_stat.Invoke(methode, 0);
			if (GetCurrentMenu().name == "OptionsMenu")
				_option.Invoke(methode, 0);
			if (GetCurrentMenu().name == "CreditsMenu")
				_cred.Invoke(methode, 0);
			if (GetCurrentMenu().name == "ConnectionMenu")
				_co.Invoke(methode, 0);
			if (GetCurrentMenu().name == "CreateAccountMenu")
				_acc.Invoke(methode, 0);
			if (GetCurrentMenu().name == "RoomSelectionMenu")
				_sroom.Invoke(methode, 0);
		}
		else
			Debug.Log("CALL " + " ERROR");
	}


	// A TESTER PLUS TARD : 
	void OnSelectionChange()
	{
		Debug.Log("selection changed");
	}

	//Do this when the selectable UI object is selected.
	public void OnSelect(BaseEventData eventData)
	{
		Debug.Log("SELECTED" + currentGo);
		HighlightEnter(currentGo.name);
	}

	//Do this when the selectable UI object is selected.
	public void OnDeselect(BaseEventData eventData)
	{
		Debug.Log("DESELECTED" + currentGo);
		if (currentGo != null)
			HighlightExit(currentGo.name);
		currentGo = null;
		/* this.GetComponent<Selectable>().OnPointerExit(null); */
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////// A AMELIORER //////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////////////
	public void HighlightEnter(string name)
	{
		if (!GameObject.Find(name).GetComponent<Toggle>() && GameObject.Find(name).GetComponentInChildren<Text>())
		{
			_btnText = GameObject.Find(name).GetComponentInChildren<Text>();
			_tmpBool = StrCompare(name, "Btn Jouer") || StrCompare(name, "Btn Statistiques");
			if (!GetState() && _tmpBool)
				TryColorText(_btnText, Color.grey, "#808080");
			else
			{
				_previousColor = _btnText.color;
				TryColorText(_btnText, Color.blue, "#1e90ff");
				_btnText.fontSize += 3;
			}
		}
	}

	public void HighlightExit(string name)
	{
		Debug.Log(name);
		if (!GameObject.Find(name).GetComponent<Toggle>() && GameObject.Find(name).GetComponentInChildren<Text>())
		{
			bool tmpBool = StrCompare(name, "Btn Jouer") || StrCompare(name, "Btn Statistiques");
			if (GetState() || !tmpBool)
			{
				_btnText.color = _previousColor;
				_btnText.fontSize -= 3;
			}
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////////////



	
	// PARTIE SOURIS :
	////////////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////// FONCTIONNEL //////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////////////
	//Do this when the selectable UI object is selected.
	public void OnPointerEnter(PointerEventData eventData)
	{
		if (eventData.pointerCurrentRaycast.gameObject != null)
		{
			// Recupère le GO du hover (text si bouton d'où parent pour avoir boutton) :
			currentGo = eventData.pointerCurrentRaycast.gameObject.transform.parent.gameObject;
			bool tmp = currentGo.GetComponent<Button>() is Button;
			/* Button btn = currentGo.GetComponent<Button>(); */
			// Marque l'objet comme "selectionné" :
			if (!m_EventSystem.alreadySelecting && tmp)
			{
				// Marque l'objet comme "selectionné" :
				m_EventSystem.SetSelectedGameObject(null);
				m_EventSystem.SetSelectedGameObject(currentGo, eventData); // GameObject à la place de currenGo pour trigger onselect
				Debug.Log("Selections : " + m_EventSystem.currentSelectedGameObject);
				clickEnter = true;
				OnSelect(null); // Active OnSelect
			}
			else currentGo = null;
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		clickEnter = false;
		Debug.Log("EXIT");
		OnDeselect(null); // Active OnDeselect
	/* 		HighlightExit(name);
		s_pbool = false; */
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////////////


	////////////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////// A AMELIORER //////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////////////
	public void onPress(string name)
	{
		bool hasText = !GameObject.Find(name).GetComponent<Toggle>() && GameObject.Find(name).GetComponentInChildren<Text>(); //pour les GO qui ont du texte, sans les toggle
		_tmpBool = StrCompare(name, "Btn Jouer") || StrCompare(name, "Btn Statistiques");
		bool tmp = (!GetState() && !_tmpBool) || GetState();
		if (tmp)
		{
			if (hasText)
			{
				_btnText = GameObject.Find(name).GetComponentInChildren<Text>();
				MethodeCall(name);
				if (HasMenuChanged())
				{
					 currentGo = null; // Deselectionne
					_btnText.fontSize -= 3;
					_btnText.color = _previousColor;
					SetMenuChanged(false);
				}
				else
					TryColorText(_btnText, Color.blue, "#1e90ff");
			}

			GameObject.Find("SoundController").GetComponent<AudioSource>().Play();
		}
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////////////




	// PARTIE CLAVIER : (avec if(Keyboard.current.anyKey.wasPressedThisFrame))
	////////////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////// A REFAIRE ////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////////////
	public void SetCursorVisible(bool b)
	{
		if (b == false)
			Cursor.lockState = CursorLockMode.Locked;
		else
			Cursor.lockState = CursorLockMode.None;
		Cursor.visible = b;
	}

	public void DirectionKey(char direction)
	{
		if (s_ibool)
		{
			var foundObjects = FindObjectsOfType<Button>();
			Debug.Log(foundObjects[s_i].name + " : " + s_i);
			if (s_pbool)
			{ // A optimiser
				HighlightExit(_pname);
				SetCursorVisible(false);
			}

			if (s_hbool)
			{
				HighlightExit(foundObjects[s_i].name);
			}

			if (direction == '+')
			{
				if (s_i == foundObjects.Length - 1)
				{
					s_i = 0;
				}
				else if (s_i < foundObjects.Length - 1 && s_i >= 0)
				{
					s_i++;
				}
			}
			else if (direction == '-')
			{
				if (s_i == 0)
				{
					s_i = foundObjects.Length - 1;
				}
				else if (s_i <= foundObjects.Length - 1 && s_i > 0)
				{
					s_i--;
				}
			}

			HighlightEnter(foundObjects[s_i].name);
			s_hbool = true;
			Debug.Log(foundObjects[s_i].name + " : " + s_i);
			s_ibool = false;
			_hname = name;
			s_keyboardOn = true;
		}
	}

	void OnGUI()
	{
	/* 		if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.RightArrow))
		{ // haut/droit
			DirectionKey('+');
		}

		if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
		{ // bas/gauche
			DirectionKey('-');
		}

		if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
		{ // Entree
			Debug.Log("methodeCall : " + _hname);
			MethodeCall(_hname);
		}

		if ((Input.GetAxis("Mouse X") != 0) && s_keyboardOn) // if mouse moves
		{ 
			HighlightExit(_hname); // A optimiser
			SetCursorVisible(true);
			s_keyboardOn = false;
		// + engrenage
		}

		bool boolKeyUp = Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Escape);
		if (boolKeyUp)
			s_ibool = true; */
	// A preserver : 	
	/* 		direction = new Vector3(0, 1, 0); // up
		btnMain = (Button)FindObjectOfType(typeof(Button));
        Selectable newSelectable = btnMain.FindSelectable(direction);
        Debug.Log(newSelectable.name);  */
	}
/* 	public void OnPointerEnter(PointerEventData eventData)
	{
		HighlightEnter(name);
		s_pbool = true;
		_pname = name;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		HighlightExit(name);
		s_pbool = false;
	} */
////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////// A REFAIRE ////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////////////////////
}