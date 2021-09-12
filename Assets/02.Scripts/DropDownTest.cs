using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DropDownTest : MonoBehaviour
{
    public TMP_Dropdown userSelectChanel;


    // Start is called before the first frame update
    void Start()
    {
        
    }


 

    // Update is called once per frame
    void Update()
    {

      string text = userSelectChanel.options[userSelectChanel.value].text; ;

        print(text);

    }
}
