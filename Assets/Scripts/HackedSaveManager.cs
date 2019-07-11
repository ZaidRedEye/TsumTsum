using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackedSaveManager : MonoBehaviour
{
    public static void SetInt(SAVE_KEY key, int value)
    {
        PlayerPrefs.SetInt(key.ToString(), value);
    }

    public static int GetInt(SAVE_KEY key, int defaultValue = 0)
    {
        return PlayerPrefs.GetInt(key.ToString(), defaultValue);
    }

    public static void SetFloat(SAVE_KEY key, float value)
    {
        PlayerPrefs.SetFloat(key.ToString(), value);
    }

    public static float GetFloat(SAVE_KEY key, float defaultValue = 0)
    {
        return PlayerPrefs.GetFloat(key.ToString(), defaultValue);
    }

    public static void SetString(SAVE_KEY key, string value)
    {
        PlayerPrefs.SetString(key.ToString(), value);
    }

    public static string GetString(SAVE_KEY key, string defaultValue = "")
    {
        return PlayerPrefs.GetString(key.ToString(), defaultValue);
    }
}

public enum SAVE_KEY
{
    STICKER_WIN,
    STICKER_START,
    STICKER_LOSE,
    DISPLAY_NAME,
    INVALID
}