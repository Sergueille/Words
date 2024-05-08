using System.Collections.Generic;
using UnityEngine;

public class Keyboard : MonoBehaviour
{
    public static Keyboard i;
    
    public GameObject linePrefab;
    public GameObject keyPrefab;
    public GameObject backspacePrefab;
    public GameObject submitPrefab;
    public Transform keyboardParent;  

    public string[] lines;

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

        // Instantiate keys
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];

            GameObject lineObject = Instantiate(linePrefab, keyboardParent);

            if (i == lines.Length - 1) {
                Instantiate(backspacePrefab, lineObject.transform);
            }

            foreach (char c in line) 
            {
                Key key = Instantiate(keyPrefab, lineObject.transform).gameObject.GetComponent<Key>();
                key.letter = c;
                key.onPress = letter => { GameManager.i.InputLetter(letter); };

                keys[keyID] = key;

                keyID++;
            }

            if (i == lines.Length - 1) {
                Instantiate(submitPrefab, lineObject.transform);
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
