using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class IOManager : Miscellaneous, IPointerEnterHandler //, IPointerExitHandler
{
	private OptionsMenu _option;
	private AccountMenu _acc;
	private HomeMenu _home;
	private ConnectionMenu _co;
	private CreditsMenu _cred;
	private StatistiquesMenu _stat;
	private RoomSelectionMenu _sroom;
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
		_option = gameObject.AddComponent(typeof(OptionsMenu)) as OptionsMenu;
		_acc = gameObject.AddComponent(typeof(AccountMenu)) as AccountMenu;
		_home = gameObject.AddComponent(typeof(HomeMenu)) as HomeMenu;
		_co = gameObject.AddComponent(typeof(ConnectionMenu)) as ConnectionMenu;
		_cred = gameObject.AddComponent(typeof(CreditsMenu)) as CreditsMenu;
		_stat = gameObject.AddComponent(typeof(StatistiquesMenu)) as StatistiquesMenu;
		_sroom = gameObject.AddComponent(typeof(RoomSelectionMenu)) as RoomSelectionMenu;
		//Fetch the current EventSystem. Make sure your Scene has one.
		eventSystem = EventSystem.current;
		// Cursor Texture :
		_cursorTexture = Resources.Load("basic_01 BLUE") as Texture2D;
		Cursor.SetCursor(_cursorTexture, _cursorHotspot, _cursorMode);
		// Cherche chaque menu -> liste chaque boutons par menu -> assignation de la fonction respectivement
		foreach (Transform menu in GameObject.Find("SubMenus").transform)
			foreach (Transform child in menu.Find("Buttons").transform)
				child.GetComponent<Button>().onClick.AddListener(() => MethodCall(child.name));
		//selection de base lors du start : firstSelect, ce bouton sera bleu et selectionné
		//pour l'instant ShowOptions mais à modifier
		currentGo = firstActiveChild(GameObject.Find("Buttons"));
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
			couleur_touches();
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
		_btnText.color = Color.white;
		_btnText.fontSize -= 3;
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (eventData.pointerCurrentRaycast.gameObject.transform.parent.gameObject.GetComponent<Button>() || eventData.pointerCurrentRaycast.gameObject.transform.parent.gameObject.GetComponent<Toggle>())
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
					ColorButtonSelected();
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
				if (currentGo.GetComponent<Button>().interactable && currentGo.GetComponentInChildren<Text>())
					ColorButtonSelected();
			//on deselectionne l'ancien et il devient blanc
			if (previousGo != currentGo)
				//double condition pour eviter qu'avec les touches du clavier on aille sur un scroller, inputfield... tout ce qui n'est pas un bouton ou toggle et il faut que le bouton ai un texte
				if (currentGo.GetComponent<Button>())
					if (currentGo.GetComponent<Button>().interactable && currentGo.GetComponentInChildren<Text>())
					{
						ColorButtonDeselected();
						previousGo = currentGo;
					}
		}
	}

	public void selectionButton()
	{
		if (HasMenuChanged() == true)
		{
			//on selectionne le premier bouton enfant du menu dans lequel on va
			eventSystem.SetSelectedGameObject(firstActiveChild(GameObject.Find("Buttons")));
			SetMenuChanged(false);
			currentGo = eventSystem.currentSelectedGameObject;
			//comme avant pour les couleurs
			//condition qui evite de mettre en couleur un bouton non selectionnable
			if (currentGo.GetComponent<Button>().interactable)
			{
				ColorButtonSelected();
				if (previousGo != currentGo)
				{
					ColorButtonDeselected();
					previousGo = currentGo;
				}
			}
		}
	}

	/* Broadcast message will call the passed in function for all the scripts on the game object 
   that have that function and for all children objects that also have that function. 
   Sendmessage only calls that function for scripts on the gameobject that have that function.
 */
	public void MethodCall(string methode)
	{
		GameObject.Find("SoundController").GetComponent<AudioSource>().Play();
		gameObject.SendMessage(methode, null);
		selectionButton();
	}
}