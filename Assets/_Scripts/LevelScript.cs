using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LevelScript : MonoBehaviour {

    GameManager m_gameManager;

    public AudioSource m_audioSource;

    public List<AudioClip> m_startingSounds = new List<AudioClip>();
    private List<AudioClip> m_remainingSounds = new List<AudioClip>();
    private List<AudioClip> m_showingSounds = new List<AudioClip>();

    [HideInInspector] public AudioClip m_lastClip = null;

    public AudioClip m_clickSound, m_correctSound, m_tryAgainSound;

    private List<Vector3> m_cardsPosition = new List<Vector3>();

    public List<GameObject> m_startingCards = new List<GameObject>();
    private List<GameObject> m_remainingCards = new List<GameObject>();
    private List<Collider2D> m_cardsCollider = new List<Collider2D>();

    private bool m_answerIsCorrect = false;

    bool touch = false;

    public enum levelState
    {
        showing,
        playing,
        waiting
    }

    public levelState m_currentState = levelState.showing;

    ParticleSystem clickParticle;

    private void Start()
    {
        m_gameManager = FindObjectOfType<GameManager>();

        foreach (AudioClip a in m_startingSounds)
        {
            m_remainingSounds.Add(a);
            m_showingSounds.Add(a);
        }

        foreach (GameObject card in m_startingCards)
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

    private IEnumerator ShowCards()
    {
        List<Animator> l_showingCards = new List<Animator>();

        yield return new WaitForSeconds(1.0f);

        for (int i = 0; i < m_startingCards.Count; i++)
        {
            if (touch)
            {
                int l_rdm = Random.Range(0, m_remainingCards.Count);
                Instantiate(m_remainingCards[l_rdm], m_cardsPosition[i], Quaternion.identity, this.transform);
                m_remainingCards.RemoveAt(l_rdm);
            }

            else
            {

                int l_rdm = Random.Range(0, m_remainingCards.Count);

                GameObject card = Instantiate(m_remainingCards[l_rdm], m_cardsPosition[i], Quaternion.identity, this.transform);
                Collider2D collider = card.GetComponent<Collider2D>();

                m_cardsCollider.Add(collider);
                collider.enabled = false;

                AudioClip l_sound = m_showingSounds[l_rdm];

                m_audioSource.PlayOneShot(l_sound);

                yield return new WaitForSeconds(l_sound.length + 0.5f);

                m_remainingCards.RemoveAt(l_rdm);
                m_showingSounds.RemoveAt(l_rdm);

                if (touch)
                    continue;

                Animator l_cardAnimator = card.GetComponent<Animator>();
                l_showingCards.Add(l_cardAnimator);
                l_cardAnimator.SetTrigger("Disable");

                yield return new WaitForSeconds(1.0f);
            }
        }

        yield return new WaitForSeconds(0.5f);

        foreach (Animator a in l_showingCards)
        {
            a.SetTrigger("Enable");
        }

        yield return new WaitForSeconds(1.0f);

        foreach (Collider2D collider in m_cardsCollider)
        {
            collider.enabled = true;
        }

        StartCoroutine(m_gameManager.StartLevel());
    }

    public void PlayRandomSound()
    {
        int rdm = Random.Range(0, m_remainingSounds.Count);
        StartCoroutine(PlaySound(m_remainingSounds[rdm], true));
    }

    public IEnumerator PlaySound(AudioClip sound, bool save)
    {
        m_currentState = levelState.playing;
        m_audioSource.PlayOneShot(sound);
        if (save) m_lastClip = sound;
        yield return new WaitForSeconds(sound.length);
        if (sound == m_correctSound) PlayRandomSound();
        else if (sound == m_clickSound) StartCoroutine(PlaySound(m_tryAgainSound, false));
        m_currentState = levelState.waiting;
    }

    private void CheckTouch(Vector3 pos)
    {
        switch (m_currentState)
        {
            case levelState.playing:
                StopAllCoroutines();
                m_audioSource.Stop();
                m_currentState = levelState.waiting;
                break;
            case levelState.showing:
                touch = true;
                break;
        }

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
              
                m_gameManager.clickParticleSystem.transform.position = hitInfo.transform.position;
                m_gameManager.clickParticleSystem.Play();

                m_remainingSounds.Remove(m_lastClip);

                if (m_remainingSounds.Count == 0)
                {
                    //SaveManager.Instance.LevelCompleted();
                    m_gameManager.EndLevel();
                    //StartCoroutine(PlaySound(m_endLevelSound, false));
                }

                else
                {
                    StartCoroutine(PlaySound(m_correctSound, false));
                    hitInfo.transform.GetComponent<Animator>().SetTrigger("Disable");
                    hitInfo.transform.GetComponent<Collider2D>().enabled = false;
                    //PlayRandomSound();
                }
            }

        }
    }
}
