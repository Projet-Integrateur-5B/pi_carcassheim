/*using System.Collections;
using System.Collections.Generic;*/
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HomeMenu : MonoBehaviour
{
	private static ConnectionMenu co;
	private static Miscellaneous ms;
	// Start is called before the first frame update
	void Start()
	{
		// SCRIPT :
		ms = gameObject.AddComponent(typeof(Miscellaneous)) as Miscellaneous;
		co = gameObject.AddComponent(typeof(ConnectionMenu)) as ConnectionMenu;
		/* Debug.Log ("antiLoop"); */
		/* GameObject.Find("Btn Statistiques").GetComponent<Button>().GetComponent<Button>().interactable = co.getState(); */
		Color newCol;
		if (co.getState() == false && ms.FindMenu("HomeMenu").activeSelf == true)
		{
			ms.FindGoTool("HomeMenu", "Btn Jouer").GetComponent<Button>().interactable = co.getState();
			ms.FindGoTool("HomeMenu", "Btn Statistiques").GetComponent<Button>().interactable = co.getState();
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
		ms.randomIntColor(GameObject.Find("Etat de connexion"));
		/* SceneManager.LoadScene("InGame"); */
	}

	public void Statistiques()
	{
		ms.randomIntColor(GameObject.Find("Etat de connexion"));
	}

	public void ShowOptions()
	{
		ms.changeMenu(ms.FindMenu("HomeMenu"), ms.FindMenu("OptionsMenu"));
	}

	public void ShowConnection()
	{
		ms.changeMenu(ms.FindMenu("HomeMenu"), ms.FindMenu("ConnectionMenu"));
	}

	public void Quitter()
	{
		Application.Quit();
		Debug.Log("Quit!");
	}
}