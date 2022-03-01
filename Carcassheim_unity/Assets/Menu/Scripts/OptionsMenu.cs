/*using System.Collections;
using System.Collections.Generic;*/
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OptionsMenu : Miscellaneous
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
	// Start is called before the first frame update
	void Start()
	{
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

	void Awake()
	{
	}

	// Update is called once per frame
	void Update()
	{
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