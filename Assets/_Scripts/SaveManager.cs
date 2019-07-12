using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour {

    public static SaveManager Instance { set; get; }
    public SaveState m_saveState;

    static bool created = false;

    private void Awake()
    {
        if (!created)
        {
            DontDestroyOnLoad(gameObject);
            created = true;
        }

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

    public void LevelCompleted (string type)
    {
        switch(type)
        {
            case "animals":
                m_saveState.m_animalsCompleted = true;
                break;
            case "clothes":
                m_saveState.m_clothesCompleted = true;
                break;
            case "colors":
                m_saveState.m_colorsCompleted = true;
                break;
            case "numbers":
                m_saveState.m_numbersCompleted = true;
                break;
        }
        Save();
    }

    public void ResetSave()
    {
        PlayerPrefs.DeleteKey("Save");
        Load();
    }
}
