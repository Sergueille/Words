using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    public TextMeshProUGUI tipText;

    private void Start()
    {
        string[] tips = new string[] {
            "Letters can only have one effect. You can remove unwanted effects from letters if you give them another.",
            "You can use Charged bonuses to remove negative effects from letters. Make sure you don't accidentally remove good effects.",
            "Remember you can remove bonuses at any time.",
            "Making words with lower scores can allow you to write 3 words per level, so letters can be improved more.",
            "The game will never propose a bonus you already have. However, you can copy them with benediction.",
            "Taking Average is generally a bad idea. Because it's rounded down.",
            "Don't be afraid of using the same word multiple times.",
            "Use bonuses that make points at the end of your run, prefer bonuses that improve letters at the beginning.",
            "You can change the keyboard layout in the settings.",
            $"Your save files are located in {Application.persistentDataPath}.",
        };

        tipText.text = tips[Random.Range(0, tips.Length)];

        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }
}
