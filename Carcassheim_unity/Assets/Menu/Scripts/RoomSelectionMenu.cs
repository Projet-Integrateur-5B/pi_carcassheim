using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RoomSelectionMenu : Miscellaneous, IPointerEnterHandler, IPointerExitHandler
{
    public Button button1;
    public Button button2;

    private GameObject currentGo;
    private Color _previousColor;
    private Text _btnText;

    public GameObject Pop_up_Options;
    public static bool s_isOpenPanel = false;

    EventSystem eventSystem;

    // Start is called before the first frame update
    void Start()
    {
        button1.Select();
        Pop_up_Options = FindMenu("Panel Options");
    }

    void Update()
    {
        Debug.Log(eventSystem.currentSelectedGameObject.name);
    }


    void OnEnable()
    {
        //Fetch the current EventSystem. Make sure your Scene has one.
        eventSystem = EventSystem.current;
        button1 = GameObject.Find("Btn Retour RoomSelection").GetComponent<Button>();
        button1.onClick.AddListener(() => buttonCallBack(button1));
        button1.Select();
        button2 = GameObject.Find("Btn Options Pop-Up").GetComponent<Button>();
        button2.onClick.AddListener(() => buttonCallBack(button2));
    }

    private void buttonCallBack(Button buttonPressed)
    {
        if (buttonPressed == button1)
        {
            Debug.Log("Clicked: " + button1.name);
            HideRoomSelection();
        }

        if (buttonPressed == button2)
        {
            Debug.Log("Clicked: " + button2.name);
            ShowPopUpOptions();
        }

        if (currentGo != null)
        {
            if (currentGo.GetComponent<Button>())
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
            _previousColor = _btnText.color;
            TryColorText(_btnText, Color.blue, "#1e90ff");
            _btnText.fontSize += 3;
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Mouse Exit " + currentGo.name);
        if (currentGo.GetComponent<Button>())
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
    }

	public void HideRoomSelection()
	{
		s_isOpenPanel = false;
		Pop_up_Options.SetActive(s_isOpenPanel);
		ChangeMenu(FindMenu("RoomSelectionMenu"), FindMenu("HomeMenu"));
	}

	public void ShowPopUpOptions()
	{
		s_isOpenPanel = !s_isOpenPanel;
		Pop_up_Options.SetActive(s_isOpenPanel);
	}

}
