using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;

public class HomeMenu : Miscellaneous, IPointerEnterHandler, IPointerExitHandler
{
    public Button button1;
    public Button button2;
    public Button button3;
    public Button button4;
    public Button button5;

    private GameObject currentGo;
    private Color _previousColor;
    private Text _btnText;

    EventSystem eventSystem;

    static private GameObject selectedGo;

    void Start()
    {
        button5.Select();
    }

    void Update()
    {
        if (eventSystem.currentSelectedGameObject != null)
        {
            selectedGo = eventSystem.currentSelectedGameObject;
            Debug.Log(selectedGo);
        }


    }

    void OnEnable()
    {
        //Fetch the current EventSystem. Make sure your Scene has one.
        eventSystem = EventSystem.current;

        Color newCol;
        if (GetState() == false && FindMenu("HomeMenu").activeSelf == true)
        {
            FindGOTool("HomeMenu", "Btn Jouer").GetComponent<Button>().interactable = GetState();
            FindGOTool("HomeMenu", "Btn Statistiques").GetComponent<Button>().interactable = GetState();
            ColorUtility.TryParseHtmlString("#808080", out newCol);
            GameObject.Find("Btn Jouer").GetComponent<Button>().GetComponentInChildren<Text>().color = newCol;
            GameObject.Find("Btn Statistiques").GetComponent<Button>().GetComponentInChildren<Text>().color = newCol;
            button1 = GameObject.Find("Btn Connexion").GetComponent<Button>();
            button1.onClick.AddListener(() => buttonCallBack(button1));
        }
        button2 = GameObject.Find("Btn Jouer").GetComponent<Button>();
        button2.onClick.AddListener(() => buttonCallBack(button2));
        button3 = GameObject.Find("Btn Options").GetComponent<Button>();
        button3.onClick.AddListener(() => buttonCallBack(button3));
        button4 = GameObject.Find("Btn Statistiques").GetComponent<Button>();
        button4.onClick.AddListener(() => buttonCallBack(button4));
        button5 = GameObject.Find("Btn Quitter le jeu").GetComponent<Button>();
        button5.onClick.AddListener(() => buttonCallBack(button5));
        button5.Select();
    }

    private void buttonCallBack(Button buttonPressed)
    {
        if (buttonPressed == button1)
        {
            Debug.Log("Clicked: " + button1.name);
            ShowConnection();
        }

        if (buttonPressed == button2)
        {
            Debug.Log("Clicked: " + button2.name);
            Jouer();
        }

        if (buttonPressed == button3)
        {
            Debug.Log("Clicked: " + button3.name);
            ShowOptions();
        }

        if (buttonPressed == button4)
        {
            Debug.Log("Clicked: " + button4.name);
            ShowStatistiques();
        }

        if (buttonPressed == button5)
        {
            Debug.Log("Clicked: " + button5.name);
            Quitter();
        }

        if (currentGo != null)
        {
            bool tmpBool = StrCompare(currentGo.name, "Btn Jouer") || StrCompare(currentGo.name, "Btn Statistiques");
            if ((GetState() || !tmpBool) && currentGo.GetComponent<Button>())
            {
                _btnText.color = _previousColor;
                _btnText.fontSize -= 3;
            }
        }
        GameObject.Find("SoundController").GetComponent<AudioSource>().Play();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        currentGo = eventData.pointerCurrentRaycast.gameObject.transform.parent.gameObject;
        Debug.Log("Mouse Enter " + currentGo.name);
        if (currentGo.GetComponent<Button>())
        {
            _btnText = currentGo.GetComponentInChildren<Text>();
            if (!GetState() && (StrCompare(currentGo.name, "Btn Jouer") || StrCompare(currentGo.name, "Btn Statistiques")))
            {
                TryColorText(_btnText, Color.grey, "#808080");
            }
            else
            {
                _previousColor = _btnText.color;
                TryColorText(_btnText, Color.blue, "#1e90ff");
                _btnText.fontSize += 3;
            }
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Mouse Exit " + currentGo.name);
        bool tmpBool = StrCompare(currentGo.name, "Btn Jouer") || StrCompare(currentGo.name, "Btn Statistiques");
        if ((GetState() || !tmpBool) && currentGo.GetComponent<Button>())
        {
            _btnText.color = _previousColor;
            _btnText.fontSize -= 3;
        }
    }

    void OnDisable()
    {
        //Un-Register Button Events
        button1.onClick.RemoveAllListeners();
        button2.onClick.RemoveAllListeners();
        button3.onClick.RemoveAllListeners();
        button4.onClick.RemoveAllListeners();
        button5.onClick.RemoveAllListeners();
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
