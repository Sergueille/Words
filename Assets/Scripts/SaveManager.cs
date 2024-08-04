using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public static class SaveManager
{
    public const string RUN_SAVE_NAME = "run";
    public const string PROGRESS_SAVE_NAME = "progress";

    public static void SaveRun(GameInfo.State state)
    {
        GameManager.i.gi.randomState = Random.state;
        GameManager.i.gi.state = state;

        GameManager.i.gi.constraint = GameManager.i.currentConstraint;

        GameManager.i.gi.bonuses = new BonusInfo[GameManager.i.bonuses.Count];
        for (int i = 0; i < GameManager.i.bonuses.Count; i++)
        {
            GameManager.i.gi.bonuses[i] = GameManager.i.bonuses[i].info;
        }

        StringBuilder b = new StringBuilder(Word.words.Count * 3);
        for (int i = 0; i < Word.words.Count; i++) {
            if (Word.wordArray[i].timesUsed != 0) {
                Util.AppendIntIntoStringBuilder(b,i);
                b.Append(':');
                Util.AppendIntIntoStringBuilder(b, Word.wordArray[i].timesUsed);
                b.Append(';');
            }
        }

        GameManager.i.gi.wordUseCounts = b.ToString();

        string txt = JsonUtility.ToJson(GameManager.i.gi, true);
        System.IO.File.WriteAllText(GetPath(RUN_SAVE_NAME), txt);
    }

    public static void LoadRun()
    {
        string txt = System.IO.File.ReadAllText(GetPath(RUN_SAVE_NAME));
        GameManager.i.gi = JsonUtility.FromJson<GameInfo>(txt);

        Random.state = GameManager.i.gi.randomState;
        PanelsManager.i.SelectPanel(GameManager.i.gi.currentPanelName, false);
        GameManager.i.CreateBonusUIFromGameInfo();
        GameManager.i.SetBlessingPoints(GameManager.i.gi.blessingPoints, true);

        int acc = 0;
        int targetWord = 0;
        for (int pos = 0; pos < GameManager.i.gi.wordUseCounts.Length; pos++) {
            if (GameManager.i.gi.wordUseCounts[pos] == ';') {
                Word.wordArray[targetWord].timesUsed = acc;
                acc = 0;
            }
            else if (GameManager.i.gi.wordUseCounts[pos] == ':') {
                targetWord = acc;
                acc = 0;
            }
            else {
                acc = 10 * acc + (GameManager.i.gi.wordUseCounts[pos] - '0');
            }
        }

        switch (GameManager.i.gi.state) {
            case GameInfo.State.Ingame:
                GameManager.i.InitLevel();
                GameManager.i.currentConstraint = GameManager.i.gi.constraint;
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

    public static void SaveProgression()
    {
        GameManager.i.progression.lastVersion = Application.version;

        string txt = JsonUtility.ToJson(GameManager.i.progression, true);
        System.IO.File.WriteAllText(GetPath(PROGRESS_SAVE_NAME), txt);
    }

    public static void LoadProgression()
    {
        string path = GetPath(PROGRESS_SAVE_NAME);

        if (System.IO.File.Exists(path)) {
            string txt = System.IO.File.ReadAllText(path);
            GameManager.i.progression = JsonUtility.FromJson<Progression>(txt);
        }
        else {
            GameManager.i.progression = Progression.GetDefaultProgression();
        }
    }

    public static void ResetAll()
    {
        try { System.IO.File.Delete(GetPath(PROGRESS_SAVE_NAME)); } catch {}
        try { System.IO.File.Delete(GetPath(RUN_SAVE_NAME)); } catch {}
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

    public static string GetPath(string filename)
    {
        return $"{Application.persistentDataPath}/{filename}.json";
    }
}
