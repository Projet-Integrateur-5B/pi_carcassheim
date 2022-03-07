/*using System.Collections;
using System.Collections.Generic;*/
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OptionsMenu : Miscellaneous, IPointerEnterHandler, IPointerExitHandler
{
	private Button _btnSon;
	private Button _btnMusique;
	private AudioSource _soundCtrl;
	private AudioSource _musicCtrl;
	private Scrollbar _soundScroll;
	private Scrollbar _musicScroll;
	private Text _pourcentSon;
	private Text _pourcentMusique;
	private float _previousSoundVol = 0.0f;
	private float _previousMusicVol = 0.0f;
	private static bool s_tmpOnce = true;
	float lastSoundValue = 0;
	float lastMusicValue = 0;

	//////////////////////////////////////////////////////////////////////

	public Button button1;
	public Button button2;
	public Button button3;
	public Button button4;
	public Button button5;
	public Button button6;

	public Toggle toggle_french;
	public Toggle toggle_english;
	public Toggle toggle_german;

	private GameObject currentGo;
	private Color _previousColor;
	private Text _btnText;

	EventSystem eventSystem;

	// Start is called before the first frame update
	void Start()
	{
		button1.Select();
		_btnSon = FindGOTool("OptionsMenu", "Btn Son").GetComponent<Button>();
		_btnMusique = FindGOTool("OptionsMenu", "Btn Musique").GetComponent<Button>();
		_soundCtrl = GameObject.Find("SoundController").GetComponent<AudioSource>();
		_musicCtrl = GameObject.Find("MusicController").GetComponent<AudioSource>();
		_soundScroll = FindGOTool("OptionsMenu", "Scrollbar Son").GetComponent<Scrollbar>();
		_musicScroll = FindGOTool("OptionsMenu", "Scrollbar Musique").GetComponent<Scrollbar>();
		_pourcentSon = FindGOTool("OptionsMenu", "Pourcent Son").GetComponent<Text>();
		_pourcentMusique = FindGOTool("OptionsMenu", "Pourcent Musique").GetComponent<Text>();
		DefaultMusicSound();
		//Subscribe to the Scrollbar event
		_soundScroll.onValueChanged.AddListener(SoundScrollbarCallBack);
		lastSoundValue = _soundScroll.value;
		_musicScroll.onValueChanged.AddListener(MusicScrollbarCallBack);
		lastMusicValue = _musicScroll.value;
	}

	void Update()
	{
		Debug.Log(eventSystem.currentSelectedGameObject.name);
	}


	void OnEnable()
	{
		//Fetch the current EventSystem. Make sure your Scene has one.
		eventSystem = EventSystem.current;

		//Register Button Events
		button1 = GameObject.Find("Btn Retour Opt").GetComponent<Button>();
		button1.onClick.AddListener(() => buttonCallBack(button1));
		button1.Select();
		button2 = GameObject.Find("Btn Son").GetComponent<Button>();
		button2.onClick.AddListener(() => buttonCallBack(button2));
		button3 = GameObject.Find("Btn Musique").GetComponent<Button>();
		button3.onClick.AddListener(() => buttonCallBack(button3));
		button4 = GameObject.Find("Btn Fenêtré").GetComponent<Button>();
		button4.onClick.AddListener(() => buttonCallBack(button4));
		button5 = GameObject.Find("Btn Aide").GetComponent<Button>();
		button5.onClick.AddListener(() => buttonCallBack(button5));
		button6 = GameObject.Find("Btn Credits").GetComponent<Button>();
		button6.onClick.AddListener(() => buttonCallBack(button6));

		toggle_french = GameObject.Find("Toggle French").GetComponent<Toggle>();
		toggle_french.onValueChanged.AddListener(delegate {ToggleValueChanged(toggle_french);});
		toggle_english = GameObject.Find("Toggle English").GetComponent<Toggle>();
		toggle_english.onValueChanged.AddListener(delegate { ToggleValueChanged(toggle_english); });
		toggle_german = GameObject.Find("Toggle German").GetComponent<Toggle>();
		toggle_german.onValueChanged.AddListener(delegate { ToggleValueChanged(toggle_german); });
	}


	void ToggleValueChanged(Toggle change)
	{
		if(change == toggle_french && change.isOn)
        {
			Debug.Log("French");
		}

		if (change == toggle_english && change.isOn)
		{
			Debug.Log("English");
		}

		if (change == toggle_german && change.isOn)
		{
			Debug.Log("German");
		}
		GameObject.Find("SoundController").GetComponent<AudioSource>().Play();
	}

	private void buttonCallBack(Button buttonPressed)
	{
		//teste si le bouton va changer de menu
		bool btn_change_menu = false;

		if (buttonPressed == button1)
		{
			Debug.Log("Clicked: " + button1.name);
			HideOptions();
			btn_change_menu = true;
		}

		if (buttonPressed == button2)
		{
			Debug.Log("Clicked: " + button2.name);
			SwitchSound();
			btn_change_menu = false;
		}

		if (buttonPressed == button3)
		{
			Debug.Log("Clicked: " + button3.name);
			SwitchMusic();
			btn_change_menu = false;
		}

		if (buttonPressed == button4)
		{
			Debug.Log("Clicked: " + button4.name);
			FullScreen();
			btn_change_menu = false;
		}

		if (buttonPressed == button5)
		{
			Debug.Log("Clicked: " + button5.name);
			Help();
			btn_change_menu = false;
		}

		if (buttonPressed == button6)
		{
			Debug.Log("Clicked: " + button6.name);
			ShowCredits();
			btn_change_menu = true;
		}
		if (currentGo != null)
		{
			if (currentGo.GetComponent<Button>() && btn_change_menu)
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
		button3.onClick.RemoveAllListeners();
		button4.onClick.RemoveAllListeners();
		button5.onClick.RemoveAllListeners();
		button6.onClick.RemoveAllListeners();
	}

	public void HideOptions()
	{
		ChangeMenu(FindMenu("OptionsMenu"), FindMenu("HomeMenu"));
	}

	public void ShowCredits()
	{
		ChangeMenu(FindMenu("OptionsMenu"), FindMenu("CreditsMenu"));
	}

	public void FlagsToggle() //affiche la langue du toggle enclenche
	{
		// foreach
		if (GameObject.Find("Toggle French").GetComponent<Toggle>().isOn == true)
		{
			Debug.Log("French");
		}
		else if (GameObject.Find("Toggle English").GetComponent<Toggle>().isOn == true)
		{
			Debug.Log("English");
		}
		else if (GameObject.Find("Toggle German").GetComponent<Toggle>().isOn == true)
		{
			Debug.Log("German");
		}
	}

	//---------------------------- Music/Sound Begin ----------------------------//
	public void VolumeSound(float value)
	{
		_soundCtrl.volume = value;
		_pourcentSon.text = Mathf.RoundToInt(value * 100) + "%";
	}

	public void VolumeMusic(float value)
	{
		_musicCtrl.volume = value;
		_pourcentMusique.text = Mathf.RoundToInt(value * 100) + "%";
	}

	public void DisplayVolumeSound(float value)
	{
		if (value > 0 /* && soundCtrl.isPlaying */)
		{
			_btnSon.GetComponentInChildren<Text>().text = "Son 'ON'";
		}
		else
		{
			_btnSon.GetComponentInChildren<Text>().text = "Son 'OFF'";
		}

		VolumeSound(value);
	}

	public void DisplayVolumeMusic(float value)
	{
		if (value > 0)
		{
			_btnMusique.GetComponentInChildren<Text>().text = "Musique 'ON'";
		}
		else
		{
			_btnMusique.GetComponentInChildren<Text>().text = "Musique 'OFF'";
		}

		VolumeMusic(value);
	}

	public void DefaultMusicSound()
	{
		if (s_tmpOnce == true)
		{
			_soundScroll.numberOfSteps = _musicScroll.numberOfSteps = 11; // 0->10 = 11
			_soundCtrl.volume = _musicCtrl.volume = _soundScroll.value = _musicScroll.value = 0.2f;
			VolumeSound(_soundScroll.value);
			VolumeMusic(_musicScroll.value);
			s_tmpOnce = !s_tmpOnce;
		}
	}

	//Will be called when Scrollbar changes
	public void SoundScrollbarCallBack(float value)
	{
		/*  if (lastSoundValue > value)
        Debug.Log("Scrolling UP: " + value);
    else
        Debug.Log("Scrolling DOWN: " + value);
*/
		DisplayVolumeSound(value);
	/* lastSoundValue = value; */
	}

	//Will be called when Scrollbar changes
	public void MusicScrollbarCallBack(float value)
	{
		DisplayVolumeMusic(value);
	}

	/* public void OnDisable()
{
    //Un-Subscribe To Scrollbar Event
    soundScroll.onValueChanged.RemoveListener(SoundScrollbarCallBack);
	musicScroll.onValueChanged.RemoveListener(MusicScrollbarCallBack);
} */
	public void SwitchSound()
	{
		if (_soundScroll.value != 0)
		{
			_previousSoundVol = _soundCtrl.volume;
			_soundScroll.value = 0.0f;
			SoundScrollbarCallBack(_soundScroll.value);
		}
		else
		{
			_soundScroll.value = _previousSoundVol;
			SoundScrollbarCallBack(_previousSoundVol);
		}
	}

	public void SwitchMusic()
	{
		if (_musicScroll.value != 0)
		{
			_previousMusicVol = _musicCtrl.volume;
			_musicScroll.value = 0.0f;
			MusicScrollbarCallBack(_musicScroll.value);
		}
		else
		{
			_musicScroll.value = _previousMusicVol;
			MusicScrollbarCallBack(_previousMusicVol);
		}
	}

	// -------------- Music/Sound End -----------------------//
	public void FullScreen()
	{
		Screen.fullScreen = !Screen.fullScreen;
		Debug.Log("Windowed");
	}

	public void Help()
	{
		Application.OpenURL("https://tinyurl.com/SlapDance");
	}
}