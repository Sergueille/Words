using System.Collections.Generic;
using UnityEngine;

public class SubmitBtn : MonoBehaviour
{
    public void OnPress() 
    {
        GameManager.i.SubmitWord();
    }
}
