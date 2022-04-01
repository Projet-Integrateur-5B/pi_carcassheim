using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Collections;

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
						MethodCall(btn.name, null);
					});
			if (menu.Find("Toggle Group"))
				foreach (Transform tog in menu.Find("Toggle Group").transform.GetChild(0).transform)
					if (tog.GetComponent<Toggle>())
						tog.GetComponent<Toggle>().onValueChanged.AddListener(delegate
						{
							MethodCall(menu.Find("Toggle Group").transform.GetChild(0).name, tog.GetComponent<Toggle>());
						});
		}
	}

	public void Update()
	{
		if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
		{
			// Debug.Log(KeyCode);
			lockMouse(true);
			previousGo = nextGo;
			nextGo = eventSystem.currentSelectedGameObject;
			changeHover();
		}

		// ESCAPE fera pas disparaitre souris si on reutilise clavier car escape permet de sortir de la zone de jeu dans unity
		if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2) || Input.GetKey(KeyCode.Escape)) && Cursor.lockState == CursorLockMode.Locked /* && !Input.anyKeyDown */)
		{
			// Debug.Log("Left Mouse Button"); // A LAISSER
			lockMouse(false);
			nextGo = eventSystem.currentSelectedGameObject;
		}
	}

	private void lockMouse(bool b)
	{
		Cursor.lockState = b ? CursorLockMode.Locked : CursorLockMode.None;
		Cursor.visible = !b;
	}

	/* 	void OnGUI() // TROP LENT (a gardé pour détecter une touche quelconque)
    {
		if(Input.anyKeyDown &&  Event.current.isKey)
				switch(Event.current.keyCode.ToString()){
				case "UpArrow" : case "DownArrow" : case "LeftArrow" : case "RightArrow" :
				break;
				}
    } */
	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!eventData.pointerCurrentRaycast.gameObject.transform.parent.gameObject.GetComponent<InputField>())
		{
			nextGo = eventData.pointerCurrentRaycast.gameObject.transform.parent.gameObject;
			selectionChange();
		}
	}

	public void selectionChange()
	{
		// Aparté : (Les inpufield : le "Text" doit avoir du raycast pour fonctionner donc à ne pas désactiver)
		// nextGo.GetComponent<Button>() est testé d'abord donc si false la partie gauche du ET non testé donc pas d'erreur
		bool btn = nextGo.GetComponent<Button>() && nextGo.GetComponent<Button>().interactable;
		bool slider = nextGo.transform.GetChild(0).name == "Handle";
		//bool inputfd = nextGo.transform.parent.name == "InputField";
		//Debug.Log(nextGo.name); 
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
			TF.transform.position = curBtn.transform.position + new Vector3(width / 2 + 90, 0, 0);
			TB.transform.position = curBtn.transform.position - new Vector3(width / 2 + 20, 0, 0);
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

	public void MethodCall(string methode, Toggle tog)
	{
		GameObject.Find("SoundController").GetComponent<AudioSource>().Play();
		gameObject.SendMessage(methode, tog);
		if (tog == null)
			NewMenuSelectButton();
	}
}