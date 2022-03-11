using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Reflection;

public class IOManager : Miscellaneous, IPointerEnterHandler//, IPointerExitHandler
{
	private OptionsMenu _option;
	private AccountMenu _acc;
	private HomeMenu _home;
	private ConnectionMenu _co;
	private CreditsMenu _cred;
	private StatistiquesMenu _stat;
	private RoomSelectionMenu _sroom;
	private GameObject currentGo;
	private Color _previousColor;
	private Text _btnText;
	static private GameObject previousGo;
	private Texture2D _cursorTexture;
	private CursorMode _cursorMode = CursorMode.Auto;
	private Vector2 _cursorHotspot = Vector2.zero;
	private EventSystem eventSystem;

	//private string previousMenu;
	private string goToMenu;

	public GameObject ActualGo;
	static bool changement = false;

	void Start()
	{
		//Fetch the current EventSystem. Make sure your Scene has one.
		eventSystem = EventSystem.current;
		// SCRIPT :
		_option = gameObject.AddComponent(typeof(OptionsMenu)) as OptionsMenu;
		_acc = gameObject.AddComponent(typeof(AccountMenu)) as AccountMenu;
		_home = gameObject.AddComponent(typeof(HomeMenu)) as HomeMenu;
		_co = gameObject.AddComponent(typeof(ConnectionMenu)) as ConnectionMenu;
		_cred = gameObject.AddComponent(typeof(CreditsMenu)) as CreditsMenu;
		_stat = gameObject.AddComponent(typeof(StatistiquesMenu)) as StatistiquesMenu;
		_sroom = gameObject.AddComponent(typeof(RoomSelectionMenu)) as RoomSelectionMenu;
		// Cursor Texture :
		_cursorTexture = Resources.Load("basic_01 BLUE") as Texture2D;
		Cursor.SetCursor(_cursorTexture, _cursorHotspot, _cursorMode);

		////////////////////////////////////////////////////////////////////////////////////////////////////////
		///////////////////////////////////////////// FONCTIONNEL //////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////////////////////////////////////

		// Cherche chaque menu -> liste chaque boutons par menu -> assignation de la fonction respectivement
		foreach (Transform menu in GameObject.Find("SubMenus").transform)
			foreach (Transform child in menu.Find("Buttons").transform) // RESOUDRE CE PROBLEME 
			{
				child.GetComponent<Button>().onClick.AddListener(() => MethodCall(child.name));
			}

		//selection de base lors du start : firstSelect, ce bouton sera bleu et selectionné
		//pour l'instant ShowOptions mais à modifier
		currentGo = GameObject.Find("ShowOptions");
		previousGo = currentGo;
		eventSystem.SetSelectedGameObject(currentGo);
		_btnText = eventSystem.currentSelectedGameObject.GetComponentInChildren<Text>();
		_previousColor = _btnText.color;
		TryColorText(_btnText, Color.blue, "#1e90ff");
		_btnText.fontSize += 3;
		////////////////////////////////////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////////////////////////////////////////
	}


	public void ColorButtonSelected()
    {
		_btnText = currentGo.GetComponentInChildren<Text>();
		TryColorText(_btnText, Color.blue, "#1e90ff");
		_btnText.fontSize += 3;
	}

	public void ColorButtonDeselected()
    {
		_btnText = previousGo.GetComponentInChildren<Text>();
		TryColorText(_btnText, Color.blue, "#ffffff");
		_btnText.fontSize -= 3;
	}


