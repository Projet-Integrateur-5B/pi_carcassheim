using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : Miscellaneous
{
	private Button _btnSon, _btnMusique;
	private AudioSource _soundCtrl, _musicCtrl;
	private Scrollbar _soundScroll, _musicScroll;
	private Text _pourcentSon, _pourcentMusique;
	private float _previousSoundVol = 0.0f;
	private float _previousMusicVol = 0.0f;
	float lastSoundValue = 0;
	float lastMusicValue = 0;
	public Toggle toggle_french, toggle_english, toggle_german;
	private GameObject containerButtons;
	private Transform optionsMenu, OCB; // Options Container Buttons
	// Start is called before the first frame update
	void Start()
	{
		// INITIALISATION
		optionsMenu = GameObject.Find("SubMenus").transform.Find("OptionsMenu").transform;
		OCB = optionsMenu.Find("Buttons").transform;
		_soundScroll = optionsMenu.Find("Scrollbar Son").GetComponent<Scrollbar>();
		_musicScroll = optionsMenu.Find("Scrollbar Musique").GetComponent<Scrollbar>();
		_pourcentMusique = optionsMenu.Find("Pourcent Musique").GetComponent<Text>();
		_pourcentSon = optionsMenu.Find("Pourcent Son").GetComponent<Text>();
		_btnMusique = OCB.Find("SwitchMusic").GetComponent<Button>();
		_btnSon = OCB.Find("SwitchSound").GetComponent<Button>();
		_soundCtrl = GameObject.Find("SoundController").GetComponent<AudioSource>();
		_musicCtrl = GameObject.Find("MusicController").GetComponent<AudioSource>();
		DefaultMusicSound();
		//Subscribe to the Scrollbar event
		_soundScroll.onValueChanged.AddListener(SoundScrollbarCallBack);
		lastSoundValue = _soundScroll.value;
		_musicScroll.onValueChanged.AddListener(MusicScrollbarCallBack);
		lastMusicValue = _musicScroll.value;
	/*	toggle_french = GameObject.Find("Toggle French").GetComponent<Toggle>();
		toggle_french.onValueChanged.AddListener(delegate
		{
			ToggleValueChanged(toggle_french);
		}); */
	}

	void ToggleValueChanged(Toggle change)
	{
		// PATCH : à faire (french/english/german)
		if (change == toggle_french && change.isOn)
			Debug.Log("French");
		GameObject.Find("SoundController").GetComponent<AudioSource>().Play();
	}

	//---------------------------- Music/Sound Begin ----------------------------//
	public void Volume(AudioSource ads, Text txt, Scrollbar sb)
	{
		ads.volume = sb.value;
		txt.text = Mathf.RoundToInt(sb.value * 100) + "%";
	}

	public void DisplayVolume(int b, float value)
	{
		if (value > 0)
			if (b == 0)
				_btnSon.GetComponentInChildren<Text>().text = "Son 'ON'";
			else
				_btnMusique.GetComponentInChildren<Text>().text = "Musique 'ON'";
		else if (b == 0)
			_btnSon.GetComponentInChildren<Text>().text = "Son 'OFF'";
		else
			_btnMusique.GetComponentInChildren<Text>().text = "Musique 'OFF'";
		if (b == 0)
			Volume(_soundCtrl, _pourcentSon, _soundScroll);
		else
			Volume(_musicCtrl, _pourcentMusique, _musicScroll);
	}

	public void DefaultMusicSound()
	{
		_soundScroll.numberOfSteps = _musicScroll.numberOfSteps = 11; // 0->10 = 11
		_soundCtrl.volume = _musicCtrl.volume = _soundScroll.value = _musicScroll.value = 0.2f;
		Volume(_soundCtrl, _pourcentSon, _soundScroll);
		Volume(_musicCtrl, _pourcentMusique, _musicScroll);
	}

	//Will be called when Scrollbar changes
	public void SoundScrollbarCallBack(float value)
	{
		DisplayVolume(0, value);
	}

	//Will be called when Scrollbar changes
	public void MusicScrollbarCallBack(float value)
	{
		DisplayVolume(1, value);
	}

	public void SwitchSound()
	{
		if (_soundScroll.value != 0)
		{
			_previousSoundVol = _soundCtrl.volume;
			_soundScroll.value = 0.0f;
			DisplayVolume(0, _soundScroll.value);
		}
		else
		{
			_soundScroll.value = _previousSoundVol;
			DisplayVolume(0, _previousSoundVol);
		}
	}

	public void SwitchMusic()
	{
		if (_musicScroll.value != 0)
		{
			_previousMusicVol = _musicCtrl.volume;
			_musicScroll.value = 0.0f;
			DisplayVolume(1, _musicScroll.value);
		}
		else
		{
			_musicScroll.value = _previousMusicVol;
			DisplayVolume(1, _previousMusicVol);
		}
	}

	// -------------- Music/Sound End -----------------------//
	public void HideOptions()
	{
		ChangeMenu("OptionsMenu", "HomeMenu");
	}

	public void FullScreen()
	{
		Screen.fullScreen = !Screen.fullScreen;
	}

	public void Help()
	{
		Application.OpenURL("https://tinyurl.com/SlapDance");
	}

	public void ShowCredits()
	{
		ChangeMenu("OptionsMenu", "CreditsMenu");
	}
}