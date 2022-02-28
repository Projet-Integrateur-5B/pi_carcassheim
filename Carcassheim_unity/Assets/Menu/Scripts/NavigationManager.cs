using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// include fonctions du script via la classe HomeMenu incluant elle meme (ConnectionMenu + Miscellaneous + Monobehaviour)
public class NavigationManager : Miscellaneous, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
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
	private string _pname;
	private string _hname;
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
	}

	// Update is called once per frame
	void Update()
	{
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		bool hasText = !GameObject.Find(name).GetComponent<Toggle>() && GameObject.Find(name).GetComponentInChildren<Text>(); //pour les GO qui ont du texte, sans les toggle
		_tmpBool = StrCompare(name, "Btn Jouer") || StrCompare(name, "Btn Statistiques");
		bool tmp = (!GetState() && !_tmpBool) || GetState();
		if (tmp)
		{
			if (hasText)
			{
				_btnText = GameObject.Find(name).GetComponentInChildren<Text>();
			}

			MethodeCall(name);
			if (hasText)
			{
				if (HasMenuChanged())
				{
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

	public void SetCursorVisible(bool b)
	{
		if (b == false)
			Cursor.lockState = CursorLockMode.Locked;
		else
			Cursor.lockState = CursorLockMode.None;
		Cursor.visible = b;
	}

	public void ForwardKey()
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

			if (s_i == foundObjects.Length - 1)
			{
				s_i = 0;
			}
			else if (s_i < foundObjects.Length - 1 && s_i >= 0)
			{
				s_i++;
			}

			HighlightEnter(foundObjects[s_i].name);
			s_hbool = true;
			Debug.Log(foundObjects[s_i].name + " : " + s_i);
			s_ibool = false;
			_hname = name;
		}
	}

	public void BackwardKey()
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

			if (s_i == 0)
			{
				s_i = foundObjects.Length - 1;
			}
			else if (s_i <= foundObjects.Length - 1 && s_i > 0)
			{
				s_i--;
			}

			HighlightEnter(foundObjects[s_i].name);
			Debug.Log(foundObjects[s_i].name + " : " + s_i);
			s_ibool = false;
			_hname = name;
		}
	}

	public void EnterKeyboard()
	{
		Debug.Log("methodeCall : " + _hname);
		MethodeCall(_hname);
	}

	void OnGUI()
	{
		if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.RightArrow))
		{ // haut/droit
			ForwardKey();
		}

		if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
		{ // bas/gauche
			BackwardKey();
		}

		if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
		{ // Entree
			EnterKeyboard();
		}

		if (Input.GetKeyDown(KeyCode.Escape))
		{ // Echape
			HighlightExit(_hname); // A optimiser
			SetCursorVisible(true);
		// + engrenage
		}

		bool boolKeyUp = Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Escape);
		if (boolKeyUp)
			s_ibool = true;
	// A preserver : 	
	/* 		direction = new Vector3(0, 1, 0); // up
		btnMain = (Button)FindObjectOfType(typeof(Button));
        Selectable newSelectable = btnMain.FindSelectable(direction);
        Debug.Log(newSelectable.name);  */
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		HighlightEnter(name);
		s_pbool = true;
		_pname = name;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		HighlightExit(name);
		s_pbool = false;
	}

	public void MethodeCall(string name)
	{
		switch (name)
		{
			// HomeMenu :
			case "Btn Connexion":
				_home.ShowConnection();
				break;
			case "Btn Jouer":
				if (GetState())
					_home.Jouer();
				break;
			case "Btn Statistiques":
				if (GetState())
					_home.ShowStatistiques();
				break;
			case "Btn Options":
				_home.ShowOptions();
				break;
			case "Btn Quitter le jeu":
				_home.Quitter();
				break;
			// StatistiquesMenu : 
			case "Btn Retour Stat":
				_stat.HideStatistiques();
				break;
			// OptionsMenu :
			case "Btn Retour Opt":
				_option.HideOptions();
				break;
			case "Btn Son":
				_option.SwitchSound();
				break;
			case "Toggle French":
			case "Toggle English":
			case "Toggle German":
				_option.FlagsToggle();
				break;
			case "Btn Musique":
				_option.SwitchMusic();
				break;
			case "Btn Fenêtré":
				_option.FullScreen();
				break;
			case "Btn Aide":
				_option.Help();
				break;
			case "Btn Credits":
				_option.ShowCredits();
				break;
			// CreditsMenu :
			case "Btn Retour Credits":
				_cred.HideCredits();
				break;
			// ConnectionMenu :
			case "Btn Retour Co":
				_co.HideConnection();
				break;
			case "Btn ForgottenPwdUser":
				_co.ForgottenPwdUser();
				break;
			case "Btn Se Connecter":
				_co.Connect();
				break;
			case "Btn Creer un compte":
				_co.CreateAccount();
				break;
			case "Toggle AfficherMdp":
				_co.HideShowPwd();
				break;
			// AccountMenu
			case "Btn Retour Crea CA":
				_acc.HideAccount();
				break;
			case "Btn Creer votre compte":
				_acc.CreateAccountConnected();
				break;
			case "Toggle AfficherMdp CA":
				_acc.HideShowPwdConf();
				break;
			// RoomSelectionMenu
			case "Btn Retour RoomSelection":
				_sroom.HideRoomSelection();
				break;
			case "Btn Options Pop-Up":
				_sroom.ShowPopUpOptions();
				break;
			default:
				return;
		}
	}
}