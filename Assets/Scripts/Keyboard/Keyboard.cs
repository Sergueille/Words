using System.Collections.Generic;
using UnityEngine;

public class Keyboard : MonoBehaviour
{
    public static Keyboard i;
    
    public GameObject linePrefab;
    public GameObject keyPrefab;
    public GameObject spacePrefab;
    public GameObject backspacePrefab;
    public GameObject submitPrefab;
    public Transform keyboardParent;  

    public string[] layouts;

    private Key[] keys;

    private void Awake()
    {
        i = this;
    }

    public void Init()
    {
        foreach (Transform t in keyboardParent) {
            Destroy(t.gameObject);
        }

        keys = new Key[26];

        int keyID = 0;

        string layout = layouts[(int)SettingsManager.i.settings.keyboardLayout];
        GameObject lineObject = Instantiate(linePrefab, keyboardParent);

        foreach (char c in layout) 
        {
            if (c == '|') {
                lineObject = Instantiate(linePrefab, keyboardParent);
            }
            else if (c == '+') {
                Instantiate(submitPrefab, lineObject.transform);
            }
            else if (c == '*') {
                Instantiate(backspacePrefab, lineObject.transform);
            }
            else if (c == '\'') {
                Instantiate(spacePrefab, lineObject.transform);
            }
            else {
                Key key = Instantiate(keyPrefab, lineObject.transform).gameObject.GetComponent<Key>();
                key.letter = c;
                key.onPress = letter => { GameManager.i.InputLetter(letter); };

                keys[keyID] = key;

                keyID++;
            }
        }
    }

    public void UpdateAllKeys(bool showParticles)
    {
        foreach (Key key in keys)
        {
            key.UpdateUI(showParticles);
        }
    }

}
