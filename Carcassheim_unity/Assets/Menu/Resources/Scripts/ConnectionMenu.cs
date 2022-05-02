using UnityEngine;
using UnityEngine.UI;
using Assets.System;
using ClassLibrary;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading;

/// <summary>
///    Connection Menu.
/// </summary>
public class ConnectionMenu : Miscellaneous
{
	private Transform coMenu, CMCI; // Account Menu Container InputField
	private InputField loginCM, passwordCM;
	private GameObject tmpGO;
	private Text tmpText;
	private Toggle hidePwd;
	public List<bool> listAction;
	public Semaphore s_listAction;
    
	/// <summary>
	/// Start is called before the first frame update <see cref = "ConnectionMenu"/> class.
	/// </summary>
	void Start()
	{
		coMenu = GameObject.Find("SubMenus").transform.Find("ConnectionMenu").transform;
		CMCI = coMenu.Find("InputField").transform.Find("InputFieldEndEdit").transform;
		loginCM = CMCI.GetChild(0).GetComponent<InputField>();
		passwordCM = CMCI.GetChild(1).GetComponent<InputField>();
		passwordCM.inputType = InputField.InputType.Password; // Hide password by default
		hidePwd = FindObject(gameObject, "Toggle ShowPwdCM").GetComponent<Toggle>();
		listAction = new List<bool>();
		s_listAction = new Semaphore(1, 1);
		OnMenuChange += OnStart;
		OnMenuChange += ClearAll;
	}

	/// <summary>
	/// OnStart is called when the menu is changed to this one <see cref = "ConnectionMenu"/> class.
	/// </summary>
	/// <param name = "pageName">Page name.</param>
	public void OnStart(string pageName)
	{
		switch (pageName)
		{
			case "ConnectionMenu":
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
	/// Reset Warning on text fields <see cref = "ConnectionMenu"/> class.
	/// </summary>
	public void ResetWarningTextCM()
	{
		tmpGO = GameObject.Find("Instructions");
		tmpText = tmpGO.GetComponent<Text>();
		tmpText.color = Color.white;
		tmpText.text = "Connectez vous";
	}

	/// <summary>
	/// Hide Connection menu when connected to the server <see cref = "ConnectionMenu"/> class.
	/// </summary>
	public void HideConnection()
	{
		HidePopUpOptions();
		ResetWarningTextCM();
		ChangeMenu("ConnectionMenu", "HomeMenu");
	}

	/// <summary>
	/// Forgot password <see cref = "ConnectionMenu"/> class.
	/// </summary>
	public void ForgottenPwdUser()
	{
	}

	/// <summary>
	/// Toggle Password visibility <see cref = "ConnectionMenu"/> class.
	/// </summary>
	/// <param name = "curT">Current Toggle</param>
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

	/// <summary>
	/// Show account menu <see cref = "ConnectionMenu"/> class.
	/// </summary>
	public void ShowAccount()
	{
		GameObject tmpGO = GameObject.Find("Instructions");
		Text tmpText = tmpGO.GetComponent<Text>();
		tmpText.color = Color.white;
		tmpText.text = "Connectez vous";
		HidePopUpOptions();
		ChangeMenu("ConnectionMenu", "AccountMenu");
	}

	/// <summary>
	/// InputField End Edit  <see cref = "ConnectionMenu"/> class.
	/// </summary>
	/// <param name = "inp">InputField</param>
	public void InputFieldEndEdit(InputField inp)
	{
	// Debug.Log("Input submitted" + " : " + inp.text);
	}

	/// <summary>
	/// Connect to the server <see cref = "ConnectionMenu"/> class.
	/// </summary>
	public void Connect()
	{
		Packet packet = new Packet();
		packet.IdMessage = Tools.IdMessage.AccountLogin;
		packet.IdPlayer = 0;
		packet.Data = new[]{RemoveLastSpace(loginCM.text), RemoveLastSpace(passwordCM.text)};
		tmpGO = GameObject.Find("Instructions");
		tmpText = tmpGO.GetComponent<Text>();
		Communication.Instance.SendAsync(packet);
	}

	/// <summary>
	/// OnPacketReceived is called when a packet is received <see cref = "ConnectionMenu"/> class.
	/// </summary>
	/// <param name = "sender">Sender.</param>
	/// <param name = "packet">Packet</param>
	public void OnPacketReceived(object sender, Packet packet)
	{
		bool res = false;
		if (packet.IdMessage == Tools.IdMessage.AccountLogin)
		{
			if (packet.Error == Tools.Errors.None)
			{
				Communication.Instance.IdClient = packet.IdPlayer;
				res = true;
			}

			s_listAction.WaitOne();
			listAction.Add(res);
			s_listAction.Release();
		}
	}

	/// <summary>
	/// Update is called once per frame <see cref = "ConnectionMenu"/> class.
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
				Connected();
				HideConnection();
			}
			else
			{
				tmpGO.GetComponent<Text>().color = Color.yellow;
				tmpText.text = "Ressaisissez votre login et votre mot de passe !";
			}
		}
	}

	/// <summary>
	/// Clear all input fields <see cref = "ConnectionMenu"/> class.
	/// </summary>
	public void ClearAll(string arg)
	{
		loginCM = Clear(loginCM);
		passwordCM = Clear(passwordCM);
		hidePwd.isOn = false;
	}
}