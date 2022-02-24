/*using System.Collections;
using System.Collections.Generic;*/
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/* using System.Text.RegularExpressions; // needed for Regex */
public class AccountMenu : ConnectionMenu
{
	private static bool modif_y_text = false;
	// Start is called before the first frame update
	void Start()
	{
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

	// Update is called once per frame
	void Update()
	{
	}

	public void resetWarningTextAM()
	{
		GameObject tmpGO = GameObject.Find("Create Account");
		Text tmpText = tmpGO.GetComponent<Text>();
		tryColor(tmpGO, Color.white, "f4fefe");
		tmpText.text = "Creez votre compte";
	}

	public void HideAccount()
	{
		resetWarningTextAM();
		changeMenu(FindMenu("CreateAccountMenu"), FindMenu("ConnectionMenu"));
	}

	public void HideAccountConnected()
	{
		resetWarningTextAM();
		changeMenu(FindMenu("CreateAccountMenu"), FindMenu("HomeMenu"));
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

	public bool getInputFields()
	{
		bool a = StrCompare(removeLastSpace(GameObject.Find("InputField Pseudo CA").GetComponent<InputField>().text), "a");
		bool b = StrCompare(removeLastSpace(GameObject.Find("InputField Email CA").GetComponent<InputField>().text), "b");
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
		//le texte etait coup car trop long (debordait sur le formulaire)
		//donc pour rendre le code portable et utilisable peu importe la taille du texte
		//on modifie la position du texte en ajoutant a sa coordonne y la moitie de sa hauteur pour que le texte ne deborde plus
		//le bool modif_y_text est la pour que l'ajout en y ne se fasse qu'une seule fois
		if (modif_y_text == false)
		{
			Vector3 up_y = new Vector3(0, tmpGO.GetComponent<RectTransform>().rect.height / 2, 0) + tmpGO.transform.position;
			tmpGO.transform.position = up_y;
			modif_y_text = true;
		}
		Text tmpText = tmpGO.GetComponent<Text>();
		setState(tmpBool && getInputFields());
		if (getState() == true)
		{
			HideAccountConnected();
		}
		else
		{
			randomIntColor(tmpGO);
			tmpText.text = "Ressaisissez vos informations et acceptez les CGU en cochant la case !";
		}
	}
}