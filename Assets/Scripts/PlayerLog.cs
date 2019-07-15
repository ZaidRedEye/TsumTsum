using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerLog
{
   public string name;
   public int totalScore;
   public List<Round> rounds = new List<Round>();
}

[Serializable]
public struct Round
{
   public int roundNo;
   public int totalScore;
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
