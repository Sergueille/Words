using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using UnityEngine;

public static class SaveManager
{
    public const string RUN_SAVE_NAME = "run";

    public static void SaveRun(GameInfo.State state)
    {
        GameManager.i.gi.randomState = Random.state;
        GameManager.i.gi.state = state;

        GameManager.i.gi.bonuses = new BonusInfo[GameManager.MAX_BONUS];
        int i = 0;
        foreach (Bonus b in GameManager.i.bonuses) {
            GameManager.i.gi.bonuses[i] = b.info;
            i++;
        }
        GameManager.i.gi.bonusCount = i;

        // Copy letters to struct
        for (int j = 0; j < 26; j++)
        {
            GameManager.i.gi.lettersCopy[j] = GameManager.i.letters[j].GetStruct();
        }

        byte[] bytes = GetBytes(GameManager.i.gi);
        System.IO.File.WriteAllBytes(GetPath(RUN_SAVE_NAME), bytes);
        Debug.Log($"Saved at {Application.persistentDataPath}");
    }

    public static void LoadRun()
    {
        byte[] bytes = System.IO.File.ReadAllBytes(GetPath(RUN_SAVE_NAME));
        GameManager.i.gi = FromBytes<GameInfo>(bytes);

        // Copy letter data
        GameManager.i.letters = new Letter[26];
        for (int i = 0; i < 26; i++)
        {
            GameManager.i.letters[i] = GameManager.i.gi.lettersCopy[i].ToRealLetter((char)('A' + i));
        }

        Random.state = GameManager.i.gi.randomState;
        PanelsManager.i.SelectPanel(GameManager.i.gi.currentPanelName, false);
        GameManager.i.CreateBonusUIFromGameInfo();
        GameManager.i.SetBlessingPoints(GameManager.i.gi.blessingPoints);

        switch (GameManager.i.gi.state) {
            case GameInfo.State.Ingame:
                GameManager.i.InitLevel();
                break;
            case GameInfo.State.Bonus:
                BonusManager.i.Do();
                break;
            case GameInfo.State.Curse:
                EventManager.i.Do(true, GameManager.i.StartNewLevel);
                break;
            case GameInfo.State.Blessing:
                EventManager.i.Do(false, GameManager.i.LevelCompleted);
                break;
            case GameInfo.State.Improvement:
                ImprovementManager.i.Do();
                break;
            default:
                throw new System.Exception("Unreachable!");
        };
    }

    private static byte[] GetBytes<T>(T obj)
    {
        int length = Marshal.SizeOf<T>();
        System.IntPtr ptr = Marshal.AllocHGlobal(length);
        byte[] myBuffer = new byte[length];

        Marshal.StructureToPtr(obj, ptr, false);
        Marshal.Copy(ptr, myBuffer, 0, length);
        Marshal.FreeHGlobal(ptr);

        return myBuffer;
    }

    private static T FromBytes<T>(byte[] bytes)
    {
        GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        
        T res = Marshal.PtrToStructure<T>(handle.AddrOfPinnedObject());

        handle.Free();

        return res;
    }

    private static string GetPath(string filename)
    {
        return $"{Application.persistentDataPath}/{filename}.save";
    }
}
