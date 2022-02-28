/*using System.Collections;
using System.Collections.Generic;*/
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OptionsMenu : Miscellaneous
{
	private Button btnSon;
	private Button btnMusique;
	private AudioSource soundCtrl;
	private AudioSource musicCtrl;
	private Scrollbar soundScroll;
	private Scrollbar musicScroll;
	private Text pourcentSon;
	private Text pourcentMusique;
	private float previousSoundVol = 0.0f;
	private float previousMusicVol = 0.0f;
	private static bool tmpOnce = true;
	float lastSoundValue = 0;
	float lastMusicValue = 0;
	// Start is called before the first frame update
	void Start()
	{
		btnSon = FindGOTool("OptionsMenu", "Btn Son").GetComponent<Button>();
		btnMusique = FindGOTool("OptionsMenu", "Btn Musique").GetComponent<Button>();
		soundCtrl = GameObject.Find("SoundController").GetComponent<AudioSource>();
		musicCtrl = GameObject.Find("MusicController").GetComponent<AudioSource>();
		soundScroll = FindGOTool("OptionsMenu", "Scrollbar Son").GetComponent<Scrollbar>();
		musicScroll = FindGOTool("OptionsMenu", "Scrollbar Musique").GetComponent<Scrollbar>();
		pourcentSon = FindGOTool("OptionsMenu", "Pourcent Son").GetComponent<Text>();
		pourcentMusique = FindGOTool("OptionsMenu", "Pourcent Musique").GetComponent<Text>();
		DefaultMusicSound();
		//Subscribe to the Scrollbar event
		soundScroll.onValueChanged.AddListener(SoundScrollbarCallBack);
		lastSoundValue = soundScroll.value;
		musicScroll.onValueChanged.AddListener(MusicScrollbarCallBack);
		lastMusicValue = musicScroll.value;
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
		soundCtrl.volume = value;
		pourcentSon.text = Mathf.RoundToInt(value * 100) + "%";
	}

	public void VolumeMusic(float value)
	{
		musicCtrl.volume = value;
		pourcentMusique.text = Mathf.RoundToInt(value * 100) + "%";
	}

	public void DisplayVolumeSound(float value)
	{
		if (value > 0 /* && soundCtrl.isPlaying */)
		{
			btnSon.GetComponentInChildren<Text>().text = "Son 'ON'";
		}
		else
		{
			btnSon.GetComponentInChildren<Text>().text = "Son 'OFF'";
		}

		VolumeSound(value);
	}

	public void DisplayVolumeMusic(float value)
	{
		if (value > 0)
		{
			btnMusique.GetComponentInChildren<Text>().text = "Musique 'ON'";
		}
		else
		{
			btnMusique.GetComponentInChildren<Text>().text = "Musique 'OFF'";
		}

		VolumeMusic(value);
	}

	public void DefaultMusicSound()
	{
		if (tmpOnce == true)
		{
			soundScroll.numberOfSteps = musicScroll.numberOfSteps = 11; // 0->10 = 11
			soundCtrl.volume = musicCtrl.volume = soundScroll.value = musicScroll.value = 0.2f;
			VolumeSound(soundScroll.value);
			VolumeMusic(musicScroll.value);
			tmpOnce = !tmpOnce;
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
		if (soundScroll.value != 0)
		{
			previousSoundVol = soundCtrl.volume;
			soundScroll.value = 0.0f;
			SoundScrollbarCallBack(soundScroll.value);
		}
		else
		{
			soundScroll.value = previousSoundVol;
			SoundScrollbarCallBack(previousSoundVol);
		}
	}

	public void SwitchMusic()
	{
		if (musicScroll.value != 0)
		{
			previousMusicVol = musicCtrl.volume;
			musicScroll.value = 0.0f;
			MusicScrollbarCallBack(musicScroll.value);
		}
		else
		{
			musicScroll.value = previousMusicVol;
			MusicScrollbarCallBack(previousMusicVol);
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