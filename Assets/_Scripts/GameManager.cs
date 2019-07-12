using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Audiosources")]
    public AudioSource m_mainAudioSource;
    public AudioSource m_otherAudioSource;

    [Header("Clips")]
    public AudioClip m_menuSong;
    public AudioClip m_gameSong;
    public AudioClip m_startLevelSound;

    [Header("Prefabs of level controllers")]
    public LevelScript m_animalLevel;
    public LevelScript m_colorLevel;
    public LevelScript m_clothLevel;
    public LevelScript m_numberLevel;

    [Header("Level Colors")]
    public Color m_animalColor;
    public Color m_colorColor;
    public Color m_clothColor;
    public Color m_numberColor;

    [Header("Collectibles List")]
    public List<Button> m_collectiblesAnimals = new List<Button>();
    public List<Button> m_collectiblesClothes = new List<Button>();
    public List<Button> m_collectiblesColors = new List<Button>();
    public List<Button> m_collectiblesNumbers = new List<Button>();


    public enum LevelType
    {
        none,
        animals,
        clothes,
        colors,
        numbers

    }

    public LevelType m_currentLevelType = LevelType.none;

    [Space(20)]
    public ParticleSystem clickParticleSystem;

    private Vector3 m_desiredPosition;
    public float leftMovementCanvas;

    FaderScript m_fader;

    public RectTransform m_scenesContainer;

    public Transform m_levelsContainer;

    public GameObject m_winPanel, m_startLevelPanel;

    public bool m_debugMode;
    public bool m_restartOnPlay;

    private Color m_mainColor;
    private Camera m_cam;

    private void Start()
    {
        m_cam = Camera.main;
        m_mainColor = m_cam.backgroundColor;
        m_fader = FindObjectOfType<FaderScript>();
        UpdateCollectableButtons();
        if (m_mainAudioSource != null) m_mainAudioSource = GetComponent<AudioSource>();
        m_mainAudioSource.clip = m_menuSong;
        m_mainAudioSource.Play();
        m_winPanel.SetActive(false);
        m_startLevelPanel.SetActive(false);

        if (m_restartOnPlay) SaveManager.Instance.ResetSave();
    }

    private void Update()
    {
        m_scenesContainer.anchoredPosition3D = Vector3.Lerp(m_scenesContainer.anchoredPosition3D, m_desiredPosition, 0.1f);

        if (m_winPanel.activeInHierarchy)
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (Input.touchCount > 0 && Input.touchCount < 2)
                {
                    if (Input.GetTouch(0).phase == TouchPhase.Began)
                    {
                        m_winPanel.SetActive(false);
                        m_cam.backgroundColor = m_mainColor;
                        Navigate(0);
                    }
                }
            }
            else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    m_winPanel.SetActive(false);
                    m_cam.backgroundColor = m_mainColor;
                    Navigate(0);
                }
            }
        }
    }

    public void OnClickAnimals()
    {
        //if (m_fader.fadeDone)
        //{
        Navigate(1);
        Instantiate(m_animalLevel);
        m_currentLevelType = LevelType.animals;
        m_cam.backgroundColor = m_animalColor;
        //}
    }

    public void OnClickColors()
    {
        //if (m_fader.fadeDone)
        //{
        Navigate(1);
        Instantiate(m_colorLevel);
        m_currentLevelType = LevelType.colors;
        m_cam.backgroundColor = m_colorColor;
        //}
    }

    public void OnClcikClothes()
    {
        //if (m_fader.fadeDone)
        //{
        Navigate(1);
        Instantiate(m_clothLevel);
        m_currentLevelType = LevelType.clothes;
        m_cam.backgroundColor = m_clothColor;
        //}
    }

    public void OnClickNumbers()
    {
        //if (m_fader.fadeDone)
        //{
        Navigate(1);
        Instantiate(m_numberLevel);
        m_currentLevelType = LevelType.numbers;
        m_cam.backgroundColor = m_numberColor;
        //}
    }

    public void OnClickMenu()
    {
        Navigate(0);
        m_currentLevelType = LevelType.none;
        m_cam.backgroundColor = m_mainColor;
    }

    public void OnClickReplay()
    {
        LevelScript ls = FindObjectOfType<LevelScript>();

        if (ls.m_currentState == LevelScript.levelState.playing)
        {
            ls.m_audioSource.Stop();
            ls.m_currentState = LevelScript.levelState.showing;
        }

        if (ls.m_lastClip != null) StartCoroutine(ls.PlaySound(ls.m_lastClip, false));
    }

    public void OnClickCollectables()
    {
        Navigate(2);
        m_currentLevelType = LevelType.none;
    }

    public void Navigate(int scene)
    {
        switch (scene)
        {
            case 0: //menu

                m_desiredPosition = Vector3.zero;

                LevelScript ls = FindObjectOfType<LevelScript>();

                if (ls != null) Destroy(ls.gameObject);

                m_otherAudioSource.Stop();
                m_mainAudioSource.clip = m_menuSong;
                m_mainAudioSource.Play();

                break;
            case 1: //game
                m_desiredPosition = Vector3.left * leftMovementCanvas; //1280

                m_mainAudioSource.clip = m_gameSong;
                m_mainAudioSource.Play();
                break;
            case 2: //collectables
                UpdateCollectableButtons();
                m_desiredPosition = Vector3.down * leftMovementCanvas;
                break;
        }
    }

    public void UpdateCollectableButtons()
    {
        if (!m_debugMode)
        {
            //DESBLOQUEAR CARTAS ANIMALES
            if(!SaveManager.Instance.m_saveState.m_animalsCompleted)
            {
                foreach (Button b in m_collectiblesAnimals)
                {
                        b.interactable = false;
                }
            }

            else
                foreach (Button b in m_collectiblesAnimals)
                {
                    b.interactable = true;
                }

            //DESBLOQUEAR CARTAS ROPA
            if (!SaveManager.Instance.m_saveState.m_clothesCompleted)
            {
                foreach (Button b in m_collectiblesClothes)
                {
                    b.interactable = false;
                }
            }

            else
                foreach (Button b in m_collectiblesClothes)
                {
                    b.interactable = true;
                }

            //DESBLOQUEAR CARTAS COLORES
            if (!SaveManager.Instance.m_saveState.m_colorsCompleted)
            {
                foreach (Button b in m_collectiblesColors)
                {
                    b.interactable = false;
                }
            }

            else
                foreach (Button b in m_collectiblesColors)
                {
                    b.interactable = true;
                }

            //DESBLOQUEAR CARTAS NUMEROS
            if (!SaveManager.Instance.m_saveState.m_numbersCompleted)
            {
                foreach (Button b in m_collectiblesNumbers)
                {
                    b.interactable = false;
                }
            }

            else
                foreach (Button b in m_collectiblesNumbers)
                {
                    b.interactable = true;
                }


        }
    }

    public void EndLevel()
    {
        LevelScript ls = FindObjectOfType<LevelScript>();
        if (ls != null) Destroy(ls.gameObject);

        m_winPanel.SetActive(true);
        m_otherAudioSource.Play();

        SaveManager.Instance.LevelCompleted(m_currentLevelType.ToString());
    }

    public IEnumerator StartLevel()
    {
        LevelScript ls = FindObjectOfType<LevelScript>();
        m_startLevelPanel.SetActive(true);
        ls.m_audioSource.PlayOneShot(m_startLevelSound);
        yield return new WaitForSeconds(m_startLevelSound.length + 0.5f);
        m_startLevelPanel.SetActive(false);
        ls.PlayRandomSound();
    }
}
