using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : Miscellaneous
{
	private Button _btnSon, _btnMusique;
	private AudioSource _soundCtrl, _musicCtrl;
	private Slider _soundSlider, _musicSlider;
	private Text _pourcentSon, _pourcentMusique;
	private float _previousSoundVol = 0.0f;
	private float _previousMusicVol = 0.0f;
	float lastSoundValue = 0;
	float lastMusicValue = 0;
	public Toggle toggle_french, toggle_english, toggle_german;
	private GameObject _btnSonUnselected, _btnMusicUnselected;
	private Transform optionsMenu, OCB, OCS, OCT; // Options Container Buttons
	private GameObject _btnSonUnselectedP, _btnMusicUnselectedP;
	private Button _btnSonP, _btnMusiqueP;
	private Transform panelMenu, PCB; // Options Container Buttons
	private Sprite _spriteMusicON, _spriteMusicOFF, _spriteSoundON, _spriteSoundOFF, _spriteMusicUnselectedON, _spriteMusicUnselectedOFF, _spriteSoundUnselectedON, _spriteSoundUnselectedOFF;
	// Start is called before the first frame update
	void Start()
	{
		// INITIALISATION
		panelMenu = GameObject.Find("SubMenus").transform.Find("Panel Options").transform;
		PCB = panelMenu.Find("Buttons").transform;
		_btnMusiqueP = PCB.Find("SwitchMusic").GetComponent<Button>();
		_btnMusicUnselectedP = _btnMusiqueP.transform.GetChild(0).gameObject;
		_btnSonP = PCB.Find("SwitchSound").GetComponent<Button>();
		_btnSonUnselectedP = _btnSonP.transform.GetChild(0).gameObject;


		optionsMenu = GameObject.Find("SubMenus").transform.Find("OptionsMenu").transform;
		OCB = optionsMenu.Find("Buttons").transform;
		OCS = optionsMenu.Find("Sliders").transform;
		OCT = optionsMenu.Find("Text").transform;
		_soundSlider = OCS.Find("Slider Son").GetComponent<Slider>();
		_musicSlider = OCS.Find("Slider Musique").GetComponent<Slider>();
		_pourcentSon = OCT.Find("Pourcent Son").GetComponent<Text>();
		_pourcentMusique = OCT.Find("Pourcent Musique").GetComponent<Text>();
		_btnMusique = OCB.Find("SwitchMusic").GetComponent<Button>();
		_btnMusicUnselected = _btnMusique.transform.GetChild(0).gameObject;
		_btnSon = OCB.Find("SwitchSound").GetComponent<Button>();
		_btnSonUnselected = _btnSon.transform.GetChild(0).gameObject;
		_soundCtrl = GameObject.Find("SoundController").GetComponent<AudioSource>();
		_musicCtrl = GameObject.Find("MusicController").GetComponent<AudioSource>();
		string pathBlue = "Miscellaneous/UI/Buttons/Moss_Blue/";
		string pathWhite = "Miscellaneous/UI/Buttons/Rock_white/";
		_spriteMusicON = Resources.Load<Sprite>(pathBlue + "button_moss_blue13");
		_spriteMusicOFF = Resources.Load<Sprite>(pathBlue + "button_moss_blue14");
		_spriteSoundON = Resources.Load<Sprite>(pathBlue + "button_moss_blue30");
		_spriteSoundOFF = Resources.Load<Sprite>(pathBlue + "button_moss_blue28");
		_spriteMusicUnselectedON = Resources.Load<Sprite>(pathWhite + "button_white13");
		_spriteMusicUnselectedOFF = Resources.Load<Sprite>(pathWhite + "button_white14");
		_spriteSoundUnselectedON = Resources.Load<Sprite>(pathWhite + "button_white30");
		_spriteSoundUnselectedOFF = Resources.Load<Sprite>(pathWhite + "button_white28");
		// Debug.Log(_spriteMusicON.name + _spriteMusicOFF.name + _spriteSoundON.name + _spriteSoundOFF.name);
		// Debug.Log(_spriteMusicUnselectedON.name + _spriteMusicUnselectedOFF.name + _spriteSoundUnselectedON.name + _spriteSoundUnselectedOFF.name);
		DefaultMusicSound();
		//Subscribe to the Slider event
		_soundSlider.onValueChanged.AddListener(SoundSliderCallBack);
		lastSoundValue = _soundSlider.value;
		_musicSlider.onValueChanged.AddListener(MusicSliderCallBack);
		lastMusicValue = _musicSlider.value;
	}

	public void ToggleValueChangedOM(Toggle curT)
	{
		if (curT.isOn)
			Debug.Log(curT.name);
	}

	//---------------------------- Music/Sound Begin ----------------------------//
	public void Volume(AudioSource ads, Text txt, Slider sb)
	{
		ads.volume = sb.value;
		txt.text = Mathf.RoundToInt(sb.value * 100) + "%";
	}

	public void DisplayVolume(int b, float value)
	{
		if (value > 0)
			if (b == 0)
			{
				_btnSon.GetComponent<Image>().sprite = _spriteSoundON;
				_btnSonUnselected.GetComponent<Image>().sprite = _spriteSoundUnselectedON;
				_btnSonP.GetComponent<Image>().sprite = _spriteSoundON;
				_btnSonUnselectedP.GetComponent<Image>().sprite = _spriteSoundUnselectedON;
			}
			else
			{
				_btnMusique.GetComponent<Image>().sprite = _spriteMusicON;
				_btnMusicUnselected.GetComponent<Image>().sprite = _spriteMusicUnselectedON;
				_btnMusiqueP.GetComponent<Image>().sprite = _spriteMusicON;
				_btnMusicUnselectedP.GetComponent<Image>().sprite = _spriteMusicUnselectedON;
			}
		else if (b == 0)
		{
			_btnSon.GetComponent<Image>().sprite = _spriteSoundOFF;
			_btnSonUnselected.GetComponent<Image>().sprite = _spriteSoundUnselectedOFF;
			_btnSonP.GetComponent<Image>().sprite = _spriteSoundOFF;
			_btnSonUnselectedP.GetComponent<Image>().sprite = _spriteSoundUnselectedOFF;
		}
		else
		{
			_btnMusique.GetComponent<Image>().sprite = _spriteMusicOFF;
			_btnMusicUnselected.GetComponent<Image>().sprite = _spriteMusicUnselectedOFF;
			_btnMusiqueP.GetComponent<Image>().sprite = _spriteMusicOFF;
			_btnMusicUnselectedP.GetComponent<Image>().sprite = _spriteMusicUnselectedOFF;
		}

		if (b == 0)
			Volume(_soundCtrl, _pourcentSon, _soundSlider);
		else
			Volume(_musicCtrl, _pourcentMusique, _musicSlider);
	}

	public void DefaultMusicSound()
	{
		_soundSlider.maxValue = _musicSlider.maxValue = 1;
		_soundCtrl.volume = _musicCtrl.volume = _soundSlider.value = _musicSlider.value = 0.4f;
		Volume(_soundCtrl, _pourcentSon, _soundSlider);
		Volume(_musicCtrl, _pourcentMusique, _musicSlider);
	}

	//Will be called when Scrollbar changes
	public void SoundSliderCallBack(float value)
	{
		DisplayVolume(0, value);
	}

	//Will be called when Scrollbar changes
	public void MusicSliderCallBack(float value)
	{
		DisplayVolume(1, value);
	}

	public void SwitchSound()
	{
		if (_soundSlider.value != 0)
		{
			_previousSoundVol = _soundCtrl.volume;
			_soundSlider.value = 0.0f;
			DisplayVolume(0, _soundSlider.value);
		}
		else
		{
			_soundSlider.value = _previousSoundVol;
			DisplayVolume(0, _previousSoundVol);
		}
	}

	public void SwitchMusic()
	{
		if (_musicSlider.value != 0)
		{
			_previousMusicVol = _musicCtrl.volume;
			_musicSlider.value = 0.0f;
			DisplayVolume(1, _musicSlider.value);
		}
		else
		{
			_musicSlider.value = _previousMusicVol;
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