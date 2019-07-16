using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerLogRepository
{
    private static PlayerLogRepository _instance;
    
    public static PlayerLogRepository Instance => _instance ?? (_instance = new PlayerLogRepository());

    public IEnumerable<PlayerGameLog> PlayerGameLogs => _playerGameLogs;

    private readonly List<PlayerGameLog> _playerGameLogs = new List<PlayerGameLog>();
    
    private PlayerLogRepository()
    {
        foreach (var path in Directory.EnumerateFiles(Application.persistentDataPath, "*.json"))
        {
            var playerLog = JsonUtility.FromJson<PlayerGameLog>(File.ReadAllText(path));
            if(playerLog == null) continue;
            _playerGameLogs.Add(playerLog);
        }
    }
}
