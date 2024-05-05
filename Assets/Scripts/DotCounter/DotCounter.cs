using System.Collections.Generic;
using UnityEngine;

public class DotCounter : MonoBehaviour
{
    public int maxValue;

    [SerializeField]
    private int startValue;
    private int currentValue;

    public Transform dotParent;
    public GameObject dotPrefab;

    [SerializeField]
    private Color inactiveColor;
    
    [SerializeField]
    private Color activeColor;

    private Dot[] dots;

    public int CurrentValue {
        get => currentValue;
    }

    private void Awake()
    {
        dots = new Dot[maxValue];
        for (int i = 0; i < maxValue; i++)
        {
            dots[i] = Instantiate(dotPrefab, dotParent).GetComponent<Dot>();
            dots[i].activeColor = activeColor;
            dots[i].inactiveColor = inactiveColor;
        }

        SetValue(startValue);
    }

    private void Update()
    {
        for (int i = 0; i < maxValue; i++)
        {
            dots[i].activeColor = activeColor;
            dots[i].inactiveColor = inactiveColor;
        }
    }

    public void SetValue(int newValue)
    {
        currentValue = newValue;

        for (int i = 0; i < maxValue; i++) 
        {
            dots[i].SetActive(i < currentValue);
        }
    }
}
