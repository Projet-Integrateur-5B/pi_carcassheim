using UnityEngine.InputSystem;
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
	private GameObject nextGo;
	private Color _previousColor, colHover;
	private Text _btnText;
	static private GameObject previousGo;
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
		changeHover();
		// Cherche chaque menu -> liste chaque boutons par menu -> assignation de la fonction respectivement
		foreach (Transform menu in GameObject.Find("SubMenus").transform)
		{
			foreach (Transform btn in menu.Find("Buttons").transform)
				if (btn.GetComponent<Button>())
					btn.GetComponent<Button>().onClick.AddListener(delegate
					{
						MethodCall(btn.name);
					});
			if (menu.Find("Toggle Group"))
				foreach (Transform tog in menu.Find("Toggle Group").transform.GetChild(0).transform)
					if (tog.GetComponent<Toggle>())
						tog.GetComponent<Toggle>().onValueChanged.AddListener(delegate
						{
							MethodCallT(menu.Find("Toggle Group").transform.GetChild(0).name, tog.GetComponent<Toggle>());
						});
		}
	}

	public void Update() // A VERIFIER
	{   //si on appuie sur une touche de deplacement
/* 		if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
			if (nextGo != eventSystem.currentSelectedGameObject)
				selectionChange(); */
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		nextGo = eventData.pointerCurrentRaycast.gameObject.transform.parent.gameObject;
		selectionChange(); // checkmark raycast toggle/button doit être activé 
	}

	public void selectionChange()
	{
		// nextGo.GetComponent<Button>() est testé d'abord donc si false la parti gauche du ET non testé donc pas d'erreur
		bool btn = nextGo.GetComponent<Button>() && nextGo.GetComponent<Button>().interactable;
		if (nextGo != eventSystem.currentSelectedGameObject && (btn || nextGo.GetComponent<Toggle>()))
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

	public void colorImage(GameObject go, float f)
	{ // transparence 
		Image image = go.transform.GetChild(0).gameObject.GetComponent<Image>();
		image.color = new Color(image.color.r, image.color.g, image.color.b, f);
	}

	public void changeHover()
	{
		if (boolSelectionChange == true)
		{
			if (previousGo != null)
			{
				Component previousTarget = previousGo.transform.GetChild(0).GetComponent<Component>();
				switch (previousTarget.name)
				{ // previousGO
					case "RawImage": // GIF
						// A changer (dezoom)
						previousGo.GetComponentInChildren<RawImage>().rectTransform.sizeDelta = new Vector2(50, 50);
						break;
					case "Unselected": // IMAGE
						colorImage(previousGo, 1);
						break;
					case "Text": // BOUTON
						textColor(Color.white, -3, previousGo);
						break;
					case "Background": // TOGGLE
						colorImage(previousGo, 1);
						break;
					default:
						break;
				}
			}

			Component nextTarget = nextGo.transform.GetChild(0).GetComponent<Component>();
			switch (nextTarget.name)
			{ // nextGO
				case "RawImage": // GIF
					// A changer (zoom)
					nextGo.GetComponentInChildren<RawImage>().rectTransform.sizeDelta = new Vector2(70, 70);
					break;
				case "Unselected": // IMAGE
					colorImage(nextGo, 0);
					break;
				case "Text": // BOUTON
					textColor(Color.blue, 3, nextGo);
					break;
				case "Background": // TOGGLE
					colorImage(nextGo, 0.5f); // semi transparent (à changer)
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

	public void MethodCall(string methode)
	{
		GameObject.Find("SoundController").GetComponent<AudioSource>().Play();
		gameObject.SendMessage(methode, null);
		NewMenuSelectButton();
	}

	public void MethodCallT(string methode, Toggle tog)
	{
		GameObject.Find("SoundController").GetComponent<AudioSource>().Play();
		gameObject.SendMessage(methode, tog);
	}
}