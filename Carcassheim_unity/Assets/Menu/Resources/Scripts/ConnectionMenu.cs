using UnityEngine;
using UnityEngine.UI;

public class ConnectionMenu : Miscellaneous
{
	private Transform coMenu, CMCI; // Account Menu Container InputField
	private InputField loginCM, passwordCM;
	void Start()
	{
		coMenu = GameObject.Find("SubMenus").transform.Find("ConnectionMenu").transform;
		CMCI = coMenu.Find("InputField").transform;
		loginCM = CMCI.GetChild(0).GetComponent<InputField>();
		passwordCM = CMCI.GetChild(1).GetComponent<InputField>();
		passwordCM.inputType = InputField.InputType.Password; // Hide password by default
	}

	public void ResetWarningTextCM()
	{
		GameObject tmpGO = GameObject.Find("Instructions");
		Text tmpText = tmpGO.GetComponent<Text>();
		tmpText.color = Color.white;
		tmpText.text = "Connectez vous";
	}

	public void HideConnection()
	{
		HidePopUpOptions();
		ResetWarningTextCM();
		ChangeMenu("ConnectionMenu", "HomeMenu");
	}

	public void ForgottenPwdUser()
	{
		Application.OpenURL("https://tinyurl.com/Kakyoin-and-Polnareff");
	}

	public void ToggleValueChangedCM(Toggle curT)
	{
		if (curT.name == "Toggle ShowPwdCM")
		{
			if (curT.isOn)
				passwordCM.inputType = InputField.InputType.Standard;
			else
				passwordCM.inputType = InputField.InputType.Password;
			//Changement immédiat sans reclic InputField
			passwordCM.ForceLabelUpdate();
		}
	}

	public void ShowAccount()
	{
		GameObject tmpGO = GameObject.Find("Instructions");
		Text tmpText = tmpGO.GetComponent<Text>();
		tmpText.color = Color.white;
		tmpText.text = "Connectez vous";
		HidePopUpOptions();
		ChangeMenu("ConnectionMenu", "AccountMenu");
	}

	public void Connect()
	{
		bool a = string.Equals(RemoveLastSpace(loginCM.text), "");
		bool b = string.Equals(passwordCM.text, "");
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
			tmpGO.GetComponent<Text>().color = Color.yellow;
			tmpText.text = "Ressaisissez votre login et votre mot de passe !";
		}
	}
}