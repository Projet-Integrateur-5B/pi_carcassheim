/*using System.Collections;
using System.Collections.Generic;*/
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ConnectionMenu : Miscellaneous, IPointerEnterHandler, IPointerExitHandler
{
	public Button button1;
	public Button button2;
	public Button button3;
	public Button button4;

	public Toggle toggle_afficher_mdp;

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
		button1 = GameObject.Find("Btn Retour Co").GetComponent<Button>();
		button1.onClick.AddListener(() => buttonCallBack(button1));
		button1.Select();
		button2 = GameObject.Find("Btn ForgottenPwdUser").GetComponent<Button>();
		button2.onClick.AddListener(() => buttonCallBack(button2));
		button3 = GameObject.Find("Btn Creer un compte").GetComponent<Button>();
		button3.onClick.AddListener(() => buttonCallBack(button3));
		button4 = GameObject.Find("Btn Se Connecter").GetComponent<Button>();
		button4.onClick.AddListener(() => buttonCallBack(button4));

		toggle_afficher_mdp = GameObject.Find("Toggle AfficherMdp").GetComponent<Toggle>();
		toggle_afficher_mdp.onValueChanged.AddListener(delegate { ToggleValueChanged(toggle_afficher_mdp); });
	}


	void ToggleValueChanged(Toggle change)
	{
		if (change == toggle_afficher_mdp)
		{
			HideShowPwd();
		}
		GameObject.Find("SoundController").GetComponent<AudioSource>().Play();
	}

	private void buttonCallBack(Button buttonPressed)
	{
		if (buttonPressed == button1)
		{
			Debug.Log("Clicked: " + button1.name);
			HideConnection();
		}

		if (buttonPressed == button2)
		{
			Debug.Log("Clicked: " + button2.name);
			ForgottenPwdUser();
		}

		if (buttonPressed == button3)
		{
			Debug.Log("Clicked: " + button3.name);
			CreateAccount();
		}

		if (buttonPressed == button4)
		{
			Debug.Log("Clicked: " + button4.name);
			Connect();
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
		button2.onClick.RemoveAllListeners();
		button3.onClick.RemoveAllListeners();
		button4.onClick.RemoveAllListeners();
	}

	public void ResetWarningTextCM()
	{
		GameObject tmpGO = GameObject.Find("Instructions");
		Text tmpText = tmpGO.GetComponent<Text>();
		TryColor(tmpGO, Color.white, "f4fefe");
		tmpText.text = "Connectez vous";
	}

	public void HideConnection()
	{
		ResetWarningTextCM();
		ChangeMenu(FindMenu("ConnectionMenu"), FindMenu("HomeMenu"));
	}

	public void ForgottenPwdUser()
	{
		Application.OpenURL("https://tinyurl.com/Kakyoin-and-Polnareff");
	}

	public void HideShowPwd()
	{
		if (GameObject.Find("Toggle AfficherMdp").GetComponent<Toggle>().isOn == true)
		{
			GameObject.Find("InputField Password").GetComponent<InputField>().inputType = InputField.InputType.Standard;
		}
		else
		{
			GameObject.Find("InputField Password").GetComponent<InputField>().inputType = InputField.InputType.Password;
		}

		//permet le changement immediat, sans cette ligne, on doit cliquer sur l'inputfield pour que le changement se fasse
		GameObject.Find("InputField Password").GetComponent<InputField>().ForceLabelUpdate();
	}

	public void Connect()
	{
		bool a = StrCompare(RemoveLastSpace(GameObject.Find("InputField Email/Login").GetComponent<InputField>().text), "");
		bool b = StrCompare(GameObject.Find("InputField Password").GetComponent<InputField>().text, "");
		SetState(a && b);
		GameObject tmpGO = GameObject.Find("Instructions");
		Text tmpText = tmpGO.GetComponent<Text>();
		if (GetState() == true)
		{
			HideConnection();
			Connected();
		}
		else
		{
			DisplayFlex();
			RandomIntColor(tmpGO);
			tmpText.text = "Ressaissiez votre login et votre mot de passe !";
		}
	}

	public void CreateAccount()
	{
		GameObject tmpGO = GameObject.Find("Instructions");
		Text tmpText = tmpGO.GetComponent<Text>();
		TryColor(tmpGO, Color.white, "f4fefe");
		tmpText.text = "Connectez vous";
		ChangeMenu(FindMenu("ConnectionMenu"), FindMenu("CreateAccountMenu"));
	}
}