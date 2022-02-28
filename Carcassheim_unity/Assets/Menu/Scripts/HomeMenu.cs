/*using System.Collections;
using System.Collections.Generic;*/
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// include fonctions du script via la classe ConnectionMenu + (incluant elle même Miscellaneous + Monobehaviour)
public class HomeMenu : Miscellaneous // Reference a class in another script properly
{
	// Start is called before the first frame update
	void Start()
	{
	}

	// Awake is called when the script object is initialised
	void Awake()
	{
		Color newCol;
		if (GetState() == false && FindMenu("HomeMenu").activeSelf == true)
		{
			FindGOTool("HomeMenu", "Btn Jouer").GetComponent<Button>().interactable = GetState();
			FindGOTool("HomeMenu", "Btn Statistiques").GetComponent<Button>().interactable = GetState();
			ColorUtility.TryParseHtmlString("#808080", out newCol);
			GameObject.Find("Btn Jouer").GetComponent<Button>().GetComponentInChildren<Text>().color = newCol;
			GameObject.Find("Btn Statistiques").GetComponent<Button>().GetComponentInChildren<Text>().color = newCol;
		}
	}

	// Update is called once per frame
	void Update()
	{
	}

	public void Jouer()
	{
		ChangeMenu(FindMenu("HomeMenu"), FindMenu("RoomSelectionMenu"));
		//RandomIntColor(GameObject.Find("Etat de connexion"));
	/* SceneManager.LoadScene("InGame"); */
	}

	public void ShowStatistiques()
	{
		ChangeMenu(FindMenu("HomeMenu"), FindMenu("StatistiquesMenu"));
	}

	public void ShowOptions()
	{
		ChangeMenu(FindMenu("HomeMenu"), FindMenu("OptionsMenu"));
	}

	public void ShowConnection()
	{
		ChangeMenu(FindMenu("HomeMenu"), FindMenu("ConnectionMenu"));
	}

	public void Quitter()
	{
		Application.Quit();
		Debug.Log("Quit!");
	}
}