using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Reflection;

public class IOManager : Miscellaneous , IPointerEnterHandler, IPointerExitHandler
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
	static private GameObject selectedGo;
	static private GameObject previousGo;
	private Texture2D _cursorTexture;
	private CursorMode _cursorMode = CursorMode.Auto;
	private Vector2 _cursorHotspot = Vector2.zero;
	private EventSystem eventSystem;
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
			print(child.gameObject.name); 
			}

        ////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////


	// PATCH (clavier à faire): 
	/*         Button button5 = GameObject.Find("Btn Quitter le jeu").GetComponent<Button>();
        button5.Select(); */
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		currentGo = eventData.pointerCurrentRaycast.gameObject.transform.parent.gameObject;
		/* Debug.Log("Mouse Enter " + currentGo.name); */
		if (currentGo.GetComponent<Button>())
		{
			_btnText = currentGo.GetComponentInChildren<Text>();
			if (!GetState() && (StrCompare(currentGo.name, "Btn Jouer") || StrCompare(currentGo.name, "Btn Statistiques")))
			{
				TryColorText(_btnText, Color.grey, "#808080");
			}
			else
			{
				_previousColor = _btnText.color;
				TryColorText(_btnText, Color.blue, "#1e90ff");
				_btnText.fontSize += 3;
			}
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		/* Debug.Log("Mouse Exit " + currentGo.name); */
		bool tmpBool = StrCompare(currentGo.name, "Btn Jouer") || StrCompare(currentGo.name, "Btn Statistiques");
		if ((GetState() || !tmpBool) && currentGo.GetComponent<Button>())
		{
			_btnText.color = _previousColor;
			_btnText.fontSize -= 3;
		}
	}

	// PATCH (clavier à faire):
	/*     private void couleur_touches()
    {
    	 if (eventSystem.currentSelectedGameObject != selectedGo) 
    	 {
             previousGo = selectedGo;
             selectedGo = eventSystem.currentSelectedGameObject;
             changement = true; //on a changé de bouton
         }
        Debug.Log("Ancien : " + previousGo + " actuel : " + selectedGo);
        if (changement == true && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)))
        {
            if (selectedGo.GetComponent<Button>())
	        {
	            if (!GetState() && (StrCompare(selectedGo.name, "Btn Jouer") || StrCompare(selectedGo.name, "Btn Statistiques")))
	            {
	                TryColorText(selectedGo.GetComponentInChildren<Text>(), Color.grey, "#808080");
	            }
	            else
	            {
	                _previousColor = selectedGo.GetComponentInChildren<Text>().color;
	                TryColorText(selectedGo.GetComponentInChildren<Text>(), Color.blue, "#1e90ff");
	                selectedGo.GetComponentInChildren<Text>().fontSize += 3;
	            }
	        }

	        bool tmpBool = StrCompare(previousGo.name, "Btn Jouer") || StrCompare(previousGo.name, "Btn Statistiques");
	        if ((GetState() || !tmpBool) && previousGo.GetComponent<Button>())
	        {
	            previousGo.GetComponentInChildren<Text>().color = _previousColor;
	            previousGo.GetComponentInChildren<Text>().fontSize -= 3;
	        }
	        changement = false;
        }
    } */
	// PARTIE COMMUNE : 
	////////////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////// FONCTIONNEL //////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////////////
	public void MethodCall(string methode)
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
		if (GetCurrentMenu().name == "AccountMenu")
			_acc.Invoke(methode, 0);
		if (GetCurrentMenu().name == "RoomSelectionMenu")
			_sroom.Invoke(methode, 0);

        GameObject.Find("SoundController").GetComponent<AudioSource>().Play();
	}
}