using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelScript : MonoBehaviour {

    GameManager m_gameManager;

    public AudioSource m_audioSource;

    public List<AudioClip> m_startingSounds = new List<AudioClip>();
    private List<AudioClip> m_remainingSounds = new List<AudioClip>();
    [HideInInspector] public AudioClip m_lastClip = null;

    public AudioClip m_clickSound, m_correctSound, m_tryAgainSound;

    private List<Vector3> m_cardsPosition = new List<Vector3>();

    public List<GameObject> m_cards = new List<GameObject>();
    private List<GameObject> m_remainingCards = new List<GameObject>();

    public bool m_canTouch = false;
    //public bool m_playSound = true;
    private bool m_answerIsCorrect = false;

    Canvas m_canvas;

    private void Start()
    {
        m_gameManager = FindObjectOfType<GameManager>();

        foreach (AudioClip a in m_startingSounds)
        {
            m_remainingSounds.Add(a);
        }

        foreach (GameObject card in m_cards)
        {
            m_remainingCards.Add(card);
        }

        foreach (Transform g in Camera.main.transform)
        {
            Vector3 l_pos = Vector3.zero;
            l_pos = transform.TransformPoint(g.position);
            m_cardsPosition.Add(l_pos);
        }

        StartCoroutine(ShowCards());
    }

    private void Update()
    {
        if(m_canTouch)
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (Input.touchCount > 0 && Input.touchCount < 2)
                {
                    if (Input.GetTouch(0).phase == TouchPhase.Began)
                    {
                        CheckTouch(Input.GetTouch(0).position);
                    }
                }
            }
            else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    CheckTouch(Input.mousePosition);
                }
            }
        }
    }

    private IEnumerator ShowCards()
    {
        print("Show cards");

        //m_playSound = false;

        //yield return new WaitForSeconds(5.0f); //duracion de la animacion

        yield return new WaitForSeconds(1.0f);

        PlaceCards();
    }

    public void PlaceCards()
    {
        print("Place cards");

        for(int i = 0; i < m_cards.Count; i++)
        {
            int l_rdm = Random.Range(0, m_remainingCards.Count);

            Instantiate(m_remainingCards[l_rdm], m_cardsPosition[i], Quaternion.identity, this.transform);
            m_remainingCards.RemoveAt(l_rdm);
        }

        PlayRandomSound();
    }

    private void PlayRandomSound()
    {
        int rdm = Random.Range(0, m_remainingSounds.Count);
        StartCoroutine(PlaySound(m_remainingSounds[rdm], true));
    }

    public IEnumerator PlaySound(AudioClip sound, bool save)
    {
        print("Play sound");
        //m_playSound = false;
        m_canTouch = false;
        m_audioSource.PlayOneShot(sound);
        yield return new WaitForSeconds(sound.length);
        if (save) m_lastClip = sound;
        m_canTouch = true;
        if (sound == m_correctSound) PlayRandomSound();
        else if (sound == m_clickSound) StartCoroutine(PlaySound(m_tryAgainSound, false));
        print("End sound");
    }

    private void CheckTouch(Vector3 pos)
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(pos), Vector2.zero);

        if (hitInfo)
        {

            m_answerIsCorrect = hitInfo.transform.tag == m_lastClip.name ? true : false;

            if (!m_answerIsCorrect)
            {
                StartCoroutine(PlaySound(m_clickSound, false));
                //StartCoroutine(PlaySound(m_tryAgainSound, false));
            }

            else
            {
                m_remainingSounds.Remove(m_lastClip);

                if (m_remainingSounds.Count == 0)
                {
                    print("end level");
                    m_canTouch = false;
                    SaveManager.Instance.LevelCompleted();
                    m_gameManager.EndLevel();
                    //StartCoroutine(PlaySound(m_endLevelSound, false));
                }

                else
                {
                    StartCoroutine(PlaySound(m_correctSound, false));
                    hitInfo.transform.GetComponent<Animator>().SetTrigger("Acierto");
                    hitInfo.transform.GetComponent<Collider2D>().enabled = false;
                    //PlayRandomSound();
                }
            }

        }
    }
}
