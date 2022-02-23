/*using System.Collections;
using System.Collections.Generic;*/
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ConnectionMenu : Miscellaneous 
{
	private static bool State = false;
	private static bool modif_y_text = false;
	// Start is called before the first frame update
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
	}

	public bool getState()
	{
		return State;
	}

	public void setState(bool b)
	{
		State = b;
	}

	public void resetWarningTextCM()
	{
		GameObject tmpGO = GameObject.Find("Instructions");
		Text tmpText = tmpGO.GetComponent<Text>();
		tryColor(tmpGO, Color.white, "f4fefe");
		tmpText.text = "Connectez vous";
	}

	public void HideConnection()
	{
		resetWarningTextCM();
		changeMenu(FindMenu("ConnectionMenu"), FindMenu("HomeMenu"));
	}

	public void ForgottenPwdUser()
	{
		Application.OpenURL("https://tinyurl.com/Kakyoin-and-Polnareff");
	}

	public void Connected()
	{
		Color newCol;
		Button tmpStat = GameObject.Find("Btn Statistiques").GetComponent<Button>();
		Button tmpJouer = GameObject.Find("Btn Jouer").GetComponent<Button>();
		tryColor(GameObject.Find("Etat de connexion"), Color.green, "#90EE90");
		GameObject.Find("Etat de connexion").GetComponent<Text>().text = "Connecte";
		//on place le texte Connecte la ou il y avait le bouton Se connecter
		GameObject.Find("Etat de connexion").transform.position = GameObject.Find("Btn Connexion").transform.position;
		GameObject.Find("Btn Connexion").SetActive(false);
		tmpJouer.interactable = true;
		tmpStat.interactable = true;
		ColorUtility.TryParseHtmlString("#f4fefe", out newCol);
		tmpJouer.GetComponentInChildren<Text>().color = newCol;
		tmpStat.GetComponentInChildren<Text>().color = newCol;
		Debug.Log("Connecté");
	}

	public void Connect()
	{
		bool a = StrCompare(GameObject.Find("InputField Email/Login").GetComponent<InputField>().text, "Hello");
		bool b = StrCompare(GameObject.Find("InputField Password").GetComponent<InputField>().text, "World");
		State = a && b;
		GameObject tmpGO = GameObject.Find("Instructions");
		//idem que dans AccountMenu 
		if (modif_y_text == false)
		{
			Vector3 up_y = new Vector3(0, tmpGO.GetComponent<RectTransform>().rect.height / 4, 0) + tmpGO.transform.position;
			tmpGO.transform.position = up_y;
			modif_y_text = true;
		}

		Text tmpText = tmpGO.GetComponent<Text>();
		if (State == true)
		{
			HideConnection();
			Connected();
		}
		else
		{
			randomIntColor(tmpGO);
			tmpText.text = "Ressaissiez votre login et votre mot de passe !";
		}
	}

	public void CreateAccount()
	{
		GameObject tmpGO = GameObject.Find("Instructions");
		Text tmpText = tmpGO.GetComponent<Text>();
		tryColor(tmpGO, Color.white, "f4fefe");
		tmpText.text = "Connectez vous";
		changeMenu(FindMenu("ConnectionMenu"), FindMenu("CreateAccountMenu"));
	}
}