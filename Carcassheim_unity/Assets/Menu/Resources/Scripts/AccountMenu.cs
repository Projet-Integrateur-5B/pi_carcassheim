using UnityEngine;
using UnityEngine.UI; /* using System.Text.RegularExpressions; // needed for Regex */
using Assets.System;
using ClassLibrary;
using System.Collections.Generic;
using System.Threading;

public class AccountMenu : Miscellaneous
{
	private Transform accMenu, AMCI; // Account Menu Container InputField
	private InputField pseudoCA, emailCA, passwordCA, confirmPwdCA;
	private static bool boolCGU = false;

	public List<bool> listAction;
	public Semaphore s_listAction;

	private GameObject tmpGO;
	private Text tmpText;
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

		listAction = new List<bool>();
		s_listAction = new Semaphore(1, 1);

		/*
		tmpGO = GameObject.Find("Create Account");
		tmpText = tmpGO.GetComponent<Text>();
		*/
		OnMenuChange += OnStart;
		OnMenuChange += ClearAll;
	}

	public void OnStart(string pageName)
    {
		switch (pageName)
		{
			case "AccountMenu":
				/* Commuication Async */
				Communication.Instance.StartListening(OnPacketReceived);
				break;

			default:
				/* Ce n'est pas la bonne page */
				/* Stop la reception dans cette class */
				Communication.Instance.StopListening(OnPacketReceived);
				break;
		}
	}	

	public void ResetWarningTextAM()
	{
		/*
		tmpText.color = Color.white;
		tmpText.text = "Creez votre compte";
		*/
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
		if (GetInputFields())
        {
			Packet packet = new Packet();
			packet.IdMessage = Tools.IdMessage.AccountSignup;
			packet.IdPlayer = 0;
			packet.Data  = new[] {
				RemoveLastSpace(pseudoCA.text),
				RemoveLastSpace(passwordCA.text),
				RemoveLastSpace(emailCA.text),
				GameObject.Find("InputField Year CA").GetComponent<InputField>().text +"/"+
				GameObject.Find("InputField Month CA").GetComponent<InputField>().text +"/"+
				GameObject.Find("InputField Day CA").GetComponent<InputField>().text
			};

			Communication.Instance.SendAsync(packet);
		}

		
	}

	public void CGU()
	{
		//Application.OpenURL("https://tinyurl.com/Kakyoin-and-Polnareff");
	}

	public void OnPacketReceived(object sender, Packet packet)
	{

		bool res = false;
		if (packet.IdMessage == Tools.IdMessage.AccountLogin)
		{
			if (packet.Error == Tools.Errors.None)
			{
				res = true;
			}

			s_listAction.WaitOne();
			listAction.Add(res);
			s_listAction.Release();
		}
	}

	private void Update()
	{
		s_listAction.WaitOne();
		int taille = listAction.Count;
		s_listAction.Release();

		if (taille > 0)
		{
			for (int i = 0; i < taille; i++)
			{
				s_listAction.WaitOne();
				SetState(listAction[i]);
				s_listAction.Release();
			}

			s_listAction.WaitOne();
			listAction.Clear();
			s_listAction.Release();

			if (GetState())
			{
				HideAccountConnected();
				Connected();
			}
			else
			{
				/*
				tmpGO.GetComponent<Text>().color = Color.yellow;
				tmpText.text = "Ressaisissez vos informations et acceptez les CGU en cochant la case !";
				*/
			}
		}
	}

	public void ClearAll(string arg)
	{
		pseudoCA = Clear(pseudoCA);
		emailCA = Clear(emailCA);
		passwordCA = Clear(passwordCA);
		confirmPwdCA = Clear(confirmPwdCA);
	}
}