using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public AudioSource m_mainAudioSource, m_otherAudioSource;

    public AudioClip m_menuSong, m_gameSong;

    private Vector3 m_desiredPosition;
    public float leftMovementCanvas;

    FaderScript m_fader;

    public RectTransform m_scenesContainer;

    public LevelScript m_animalLevel;
    public LevelScript m_colorLevel;
    public LevelScript m_clothLevel;
    public LevelScript m_numberLevel;

    public Transform m_levelsContainer;

    public GameObject m_winPanel;

    public bool m_debugMode;

    private void Start()
    {
        m_fader = FindObjectOfType<FaderScript>();
        UpdateMenuButtons();
        if (m_mainAudioSource != null) m_mainAudioSource = GetComponent<AudioSource>();
        m_mainAudioSource.clip = m_menuSong;
        m_mainAudioSource.Play();
        m_winPanel.SetActive(false);
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
                        Navigate(0);
                    }
                }
            }
            else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    m_winPanel.SetActive(false);
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
        //}
    }

    public void OnClickColors()
    {
        //if (m_fader.fadeDone)
        //{
            Navigate(1);
            Instantiate(m_colorLevel);
        //}
    }

    public void OnClcikClothes()
    {
        //if (m_fader.fadeDone)
        //{
            Navigate(1);
            Instantiate(m_clothLevel);
        //}
    }

    public void OnClickNumbers()
    {
        //if (m_fader.fadeDone)
        //{
            Navigate(1);
            Instantiate(m_numberLevel);
        //}
    }

    public void OnClickMenu()
    {
        Navigate(0);
    }

    public void OnClickReplay()
    {
        LevelScript ls = FindObjectOfType<LevelScript>();

        if (ls.m_canTouch)
        {
            if (ls.m_lastClip != null) StartCoroutine(ls.PlaySound(ls.m_lastClip, false));
        }
    }

    public void Navigate(int scene)
    {
        switch (scene)
        {
            case 0: //menu
                UpdateMenuButtons();

                m_desiredPosition = Vector3.zero;

                LevelScript ls = FindObjectOfType<LevelScript>();

                if(ls!=null) Destroy(ls.gameObject);

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
}
