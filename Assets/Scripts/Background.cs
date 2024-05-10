using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;
using Unity.VisualScripting.Antlr3.Runtime;

public class Background : MonoBehaviour
{
    public float angle;
    public float speed;
    public float lineHeight;
    public int lineCount;
    public int charCount;
    public GameObject linePrefab;
    public Transform lineParent;

    private TextMeshPro[] texts;
    private float textsWidth;

    private void Start()
    {
        texts = new TextMeshPro[lineCount];

        for (int i = 0; i < lineCount; i++)
        {
            float yPos = GetBaseYPosOfText(i);

            TextMeshPro text = Instantiate(linePrefab, lineParent).GetComponent<TextMeshPro>();
            
            string randomStr = GetRandomString(charCount);
            text.text = randomStr + randomStr;
            
            text.transform.position = new Vector3(0, yPos, 0);
            text.transform.eulerAngles = new Vector3(0, 0, angle);
            texts[i] = text;
        }
    }

    private void Update()
    {
        for (int i = 0; i < lineCount; i++)
        {
            float xPos = ((speed * Time.time) % (texts[i].preferredWidth * 0.5f)) - texts[i].preferredWidth * 0.25f;

            if (i % 2 == 0)
            {
                xPos = -xPos;
            }

            float baseYPos = GetBaseYPosOfText(i);

            texts[i].transform.position = new Vector3(
                xPos * Mathf.Cos(angle * Mathf.Deg2Rad), 
                xPos * Mathf.Sin(angle * Mathf.Deg2Rad) + baseYPos, 
                lineParent.transform.position.z
            );
        }
    }

    private float GetBaseYPosOfText(int textId)
    {
        return ((float)textId / (float)(lineCount - 1) - 0.5f) * lineHeight * lineCount;
    }

    private string GetRandomString(int len)
    {
        StringBuilder randomStrBuilder = new StringBuilder(charCount);

        for (int i = 0; i < len; i++)
        {
            char c = (char)('A' + Random.Range(0, 26));
            randomStrBuilder.Append(c);
        }

        return randomStrBuilder.ToString();
    }
}
