/*using System.Collections;
using System.Collections.Generic;*/
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/* using System.Text.RegularExpressions; // needed for Regex */
public class AccountMenu : Miscellaneous
{
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