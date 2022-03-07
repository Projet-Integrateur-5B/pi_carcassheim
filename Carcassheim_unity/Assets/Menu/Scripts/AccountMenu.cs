/*using System.Collections;
using System.Collections.Generic;*/
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/* using System.Text.RegularExpressions; // needed for Regex */
public class AccountMenu : Miscellaneous, IPointerEnterHandler, IPointerExitHandler
{
	public Button button1;
	public Button button2;
	public Button button3;
	public Button button4;

	public Toggle toggle_afficher_mdp_ca;

	private GameObject currentGo;
	private Color _previousColor;
	private Text _btnText;


	EventSystem eventSystem;

	// Start is called before the first frame update
	void Start()
	{
		button1.Select();
		if (FindMenu("CreateAccountMenu").activeSelf == true)
		{
			GameObject.Find("Toggle CA").GetComponent<Toggle>().isOn = false;
			InputField tmpDay = GameObject.Find("InputField Day CA").GetComponent<InputField>();
			tmpDay.characterLimit = 2;
			InputField tmpDay2 = GameObject.Find("InputField Month CA").GetComponent<InputField>();
			tmpDay2.characterLimit = 2;
			InputField tmpDay3 = GameObject.Find("InputField Year CA").GetComponent<InputField>();
			tmpDay3.characterLimit = 4;
			/*  InputField.CharacterValidation =  tmpDay.CharacterValidation.None; */
			/* Regex.Replace(tmpDay.text, @"[^a-zA-Z0-9 ]", ""); */
		}
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
		button1 = GameObject.Find("Btn Retour Crea CA").GetComponent<Button>();
		button1.onClick.AddListener(() => buttonCallBack(button1));
		button1.Select();
		button2 = GameObject.Find("Btn Creer votre compte").GetComponent<Button>();
		button2.onClick.AddListener(() => buttonCallBack(button2));
		button3 = GameObject.Find("Btn CGU").GetComponent<Button>();
		button3.onClick.AddListener(() => buttonCallBack(button3));

		toggle_afficher_mdp_ca = GameObject.Find("Toggle AfficherMdp CA").GetComponent<Toggle>();
		toggle_afficher_mdp_ca.onValueChanged.AddListener(delegate { ToggleValueChanged(toggle_afficher_mdp_ca); });
	}


	void ToggleValueChanged(Toggle change)
	{
		if (change == toggle_afficher_mdp_ca)
		{
			HideShowPwdConf();
		}
		GameObject.Find("SoundController").GetComponent<AudioSource>().Play();
	}

	private void buttonCallBack(Button buttonPressed)
	{
		if (buttonPressed == button1)
		{
			Debug.Log("Clicked: " + button1.name);
			HideAccount();
		}

		if (buttonPressed == button2)
		{
			Debug.Log("Clicked: " + button2.name);
			CreateAccountConnected();
		}

		if (buttonPressed == button3)
		{
			Debug.Log("Clicked: " + button3.name);
			//CreateAccount();
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
	}

	public void ResetWarningTextAM()
	{
		GameObject tmpGO = GameObject.Find("Create Account");
		Debug.Log(tmpGO); // A DEBUGUER (connection menu fonctionne, pourtant c'est identique) => Navigation Manager
		Text tmpText = tmpGO.GetComponent<Text>();
		TryColor(tmpGO, Color.white, "f4fefe");
		tmpText.text = "Creez votre compte";
	}

	public void HideAccount()
	{
		ResetWarningTextAM();
		ChangeMenu(FindMenu("CreateAccountMenu"), FindMenu("ConnectionMenu"));
	}

	public void HideAccountConnected()
	{
		ResetWarningTextAM();
		ChangeMenu(FindMenu("CreateAccountMenu"), FindMenu("HomeMenu"));
		Connected();
	}

	public void HideShowPwdConf()
	{
		if (GameObject.Find("Toggle AfficherMdp CA").GetComponent<Toggle>().isOn == true)
		{
			GameObject.Find("InputField Password CA").GetComponent<InputField>().inputType = InputField.InputType.Standard;
			GameObject.Find("InputField ConfirmPwd CA").GetComponent<InputField>().inputType = InputField.InputType.Standard;
		}
		else
		{
			GameObject.Find("InputField Password CA").GetComponent<InputField>().inputType = InputField.InputType.Password;
			GameObject.Find("InputField ConfirmPwd CA").GetComponent<InputField>().inputType = InputField.InputType.Password;
		}

		//permet le changement immediat, sans cette ligne, on doit cliquer sur l'inputfield pour que le changement se fasse
		GameObject.Find("InputField Password CA").GetComponent<InputField>().ForceLabelUpdate();
		GameObject.Find("InputField ConfirmPwd CA").GetComponent<InputField>().ForceLabelUpdate();
	}

	public bool GetInputFields()
	{
		bool a = StrCompare(RemoveLastSpace(GameObject.Find("InputField Pseudo CA").GetComponent<InputField>().text), "a");
		bool b = StrCompare(RemoveLastSpace(GameObject.Find("InputField Email CA").GetComponent<InputField>().text), "b");
		bool c = StrCompare(GameObject.Find("InputField Day CA").GetComponent<InputField>().text, "01");
		bool d = StrCompare(GameObject.Find("InputField Month CA").GetComponent<InputField>().text, "02");
		bool e = StrCompare(GameObject.Find("InputField Year CA").GetComponent<InputField>().text, "0304");
		string tmpPwd = GameObject.Find("InputField Password CA").GetComponent<InputField>().text;
		string tmpPwd2 = GameObject.Find("InputField ConfirmPwd CA").GetComponent<InputField>().text;
		bool f = StrCompare(tmpPwd, "c");
		bool g = StrCompare(tmpPwd2, tmpPwd);
		return a && b && c && d && e && f && g;
	}

	public void CreateAccountConnected()
	{
		bool tmpBool = GameObject.Find("Toggle CA").GetComponent<Toggle>().isOn;
		GameObject tmpGO = GameObject.Find("Create Account");
		//Texte deborde sur formulaire. Rendre code portable et utilisable :
		// Modification position texte en ajoutant a sa coordonne la moitie de sa hauteur.
		Text tmpText = tmpGO.GetComponent<Text>();
		SetState(tmpBool && GetInputFields());
		if (GetState() == true)
			HideAccountConnected();
		else
		{
			DisplayFlex();
			RandomIntColor(tmpGO);
			tmpText.text = "Ressaisissez vos informations et acceptez les CGU en cochant la case !";
		}
	}
}