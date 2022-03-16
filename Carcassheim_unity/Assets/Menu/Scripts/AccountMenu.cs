using UnityEngine;
using UnityEngine.UI; /* using System.Text.RegularExpressions; // needed for Regex */

public class AccountMenu : Miscellaneous
{
	private Toggle toggle_afficher_mdp_ca;
	private Transform accMenu;
	private InputField passwordCA, confirmPwdCA;

	void Start()
	{
		// INITIALISATION
		accMenu = GameObject.Find("SubMenus").transform.Find("AccountMenu").transform;
		accMenu.Find("Toggle CA").GetComponent<Toggle>().isOn = false;
		accMenu.Find("InputField Day CA").GetComponent<InputField>().characterLimit = 2;
		accMenu.Find("InputField Month CA").GetComponent<InputField>().characterLimit = 2;
		accMenu.Find("InputField Year CA").GetComponent<InputField>().characterLimit = 4;
		passwordCA = accMenu.Find("InputField Password CA").GetComponent<InputField>();
		confirmPwdCA = accMenu.Find("InputField ConfirmPwd CA").GetComponent<InputField>();
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
		ChangeMenu("AccountMenu", "ConnectionMenu");
	}

	public void HideAccountConnected()
	{
		ResetWarningTextAM();
		ChangeMenu("AccountMenu", "HomeMenu");
		Connected();
	}

	public void ShowPwdAcc()
	{
		if (GameObject.Find("ShowPwdAcc").GetComponent<Toggle>().isOn == true)
			passwordCA.inputType = confirmPwdCA.inputType = InputField.InputType.Standard;
		else
			passwordCA.inputType = confirmPwdCA.inputType = InputField.InputType.Password;
		//permet le changement immediat, sans cette ligne, on doit cliquer sur l'inputfield pour que le changement se fasse
		passwordCA.ForceLabelUpdate();
		confirmPwdCA.ForceLabelUpdate();
	}

	public bool GetInputFields()
	{
		bool a = string.Equals(RemoveLastSpace(GameObject.Find("InputField Pseudo CA").GetComponent<InputField>().text), "a");
		bool b = string.Equals(RemoveLastSpace(GameObject.Find("InputField Email CA").GetComponent<InputField>().text), "b");
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
			tmpGO.GetComponent<Text>().color = Color.yellow;
			tmpText.text = "Ressaisissez vos informations et acceptez les CGU en cochant la case !";
		}
	}

	public void CGU()
	{
		Application.OpenURL("https://tinyurl.com/Kakyoin-and-Polnareff");
	}
}