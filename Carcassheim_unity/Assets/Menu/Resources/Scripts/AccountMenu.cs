using UnityEngine;
using UnityEngine.UI; /* using System.Text.RegularExpressions; // needed for Regex */
using Assets.System;

public class AccountMenu : Miscellaneous
{
	private Transform accMenu, AMCI; // Account Menu Container InputField
	private InputField pseudoCA, emailCA, passwordCA, confirmPwdCA;
	private static bool boolCGU = false;
	void Start()
	{
		// INITIALISATION
		accMenu = GameObject.Find("SubMenus").transform.Find("AccountMenu").transform;
		AMCI = accMenu.Find("InputField").transform.Find("InputFieldEndEdit").transform;
		AMCI.Find("InputField Day CA").GetComponent<InputField>().characterLimit = 2;
		AMCI.Find("InputField Month CA").GetComponent<InputField>().characterLimit = 2;
		AMCI.Find("InputField Year CA").GetComponent<InputField>().characterLimit = 4;
		pseudoCA = AMCI.Find("InputField Pseudo CA").GetComponent<InputField>();
		emailCA = AMCI.Find("InputField Email CA").GetComponent<InputField>();
		passwordCA = AMCI.Find("InputField Password CA").GetComponent<InputField>();
		confirmPwdCA = AMCI.Find("InputField ConfirmPwd CA").GetComponent<InputField>();
		passwordCA.inputType = confirmPwdCA.inputType = InputField.InputType.Password; // Hide password by default
	}

	public void ResetWarningTextAM()
	{
		GameObject tmpGO = GameObject.Find("Create Account");
		Text tmpText = tmpGO.GetComponent<Text>();
		tmpText.color = Color.white;
		tmpText.text = "Creez votre compte";
	}

	public void HideAccount()
	{
		ResetWarningTextAM();
		HidePopUpOptions();
		ChangeMenu("AccountMenu", "ConnectionMenu");
	}

	public void HideAccountConnected()
	{
		ResetWarningTextAM();
		HidePopUpOptions();
		ChangeMenu("AccountMenu", "HomeMenu");
		Connected();
	}

	public void ToggleValueChangedAM(Toggle curT)
	{
		if (curT.name == "Toggle ShowPwdAcc")
		{
			if (curT.isOn)
				passwordCA.inputType = confirmPwdCA.inputType = InputField.InputType.Standard;
			else
				passwordCA.inputType = confirmPwdCA.inputType = InputField.InputType.Password;
			//Changement immédiat sans reclic InputField
			passwordCA.ForceLabelUpdate();
			confirmPwdCA.ForceLabelUpdate();
		}

		if (curT.name == "Toggle CGU")
			boolCGU = !boolCGU;
	}

	public bool GetInputFields()
	{
		string tmpPwd = RemoveLastSpace(passwordCA.text);
		string tmpPwd2 = RemoveLastSpace(confirmPwdCA.text);

		return string.Equals(tmpPwd2, tmpPwd) && boolCGU;
	}

	public void CreateAccount()
	{
		//bool tmpBool = GameObject.Find("Toggle CA").GetComponent<Toggle>().isOn;
		GameObject tmpGO = GameObject.Find("Create Account");
		//Texte deborde sur formulaire. Rendre code portable et utilisable :
		// Modification position texte en ajoutant a sa coordonne la moitie de sa hauteur.
		Text tmpText = tmpGO.GetComponent<Text>();
		bool res = false;

		if (GetInputFields())
        {
			string[] values = new[] {
			RemoveLastSpace(pseudoCA.text),
			RemoveLastSpace(passwordCA.text),
			RemoveLastSpace(emailCA.text),
			GameObject.Find("InputField Year CA").GetComponent<InputField>().text +"/"+
			GameObject.Find("InputField Month CA").GetComponent<InputField>().text +"/"+
			GameObject.Find("InputField Day CA").GetComponent<InputField>().text
			};
			res = Communication.Instance.CommunicationWithoutResult(Tools.IdMessage.Signup, values);
		}

		SetState(res);
		if (GetState() == true)
		{
			HideAccountConnected();
			return;
		}
        else
        {
			tmpGO.GetComponent<Text>().color = Color.yellow;
			tmpText.text = "Ressaisissez vos informations et acceptez les CGU en cochant la case !";
		}
	}

	public void CGU()
	{
		Application.OpenURL("https://tinyurl.com/Kakyoin-and-Polnareff");
	}
}