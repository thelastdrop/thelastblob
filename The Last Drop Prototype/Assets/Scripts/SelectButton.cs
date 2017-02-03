using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectButton : MonoBehaviour {

    public Button Button;

    private void OnEnable()
    {
        selectit();
    }

    void selectit()
    {
        Button.Select();
    }
}
