using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColletableManager : MonoBehaviour
{
    public AudioSource m_audiosource;

    public List<AudioClip> m_collectableSounds = new List<AudioClip>();

    public void OnClicked(Button button)
    {
        foreach(AudioClip sound in m_collectableSounds)
        {
            if(sound.name == button.tag)
            {
                m_audiosource.PlayOneShot(sound);
                return;
            }
        }
    }
}
