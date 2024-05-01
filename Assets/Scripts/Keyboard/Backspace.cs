using System.Collections.Generic;
using UnityEngine;

public class Backspace : MonoBehaviour
{
    public void OnPress() 
    {
        GameManager.i.EraseLastLetter();
    }
}
