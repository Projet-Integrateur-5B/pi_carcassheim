using UnityEngine;
using UnityEngine.UI; /* using System.Text.RegularExpressions; // needed for Regex */

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
		bool a = string.Equals(RemoveLastSpace(pseudoCA.text), "a");
		bool b = string.Equals(RemoveLastSpace(emailCA.text), "b");
		bool c = string.Equals(GameObject.Find("InputField Day CA").GetComponent<InputField>().text, "01");
		bool d = string.Equals(GameObject.Find("InputField Month CA").GetComponent<InputField>().text, "02");
		bool e = string.Equals(GameObject.Find("InputField Year CA").GetComponent<InputField>().text, "0304");
		string tmpPwd = passwordCA.text;
		string tmpPwd2 = confirmPwdCA.text;
		bool f = string.Equals(tmpPwd, "c");
		bool g = string.Equals(tmpPwd2, tmpPwd);
		return a && b && c && d && e && f && g;
	}

	public void CreateAccount()
	{
		//bool tmpBool = GameObject.Find("Toggle CA").GetComponent<Toggle>().isOn;
		GameObject tmpGO = GameObject.Find("Create Account");
		//Texte deborde sur formulaire. Rendre code portable et utilisable :
		// Modification position texte en ajoutant a sa coordonne la moitie de sa hauteur.
		Text tmpText = tmpGO.GetComponent<Text>();
		SetState(boolCGU && GetInputFields());
		if (GetState() == true)
			HideAccountConnected();
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