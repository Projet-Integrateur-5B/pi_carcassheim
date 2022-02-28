using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// include fonctions du script via la classe HomeMenu incluant elle meme (ConnectionMenu + Miscellaneous + Monobehaviour)
public class NavigationManager : Miscellaneous, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
	private OptionsMenu option;
	private AccountMenu acc;
	private HomeMenu home;
	private ConnectionMenu co;
	private CreditsMenu cred;
	private StatistiquesMenu stat;
	private RoomSelectionMenu sroom;
	private Texture2D cursorTexture;
	private CursorMode cursorMode = CursorMode.Auto;
	private Vector2 cursorHotspot = Vector2.zero;
	private Color previousColor;
	private Text btnText;
	private bool tmpBool;
	private static int i = 0;
	public static bool ibool = true;
	public static bool hbool = false;
	public static bool pbool = false;
	private string pname;
	private string hname;
	// Start is called before the first frame update
	void Start()
	{
		// Cursor Texture :
		cursorTexture = Resources.Load("basic_01 BLUE") as Texture2D;
		Cursor.SetCursor(cursorTexture, cursorHotspot, cursorMode);
		// SCRIPT :
		option = gameObject.AddComponent(typeof(OptionsMenu)) as OptionsMenu;
		acc = gameObject.AddComponent(typeof(AccountMenu)) as AccountMenu;
		home = gameObject.AddComponent(typeof(HomeMenu)) as HomeMenu;
		co = gameObject.AddComponent(typeof(ConnectionMenu)) as ConnectionMenu;
		cred = gameObject.AddComponent(typeof(CreditsMenu)) as CreditsMenu;
		stat = gameObject.AddComponent(typeof(StatistiquesMenu)) as StatistiquesMenu;
		sroom = gameObject.AddComponent(typeof(RoomSelectionMenu)) as RoomSelectionMenu;
	}

	// Update is called once per frame
	void Update()
	{
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		bool hasText = !GameObject.Find(name).GetComponent<Toggle>() && GameObject.Find(name).GetComponentInChildren<Text>(); //pour les GO qui ont du texte, sans les toggle
		Debug.Log(hasText);
		tmpBool = StrCompare(name, "Btn Jouer") || StrCompare(name, "Btn Statistiques");
		bool tmp = (!GetState() && !tmpBool) || GetState();
		if (tmp)
		{
			if (hasText)
			{
				btnText = GameObject.Find(name).GetComponentInChildren<Text>();
			}

			MethodeCall(name);
			if (hasText)
			{
				if (HasMenuChanged())
				{
					btnText.fontSize -= 3;
					btnText.color = previousColor;
					SetMenuChanged(false);
				}
				else
					TryColorText(btnText, Color.blue, "#1e90ff");
			}

			GameObject.Find("SoundController").GetComponent<AudioSource>().Play();
		}
	}

	public void HighlightEnter(string name)
	{
		if (!GameObject.Find(name).GetComponent<Toggle>() && GameObject.Find(name).GetComponentInChildren<Text>())
		{
			btnText = GameObject.Find(name).GetComponentInChildren<Text>();
			tmpBool = StrCompare(name, "Btn Jouer") || StrCompare(name, "Btn Statistiques");
			if (!GetState() && tmpBool)
				TryColorText(btnText, Color.grey, "#808080");
			else
			{
				previousColor = btnText.color;
				TryColorText(btnText, Color.blue, "#1e90ff");
				btnText.fontSize += 3;
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
				btnText.color = previousColor;
				btnText.fontSize -= 3;
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
		if (ibool)
		{
			var foundObjects = FindObjectsOfType<Button>();
			Debug.Log(foundObjects[i].name + " : " + i);
			if (pbool)
			{ // A optimiser
				HighlightExit(pname);
				SetCursorVisible(false);
			}

			if (hbool)
			{
				HighlightExit(foundObjects[i].name);
			}

			if (i == foundObjects.Length - 1)
			{
				i = 0;
			}
			else if (i < foundObjects.Length - 1 && i >= 0)
			{
				i++;
			}

			HighlightEnter(foundObjects[i].name);
			hbool = true;
			Debug.Log(foundObjects[i].name + " : " + i);
			ibool = false;
			hname = name;
		}
	}

	public void BackwardKey()
	{
		if (ibool)
		{
			var foundObjects = FindObjectsOfType<Button>();
			Debug.Log(foundObjects[i].name + " : " + i);
			if (pbool)
			{ // A optimiser
				HighlightExit(pname);
				SetCursorVisible(false);
			}

			if (hbool)
			{
				HighlightExit(foundObjects[i].name);
			}

			if (i == 0)
			{
				i = foundObjects.Length - 1;
			}
			else if (i <= foundObjects.Length - 1 && i > 0)
			{
				i--;
			}

			HighlightEnter(foundObjects[i].name);
			Debug.Log(foundObjects[i].name + " : " + i);
			ibool = false;
			hname = name;
		}
	}

	public void EnterKeyboard()
	{
		Debug.Log("methodeCall : " + hname);
		MethodeCall(hname);
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
			HighlightExit(hname); // A optimiser
			SetCursorVisible(true);
		// + engrenage
		}

		bool boolKeyUp = Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Escape);
		if (boolKeyUp)
			ibool = true;
	// A preserver : 	
	/* 		direction = new Vector3(0, 1, 0); // up
		btnMain = (Button)FindObjectOfType(typeof(Button));
        Selectable newSelectable = btnMain.FindSelectable(direction);
        Debug.Log(newSelectable.name);  */
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		HighlightEnter(name);
		pbool = true;
		pname = name;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		HighlightExit(name);
		pbool = false;
	}

	public void MethodeCall(string name)
	{
		switch (name)
		{
			// HomeMenu :
			case "Btn Connexion":
				home.ShowConnection();
				break;
			case "Btn Jouer":
				if (GetState())
					home.Jouer();
				break;
			case "Btn Statistiques":
				if (GetState())
					home.ShowStatistiques();
				break;
			case "Btn Options":
				home.ShowOptions();
				break;
			case "Btn Quitter le jeu":
				home.Quitter();
				break;
			// StatistiquesMenu : 
			case "Btn Retour Stat":
				stat.HideStatistiques();
				break;
			// OptionsMenu :
			case "Btn Retour Opt":
				option.HideOptions();
				break;
			case "Btn Son":
				option.SwitchSound();
				break;
			case "Toggle French":
			case "Toggle English":
			case "Toggle German":
				option.FlagsToggle();
				break;
			case "Btn Musique":
				option.SwitchMusic();
				break;
			case "Btn Fenêtré":
				option.FullScreen();
				break;
			case "Btn Aide":
				option.Help();
				break;
			case "Btn Credits":
				option.ShowCredits();
				break;
			// CreditsMenu :
			case "Btn Retour Credits":
				cred.HideCredits();
				break;
			// ConnectionMenu :
			case "Btn Retour Co":
				co.HideConnection();
				break;
			case "Btn ForgottenPwdUser":
				co.ForgottenPwdUser();
				break;
			case "Btn Se Connecter":
				co.Connect();
				break;
			case "Btn Creer un compte":
				co.CreateAccount();
				break;
			case "Toggle AfficherMdp":
				co.HideShowPwd();
				break;
			// AccountMenu
			case "Btn Retour Crea CA":
				acc.HideAccount();
				break;
			case "Btn Creer votre compte":
				acc.CreateAccountConnected();
				break;
			case "Toggle AfficherMdp CA":
				acc.HideShowPwdConf();
				break;
			// RoomSelectionMenu
			case "Btn Retour RoomSelection":
				sroom.HideRoomSelection();
				break;
			case "Btn Options Pop-Up":
				sroom.ShowPopUpOptions();
				break;
			default:
				return;
		}
	}
}