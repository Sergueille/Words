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


    private void Awake()
    {
        i = this;
    }

    private void Start() 
    {
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
                key.UpdateTexts();
            }

            if (i == lines.Length - 1) {
                Instantiate(submitPrefab, lineObject.transform);
            }
        }
    }

}
