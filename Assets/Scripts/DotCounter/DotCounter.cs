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
    
    [SerializeField]
    private bool circlesOnChange;
    [SerializeField]
    private float circlesSize;

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
            dots[i].SetActive(i < startValue);
        }

        currentValue = startValue;
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
        if (currentValue < newValue && circlesOnChange) {
            for (int i = currentValue; i < newValue; i++) {
                ParticlesManager.i.CircleParticles(dots[i].transform.position, circlesSize);
            }
        }

        currentValue = newValue;

        for (int i = 0; i < maxValue; i++) 
        {
            dots[i].SetActive(i < currentValue);
        }
    }
}
