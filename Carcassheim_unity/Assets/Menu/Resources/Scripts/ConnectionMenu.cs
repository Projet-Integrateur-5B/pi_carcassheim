using UnityEngine;
using UnityEngine.UI;
using Assets.System;
using ClassLibrary;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading;

public class ConnectionMenu : Miscellaneous
{
	private Transform coMenu, CMCI; // Account Menu Container InputField
	private InputField loginCM, passwordCM;
	private GameObject tmpGO;
	private Text tmpText;

	public List<bool> listAction;
	public Semaphore s_listAction;
	
	void Start()
	{
		coMenu = GameObject.Find("SubMenus").transform.Find("ConnectionMenu").transform;
		CMCI = coMenu.Find("InputField").transform.Find("InputFieldEndEdit").transform;
		loginCM = CMCI.GetChild(0).GetComponent<InputField>();
		passwordCM = CMCI.GetChild(1).GetComponent<InputField>();
		passwordCM.inputType = InputField.InputType.Password; // Hide password by default

		listAction = new List<bool>();
		s_listAction = new Semaphore(1, 1);


		/* Commuication Async */
		Communication.Instance.StartListening(OnPacketReceived);
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

		/* Stop la reception dans cette class */
		Communication.Instance.StopListening(OnPacketReceived);
	}

	public void ForgottenPwdUser()
	{
		
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

		/* Stop la reception dans cette class */
		Communication.Instance.StopListening(OnPacketReceived);
	}

	public void InputFieldEndEdit(InputField inp)
	{
		Debug.Log("Input submitted" + " : " + inp.text);
	}

	public void Connect()
	{

		Packet packet = new Packet();
		packet.IdMessage = Tools.IdMessage.AccountLogin;
		packet.IdPlayer = 0;
		packet.Data = new[] { RemoveLastSpace(loginCM.text), RemoveLastSpace(passwordCM.text) };

		tmpGO = GameObject.Find("Instructions");
		tmpText = tmpGO.GetComponent<Text>();

		Communication.Instance.SendAsync(packet);
	}

	public void OnPacketReceived(object sender, Packet packet)
    {

		bool res = false;
		if(packet.IdMessage == Tools.IdMessage.AccountLogin)
        {
			if(packet.Error == Tools.Errors.None)
            {
				Communication.Instance.idClient = packet.IdPlayer;
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

		if(taille > 0)
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
}