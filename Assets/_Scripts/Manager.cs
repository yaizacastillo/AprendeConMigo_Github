using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour {

    public static Manager Instance { set; get; }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    public int m_currentLevel = 0;
    public int m_menuFocus = 0;
}
