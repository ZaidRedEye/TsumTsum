using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class PlayerGameLog
{
   public string name;
   public int[] stickerIds;
   public int totalScore;
   public long dateTime;

   public PlayerGameLog(string playerName, int[] stickerIds)
   {
      name = playerName;
      dateTime = DateTime.Now.Ticks;
      this.stickerIds = stickerIds;
   }
   
   public List<Round> rounds = new List<Round>();

   public void SaveToFile(bool prettyPrint = false)
   {
      var filePath = Path.Combine(Application.persistentDataPath, $"{name}_{dateTime}_{totalScore}.json");
      Debug.Log(filePath);
      File.WriteAllText(filePath,
         JsonUtility.ToJson(this, prettyPrint));
   }
}

[Serializable]
public struct Round
{
   public int roundNo;
   public float startTimeStamp;
   public List<ScoreLog> scoreLogs;
   public List<FeverLog> feverLogs;
}

[Serializable]
public struct ScoreLog
{
   public float timeStamp;
   public int currentScore;
}

[Serializable]
public struct FeverLog
{
   public float timeStamp;
}