	public void Update()
	{
		//si on appuie sur une touche de deplacement
		if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
        {
			couleur_touches();
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (eventData.pointerCurrentRaycast.gameObject.transform.parent.gameObject.GetComponent<Button>() 
		 || eventData.pointerCurrentRaycast.gameObject.transform.parent.gameObject.GetComponent<Toggle>())
        {
			//si on est sur un bouton different de celui selectionne, alors on le selectionne et celui ci devient bleu
			if (eventData.pointerCurrentRaycast.gameObject.transform.parent.gameObject != eventSystem.currentSelectedGameObject)
			{
				//le bouton sera celui pointe par la souris
				currentGo = eventData.pointerCurrentRaycast.gameObject.transform.parent.gameObject;

				//on le selectionne
				if (currentGo.GetComponent<Button>().interactable)
					eventSystem.SetSelectedGameObject(currentGo);

				//si les conditions le permettent, on selectionne ce bouton
				if (currentGo.GetComponent<Button>().interactable)
				{
					ColorButtonSelected();
				}
				//on deselectionne l'ancien
				if (previousGo != currentGo)
				{
					if (currentGo.GetComponent<Button>().interactable)
					{
						ColorButtonDeselected();
						previousGo = currentGo;
					}
				}
			}
		}
	}

	private void couleur_touches()
	{
		//le go actuel est celui qui est selectionne
		if (currentGo != eventSystem.currentSelectedGameObject)
		{
			currentGo = eventSystem.currentSelectedGameObject;
			//bouton en bleu
			if (currentGo.GetComponent<Button>())
			{
				if (currentGo.GetComponent<Button>().interactable && currentGo.GetComponentInChildren<Text>())
				{
					ColorButtonSelected();
				}
			}
			//on deselectionne l'ancien et il devient blanc
			if (previousGo != currentGo)
			{
				//double condition pour eviter qu'avec les touches du clavier on aille sur un scroller, inputfield... tout ce qui n'est pas un bouton ou toggle et il faut que le bouton ai un texte
				if (currentGo.GetComponent<Button>())
				{
					if (currentGo.GetComponent<Button>().interactable && currentGo.GetComponentInChildren<Text>())
					{
						ColorButtonDeselected();
						previousGo = currentGo;
					}
				}
			}
		}
	}
	// PARTIE COMMUNE : 
	////////////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////// FONCTIONNEL //////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////////////
	public void MethodCall(string methode)
	{ 
		if (GetCurrentMenu().name == "HomeMenu")
		{
			_home.Invoke(methode, 0);
			changement = false;
			if (methode == "ShowConnection")
			{
				goToMenu = "ConnectionMenu";
				//previousMenu = "HomeMenu";
				changement = true;
			}
			if (methode == "ShowOptions")
			{
				goToMenu = "OptionsMenu";
				//previousMenu = "HomeMenu";
				changement = true;
			}
			if (methode == "ShowStat")
			{
				goToMenu = "StatMenu";
				//previousMenu = "HomeMenu";
				changement = true;
			}
			if (methode == "ShowRoomSelection")
			{
				goToMenu = "RoomSelectionMenu";
				//previousMenu = "HomeMenu";
				changement = true;
			}
	}
		if (GetCurrentMenu().name == "StatMenu")
		{
			_stat.Invoke(methode, 0);
			changement = false;
			if (methode == "HideStat")
			{
				goToMenu = "HomeMenu";
				//previousMenu = "StatMenu";
				changement = true;
			}
		}
		if (GetCurrentMenu().name == "OptionsMenu")
		{
			_option.Invoke(methode, 0);
			changement = false;
			if (methode == "HideOptions")
			{
				goToMenu = "HomeMenu";
				//previousMenu = "OptionsMenu";
				changement = true;
			}
			if (methode == "ShowCredits")
			{
				goToMenu = "CreditsMenu";
				//previousMenu = "OptionsMenu";
				changement = true;
			}
		}
		if (GetCurrentMenu().name == "CreditsMenu")
		{
			_cred.Invoke(methode, 0);
			changement = false;
			if (methode == "HideCredits")
			{
				goToMenu = "OptionsMenu";
				//previousMenu = "CreditsMenu";
				changement = true;
			}
		}
		if (GetCurrentMenu().name == "ConnectionMenu")
		{
			_co.Invoke(methode, 0);
			changement = false;
			if (methode == "HideConnection" || methode == "Connect")
			{
				goToMenu = "HomeMenu";
				//previousMenu = "ConnectionMenu";
				changement = true;
			}
			if (methode == "ShowAccount")
			{
				goToMenu = "AccountMenu";
				//previousMenu = "ConnectionMenu";
				changement = true;
			}
		}
		if (GetCurrentMenu().name == "AccountMenu" || methode == "CreateAccount")
		{
			_acc.Invoke(methode, 0);
			changement = false;
			if (methode == "HideAccount")
			{
				goToMenu = "ConnectionMenu";
				//previousMenu = "AccountMenu";
				changement = true;
			}
		}
		if (GetCurrentMenu().name == "RoomSelectionMenu")
		{
			_sroom.Invoke(methode, 0);
			changement = false;
			if (methode == "HideRoomSelection")
			{
				goToMenu = "HomeMenu";
				//previousMenu = "RoomSelectionMenu";
				changement = true;
			}
		}

		//lors d'un changement de menu (bouton qui change de menu, click ou Entrée
		if (changement == true)
		{
			//on selectionne le premier bouton enfant du menu dans lequel on va
			eventSystem.SetSelectedGameObject(FindGOTool(goToMenu, "Buttons").transform.GetChild(0).gameObject);
			currentGo = eventSystem.currentSelectedGameObject;

			//comme avant pour les couleurs
			//condition qui evite de mettre en couleur un bouton non selectionnable
			if (currentGo.GetComponent<Button>().interactable)
			{
				ColorButtonSelected();
				changement = false;
				if (previousGo != currentGo)
				{
					ColorButtonDeselected();
					previousGo = currentGo;
				}
			}
		}
		GameObject.Find("SoundController").GetComponent<AudioSource>().Play();
	}
}
