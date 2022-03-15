/*using System.Collections;
using System.Collections.Generic;*/
using UnityEngine;
using UnityEngine.UI;

/* using System.Text.RegularExpressions; // needed for Regex */
public class AccountMenu : Miscellaneous
{
	private Toggle toggle_afficher_mdp_ca;
	private Transform accMenu;
	// Start is called before the first frame update
	void Start()
	{
		// INITIALISATION
		accMenu = GameObject.Find("SubMenus").transform.Find("AccountMenu").transform;
		accMenu.Find("Toggle CA").GetComponent<Toggle>().isOn = false;
		InputField tmpDay = accMenu.Find("InputField Day CA").GetComponent<InputField>();
		tmpDay.characterLimit = 2;
		InputField tmpDay2 = accMenu.Find("InputField Month CA").GetComponent<InputField>();
		tmpDay2.characterLimit = 2;
		InputField tmpDay3 = accMenu.Find("InputField Year CA").GetComponent<InputField>();
		tmpDay3.characterLimit = 4;
	// PATCH : (à faire)
	/* 		toggle_afficher_mdp_ca = GameObject.Find("Toggle AfficherMdp CA").GetComponent<Toggle>();
		toggle_afficher_mdp_ca.onValueChanged.AddListener(delegate { ToggleValueChanged(toggle_afficher_mdp_ca); }); */
	}

	void Update()
	{
	}

	void ToggleValueChanged(Toggle change)
	{
		if (change == toggle_afficher_mdp_ca)
		{
			HidePwdAcc();
		}

		GameObject.Find("SoundController").GetComponent<AudioSource>().Play();
	}

	public void ResetWarningTextAM()
	{
		Debug.Log("grgrqgr");
		GameObject tmpGO = GameObject.Find("Create Account");
		Debug.Log("lpkooopl" + tmpGO);
		Text tmpText = tmpGO.GetComponent<Text>();
		TryColor(tmpGO, Color.white, "f4fefe");
		tmpText.text = "Creez votre compte";
	}

	public void HideAccount()
	{
		ResetWarningTextAM();
		ChangeMenu("AccountMenu", "ConnectionMenu");
	}

	public void HideAccountConnected()
	{
		ResetWarningTextAM();
		ChangeMenu("AccountMenu", "HomeMenu");
		Connected();
	}

	public void HidePwdAcc()
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

	public void CreateAccount()
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

	public void CGU()
	{
		Application.OpenURL("https://tinyurl.com/Kakyoin-and-Polnareff");
	}
}