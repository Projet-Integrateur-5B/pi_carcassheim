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
	private GameObject currentGo;
	private Color _previousColor, colHover;
	private Text _btnText;
	static private GameObject previousGo;
	private Texture2D _cursorTexture;
	private CursorMode _cursorMode = CursorMode.Auto;
	private Vector2 _cursorHotspot = Vector2.zero;
	private EventSystem eventSystem;
	void Start()
	{
		ColorUtility.TryParseHtmlString("#1e90ff", out colHover);
		// SCRIPT : (nécessaire pour SendMessage) => chercher un moyen de l'enlever.
		// ---------------------------------- PATCH : ------------------------------------
		// à ameliorer :
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
		//Fetch the current EventSystem. Make sure your Scene has one.
		eventSystem = EventSystem.current;
		// Cursor Texture :
		_cursorTexture = Resources.Load("Miscellaneous/basic_01 BLUE") as Texture2D;
		Cursor.SetCursor(_cursorTexture, _cursorHotspot, _cursorMode);
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

		//selection de base lors du start : firstSelect, ce bouton sera bleu et selectionné
		//pour l'instant ShowOptions mais à modifier
		currentGo = FirstActiveChild(GameObject.Find("Buttons"));
		previousGo = currentGo;
		eventSystem.SetSelectedGameObject(currentGo);
		_btnText = eventSystem.currentSelectedGameObject.GetComponentInChildren<Text>();
		_previousColor = _btnText.color;
		_btnText.color = colHover;
		_btnText.fontSize += 3;
	}

	public void Update()
	{
		//si on appuie sur une touche de deplacement
		if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
			Couleur_touches();
	}

	public void ColorButtonSelected()
	{
		_btnText = currentGo.GetComponentInChildren<Text>();
		_btnText.color = colHover;
		_btnText.fontSize += 3;
	}

	public void ColorButtonDeselected()
	{
		_btnText = previousGo.GetComponentInChildren<Text>();
		if (_btnText.transform.parent.name == "CGU" || _btnText.transform.parent.name == "ForgottenPwdUser")
			_btnText.color = Color.green;
		else
			_btnText.color = Color.white;
		_btnText.fontSize -= 3;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		currentGo = eventData.pointerCurrentRaycast.gameObject.transform.parent.gameObject;
		//si on est sur un bouton different de celui selectionne, alors on le selectionne et celui ci devient bleu
		if (currentGo != eventSystem.currentSelectedGameObject)
			if ((currentGo.GetComponent<Button>() && currentGo.GetComponent<Button>().interactable) || currentGo.transform.parent.gameObject.GetComponent<Toggle>())
			{
				if (currentGo.transform.parent.gameObject.GetComponent<Toggle>())
					currentGo = currentGo.transform.parent.gameObject;
				eventSystem.SetSelectedGameObject(currentGo);
				SelectionChange();
			}
	}

	private void SelectionChange()
	{
		currentGo = eventSystem.currentSelectedGameObject;
		ColorBlock cb;
		if (!currentGo.GetComponent<InputField>())
		{
			if (currentGo.GetComponentInChildren<Text>())
				ColorButtonSelected();
			else if (currentGo.GetComponent<Image>())
				currentGo.GetComponent<Image>().color = colHover;
			else if (currentGo.GetComponent<Toggle>())
			{
				cb = currentGo.GetComponent<Toggle>().colors;
				cb.selectedColor = colHover;
				currentGo.GetComponent<Toggle>().colors = cb;
			}

			if (previousGo != currentGo)
			{
				if (previousGo.GetComponentInChildren<Text>())
					ColorButtonDeselected();
				else if (previousGo.GetComponent<Image>())
					previousGo.GetComponent<Image>().color = Color.white;
				else if (previousGo.GetComponent<Toggle>())
				{
					cb = previousGo.GetComponent<Toggle>().colors;
					cb.selectedColor = Color.white;
					previousGo.GetComponent<Toggle>().colors = cb;
				}

				previousGo = currentGo;
			}
		}
	}

	private void Couleur_touches()
	{
		if (currentGo != eventSystem.currentSelectedGameObject)
			SelectionChange();
	}

	public void SelectionButton()
	{
		if (HasMenuChanged() == true)
		{
			string previousMenu = GetPreviousMenu().name.Substring(0, GetPreviousMenu().name.Length - 4);
			foreach (Transform child in GameObject.Find("Buttons").transform)
			{
				eventSystem.SetSelectedGameObject(FirstActiveChild(GameObject.Find("Buttons")));
				if (child.name.Contains(previousMenu) && child.gameObject.activeSelf)
				{
					eventSystem.SetSelectedGameObject(child.gameObject);
					break;
				}
			}

			SetMenuChanged(false);
			SelectionChange();
		}
	}

	/* Broadcast message will call the passed in function for all the scripts on the game object 
   that have that function and for all children objects that also have that function. 
   Sendmessage only calls that function for scripts on the gameobject that have that function. */
	public void MethodCall(string methode)
	{
		GameObject.Find("SoundController").GetComponent<AudioSource>().Play();
		gameObject.SendMessage(methode, null);
		SelectionButton();
	}

	public void MethodCallT(string methode, Toggle tog)
	{
		GameObject.Find("SoundController").GetComponent<AudioSource>().Play();
		gameObject.SendMessage(methode, tog);
	}
}