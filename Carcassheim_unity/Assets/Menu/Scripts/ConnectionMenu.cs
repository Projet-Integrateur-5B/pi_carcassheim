using UnityEngine;
using UnityEngine.UI;

public class ConnectionMenu : Miscellaneous
{
	private Toggle toggle_afficher_mdp;
	void Start()
	{
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
		ResetWarningTextCM();
		ChangeMenu("ConnectionMenu", "HomeMenu");
	}

	public void ForgottenPwdUser()
	{
		Application.OpenURL("https://tinyurl.com/Kakyoin-and-Polnareff");
	}

	public void ShowPwd()
	{
		if (GameObject.Find("ShowPwd").GetComponent<Toggle>().isOn == true)
			GameObject.Find("InputField Password").GetComponent<InputField>().inputType = InputField.InputType.Standard;
		else
			GameObject.Find("InputField Password").GetComponent<InputField>().inputType = InputField.InputType.Password;
		// Permet le changement immediat, sans cette ligne, on doit cliquer sur l'inputfield pour que le changement se fasse
		GameObject.Find("InputField Password").GetComponent<InputField>().ForceLabelUpdate();
	}

	public void ShowAccount()
	{
		GameObject tmpGO = GameObject.Find("Instructions");
		Text tmpText = tmpGO.GetComponent<Text>();
		tmpText.color = Color.white;
		tmpText.text = "Connectez vous";
		ChangeMenu("ConnectionMenu", "AccountMenu");
	}

	public void Connect()
	{
		bool a = string.Equals(RemoveLastSpace(GameObject.Find("InputField Email/Login").GetComponent<InputField>().text), "");
		bool b = string.Equals(GameObject.Find("InputField Password").GetComponent<InputField>().text, "");
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
			tmpText.text = "Ressaissiez votre login et votre mot de passe !";
		}
	}
}