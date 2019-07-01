using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour {

    public static SaveManager Instance { set; get; }
    public SaveState m_saveState;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Instance = this;
        Load();
    }

    public void Load()
    {
        if (PlayerPrefs.HasKey("Save"))
            m_saveState = HelperScript.Deserialize<SaveState>(PlayerPrefs.GetString("Save"));
        else
        {
            m_saveState = new SaveState();
            Save();
        }
    }

    public void Save()
    {
        PlayerPrefs.SetString("Save", HelperScript.Serialize<SaveState>(m_saveState));
    }

    public void LevelCompleted ()
    {
        m_saveState.m_levelCompleted++;
        Save();
    }

    public void ResetSave()
    {
        PlayerPrefs.DeleteKey("Save");
    }
}
