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

    [Space(20)]

    public ParticleSystem clickParticleSystem;

    private Vector3 m_desiredPosition;
    public float leftMovementCanvas;

    FaderScript m_fader;

    public RectTransform m_scenesContainer;

    public Transform m_levelsContainer;

    public GameObject m_winPanel, m_startLevelPanel;

    public bool m_debugMode;

    private Color m_mainColor;
    private Camera m_cam;

    private void Start()
    {
        m_cam = Camera.main;
        m_mainColor = m_cam.backgroundColor;
        m_fader = FindObjectOfType<FaderScript>();
        UpdateMenuButtons();
        if (m_mainAudioSource != null) m_mainAudioSource = GetComponent<AudioSource>();
        m_mainAudioSource.clip = m_menuSong;
        m_mainAudioSource.Play();
        m_winPanel.SetActive(false);
        m_startLevelPanel.SetActive(false);
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
        m_cam.backgroundColor = m_animalColor;
        //}
    }

    public void OnClickColors()
    {
        //if (m_fader.fadeDone)
        //{
        Navigate(1);
        Instantiate(m_colorLevel);
        m_cam.backgroundColor = m_colorColor;
        //}
    }

    public void OnClcikClothes()
    {
        //if (m_fader.fadeDone)
        //{
        Navigate(1);
        Instantiate(m_clothLevel);
        m_cam.backgroundColor = m_clothColor;
        //}
    }

    public void OnClickNumbers()
    {
        //if (m_fader.fadeDone)
        //{
        Navigate(1);
        Instantiate(m_numberLevel);
        m_cam.backgroundColor = m_numberColor;
        //}
    }

    public void OnClickMenu()
    {
        Navigate(0);
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

    public void Navigate(int scene)
    {
        switch (scene)
        {
            case 0: //menu
                UpdateMenuButtons();

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
        }
    }

    public void UpdateMenuButtons()
    {
        if (!m_debugMode)
        {
            int i = 0;

            foreach (Transform t in m_levelsContainer)
            {
                int index = i;

                Button b = t.GetComponent<Button>();

                //nivel desbloqueado?
                if (i <= SaveManager.Instance.m_saveState.m_levelCompleted)
                {
                    //debloqueado, completado?
                    if (i < SaveManager.Instance.m_saveState.m_levelCompleted)
                    {
                        //completado

                    }
                }
                //no desbloqueado
                else
                {
                    b.interactable = false;
                }

                i++;
            }
        }

        else
        {
            int i = 0;

            foreach (Transform t in m_levelsContainer)
            {
                int index = i;

                Button b = t.GetComponent<Button>();

                b.interactable = true;

                i++;
            }
        }
    }

    public void EndLevel()
    {
        LevelScript ls = FindObjectOfType<LevelScript>();
        if (ls != null) Destroy(ls.gameObject);

        m_winPanel.SetActive(true);
        m_otherAudioSource.Play();
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
