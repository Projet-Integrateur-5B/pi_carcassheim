using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Miscellaneous : MonoBehaviour {
    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() { }

    public GameObject FindMenu(string menu)
    {
        return GameObject.Find("SubMenus").transform.Find(menu).gameObject;
    }

    public void tryColorText(Text change, Color defaultColor, string coloration)
    {
        Color newCol;

        if (ColorUtility.TryParseHtmlString(coloration, out newCol)) {
            change.color = newCol;
        } else {
            change.color = defaultColor;
        }
    }

    public void tryColor(
        GameObject change, Color defaultColor, string coloration)
    {
        Color newCol;

        if (ColorUtility.TryParseHtmlString(coloration, out newCol)) {
            change.GetComponent<Text>().color = newCol;
        } else {
            change.GetComponent<Text>().color = defaultColor;
        }
    }

    public void changeMenu(GameObject close, GameObject goTo)
    {
        close.SetActive(false);
        goTo.SetActive(true);
    }

    public bool StrCompare(string str1, string str2)
    {
        return (str2.Equals(str1));
    }

    public void randomIntColor(GameObject GO)
    {
        Color randomColor = new Color(Random.Range(0f, 1f), // Red
            Random.Range(0f, 1f), // Green
            Random.Range(0f, 1f), // Blue
            1 // Alpha (transparency)
        );

        int r = Random.Range(40, 70);
        GO.GetComponent<Text>().color = randomColor;
        GO.GetComponent<Text>().fontSize = r;
    }
}