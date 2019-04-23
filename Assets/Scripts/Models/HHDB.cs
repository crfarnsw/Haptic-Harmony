using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.Leaderboard;
using Assets.Scripts.Models;
using UnityEngine;
using SQLite4Unity3d;

public class HapticHarmonyDB
{
    private SQLiteConnection context;

    public HapticHarmonyDB(string dbName)
    {
        var dbPath = Path.Combine("Assets/StreamingAssets/", dbName);

        if (!File.Exists(dbPath))
        {
            context = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
            CreateDB();
        }
        else
        {
            context = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
        }

        Debug.Log("Final PATH: " + dbPath);     
    }

    public void CreateDB()
    {
        //context.DropTable<Leaderboard>();
        //context.DropTable<PlayerProfile>();
        context.CreateTable<Leaderboard>();
        context.CreateTable<PlayerProfile>();
    }
    
    public IEnumerable<PlayerProfile> GetPlayers()
    {
        return context.Table<PlayerProfile>();
    }

    public IEnumerable<Leaderboard> GetScores()
    {
        return context.Table<Leaderboard>();
    }
    
    public PlayerProfile InsertPlayer(string name)
    {
        var p = new PlayerProfile
        {
            Name = name
        };
        context.Insert(p);
        return p;
    }

    public PlayerProfile GetPlayerByName(string name)
    {
        return GetPlayers().First(p => p.Name == name);
    }

    public Leaderboard InsertScore(int playerId, float score, string song)
    {
        var p = new Leaderboard
        {
            PlayerProfileID = playerId,
            Score = score,
            Song = song
        };

        context.Insert(p);
        return p;
    }
}
