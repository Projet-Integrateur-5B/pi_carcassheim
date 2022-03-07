using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CreditsMenu : Miscellaneous, IPointerEnterHandler, IPointerExitHandler
{
	public Button button1;

	private GameObject currentGo;
	private Color _previousColor;
	private Text _btnText;

	EventSystem eventSystem;

	void Start()
	{
		button1.Select();
	}

	void Update()
	{
		Debug.Log(eventSystem.currentSelectedGameObject.name);
	}


	void OnEnable()
	{
		//Fetch the current EventSystem. Make sure your Scene has one.
		eventSystem = EventSystem.current;
		//Register Button Events
		button1 = GameObject.Find("Btn Retour Credits").GetComponent<Button>();
		button1.onClick.AddListener(() => buttonCallBack(button1));
		button1.Select();
	}

	private void buttonCallBack(Button buttonPressed)
	{
		if (buttonPressed == button1)
		{
			Debug.Log("Clicked: " + button1.name);
			HideCredits();
		}

		if (currentGo != null)
		{
			if (currentGo.GetComponent<Button>())
			{
				_btnText.color = _previousColor;
				_btnText.fontSize -= 3;
			}
		}
		GameObject.Find("SoundController").GetComponent<AudioSource>().Play();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		currentGo = eventData.pointerCurrentRaycast.gameObject.transform.parent.gameObject;
		Debug.Log("Mouse Enter " + currentGo.name);
		if (currentGo.GetComponent<Button>())
		{
			_btnText = currentGo.GetComponentInChildren<Text>();
			_previousColor = _btnText.color;
			TryColorText(_btnText, Color.blue, "#1e90ff");
			_btnText.fontSize += 3;
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		Debug.Log("Mouse Exit " + currentGo.name);
		if (currentGo.GetComponent<Button>())
		{
			_btnText.color = _previousColor;
			_btnText.fontSize -= 3;
		}
	}

	void OnDisable()
	{
		//Un-Register Button Events
		button1.onClick.RemoveAllListeners();
	}

	public void HideCredits()
	{
		ChangeMenu(FindMenu("CreditsMenu"), FindMenu("OptionsMenu"));
	}
}