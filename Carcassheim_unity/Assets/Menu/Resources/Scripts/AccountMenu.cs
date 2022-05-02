using UnityEngine;
using UnityEngine.UI; /* using System.Text.RegularExpressions; // needed for Regex */
using Assets.System;
using ClassLibrary;
using System.Collections.Generic;
using System.Threading;

/// <summary>
///     Account menu.
/// </summary>
public class AccountMenu : Miscellaneous
{
	private Transform accMenu, AMCI; // Account Menu Container InputField
	private InputField pseudoCA, emailCA, passwordCA, confirmPwdCA;
	private static bool boolCGU = false;
	public List<bool> listAction;
	public Semaphore s_listAction;
	private GameObject tmpGO;
	private Text tmpText;

	/// <summary>
	/// Start is called before the first frame update <see cref = "AccountMenu"/> class.
	/// </summary>
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

	/// <summary>
	/// OnStart is called when the menu is changed to this one <see cref = "AccountMenu"/> class.
	/// </summary>
	/// <param name = "pageName">Page name.</param>
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

	/// <summary>
	/// Reset Warning on text fields <see cref = "AccountMenu"/> class.
	/// </summary>
	public void ResetWarningTextAM()
	{
	/*
		tmpText.color = Color.white;
		tmpText.text = "Creez votre compte";
		*/
	}

	/// <summary>
	/// Hide Account menu when connected to the server <see cref = "AccountMenu"/> class.
	/// </summary>
	public void HideAccount()
	{
		ResetWarningTextAM();
		HidePopUpOptions();
		ChangeMenu("AccountMenu", "ConnectionMenu");
	}

	/// <summary>
	/// Hide Account when connected to the server <see cref = "AccountMenu"/> class.
	/// </summary>
	public void HideAccountConnected()
	{
		ResetWarningTextAM();
		HidePopUpOptions();
		ChangeMenu("AccountMenu", "HomeMenu");
		Connected();
	}

	/// <summary>
	/// Toggle Password visibility <see cref = "AccountMenu"/> class.
	/// </summary>
	/// <param name = "curT">Current Toggle</param>
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

	/// <summary>
	/// Get the input value of the input fields password and confirm password <see cref = "AccountMenu"/> class.
	/// </summary>
	/// <returns>The value of the input fields password and confirm password</returns>
	public bool GetInputFields()
	{
		string tmpPwd = RemoveLastSpace(passwordCA.text);
		string tmpPwd2 = RemoveLastSpace(confirmPwdCA.text);
		return string.Equals(tmpPwd2, tmpPwd) && boolCGU;
	}

	/// <summary>
	/// Create the account <see cref = "AccountMenu"/> class.
	/// </summary>
	public void CreateAccount()
	{
		if (GetInputFields())
		{
			Packet packet = new Packet();
			packet.IdMessage = Tools.IdMessage.AccountSignup;
			packet.IdPlayer = 0;
			packet.Data = new[]{RemoveLastSpace(pseudoCA.text), RemoveLastSpace(passwordCA.text), RemoveLastSpace(emailCA.text), GameObject.Find("InputField Year CA").GetComponent<InputField>().text + "/" + GameObject.Find("InputField Month CA").GetComponent<InputField>().text + "/" + GameObject.Find("InputField Day CA").GetComponent<InputField>().text};
			Communication.Instance.SendAsync(packet);
		}
	}

	/// <summary>
	 // CGU Link
	/// </summary>
	public void CGU()
	{
	//Application.OpenURL("https://tinyurl.com/Kakyoin-and-Polnareff");
	}

	/// <summary>
	/// OnPacketReceived is called when a packet is received <see cref = "AccountMenu"/> class.
	/// </summary>
	/// <param name = "sender">Sender.</param>
	/// <param name = "packet">Packet.</param>
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

	/// <summary>
	/// Update every frame <see cref = "AccountMenu"/> class.
	/// </summary>
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

	/// <summary>
	/// Clean all input fields <see cref = "AccountMenu"/> class.
	/// </summary>
	public void ClearAll(string arg)
	{
		pseudoCA = Clear(pseudoCA);
		emailCA = Clear(emailCA);
		passwordCA = Clear(passwordCA);
		confirmPwdCA = Clear(confirmPwdCA);
	}
}