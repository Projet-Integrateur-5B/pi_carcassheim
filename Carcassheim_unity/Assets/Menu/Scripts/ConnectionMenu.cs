/*using System.Collections;
using System.Collections.Generic;*/
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ConnectionMenu : Miscellaneous
{
	// Start is called before the first frame update
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
	}

	void Awake()
	{
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