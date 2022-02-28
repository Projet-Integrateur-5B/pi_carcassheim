using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class Miscellaneous : MonoBehaviour
{
	private GameObject _goToFind;
	private static bool s_state = false;
	private static bool s_menuHasChanged = false;
	private static bool s_displayFlexOnce = false;
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
	}

	// ---- Etat de connection Account et Connection Menu ----
	public bool GetState()
	{
		return s_state;
	}

	public void SetState(bool b)
	{
		s_state = b;
	}

	public void Connected()
	{
		Color newCol;
		Button tmpStat = GameObject.Find("Btn Statistiques").GetComponent<Button>();
		Button tmpJouer = GameObject.Find("Btn Jouer").GetComponent<Button>();
		TryColor(GameObject.Find("Etat de connexion"), Color.green, "#90EE90");
		GameObject.Find("Etat de connexion").GetComponent<Text>().text = "Connecte";
		GameObject.Find("Btn Connexion").SetActive(false);
		tmpJouer.interactable = true;
		tmpStat.interactable = true;
		Debug.Log("CMMM" + tmpJouer.interactable);
		ColorUtility.TryParseHtmlString("#f4fefe", out newCol);
		tmpJouer.GetComponentInChildren<Text>().color = newCol;
		tmpStat.GetComponentInChildren<Text>().color = newCol;
		Debug.Log("Connecté");
	}

	// -------------------------------------------------------
	public void DisplayFlex()
	{
		//bool s_displayFlexOnce : l'ajout en y ne se fasse qu'une seule fois
		GameObject tmpDF = null;
		if (GetCurrentMenu().name == "ConnectionMenu")
			tmpDF = FindGOTool("ConnectionMenu", "Instructions");
		else
			tmpDF = GameObject.Find("Create Account");
		Text tmpDFText = tmpDF.GetComponent<Text>();
		if (s_displayFlexOnce == false)
		{
			Vector3 up_y = new Vector3(0, tmpDF.GetComponent<RectTransform>().rect.height / 4, 0) + tmpDF.transform.position;
			tmpDF.transform.position = up_y;
			s_displayFlexOnce = true;
		}
	}

	public void SetMenuChanged(bool b)
	{
		s_menuHasChanged = b;
	}

	public bool HasMenuChanged()
	{
		return s_menuHasChanged;
	}

	public GameObject GetCurrentMenu()
	{
		_goToFind = GameObject.Find("SubMenus");
		for (int i = 0; i < _goToFind.transform.childCount; i++)
			if (_goToFind.transform.GetChild(i).gameObject.activeSelf)
			{
				_goToFind = _goToFind.transform.GetChild(i).gameObject;
				break;
			}

		return _goToFind;
	}

	public GameObject FindMenu(string menu)
	{
		return GameObject.Find("SubMenus").transform.Find(menu).gameObject;
	}

	public GameObject FindGOTool(string menu, string tool)
	{
		return FindMenu(menu).transform.Find(tool).gameObject;
	}

	public void TryColorText(Text change, Color defaultColor, string coloration)
	{
		Color newCol;
		if (ColorUtility.TryParseHtmlString(coloration, out newCol))
		{
			change.color = newCol;
		}
		else
		{
			change.color = defaultColor;
		}
	}

	public void TryColor(GameObject change, Color defaultColor, string coloration)
	{
		Color newCol;
		if (ColorUtility.TryParseHtmlString(coloration, out newCol))
		{
			change.GetComponent<Text>().color = newCol;
		}
		else
		{
			change.GetComponent<Text>().color = defaultColor;
		}
	}

	public void ChangeMenu(GameObject close, GameObject goTo)
	{
		s_menuHasChanged = false;
		close.SetActive(false);
		goTo.SetActive(true);
		s_menuHasChanged = true;
	}

	public bool StrCompare(string str1, string str2)
	{
		return (str2.Equals(str1));
	}

	public void RandomIntColor(GameObject GO)
	{
		Color randomColor = new Color(Random.Range(0f, 1f), // Red
 Random.Range(0f, 1f), // Green
 Random.Range(0f, 1f), // Blue
 1 // Alpha (transparency)
		);
		int r = Random.Range(40, 70);
		GO.GetComponent<Text>().color = randomColor;
	/* GO.GetComponent<Text>().fontSize = r; */
	}

	//methode a ne pas utiliser pour les mots de passe car dans ceux-ci le caractere ' ' est compte comme un vrai caractere
	public string RemoveLastSpace(string mot)
	{
		//comme les string sont immutable, on doit passer par une autre string
		string modif = "";
		//on verifie que la string ne soit pas vide
		if (mot.Length > 1)
		{
			//on enleve tous les caracteres ' ' a la fin du mot
			modif = mot.TrimEnd();
			return modif;
		}
		else
			return mot;
	}
}