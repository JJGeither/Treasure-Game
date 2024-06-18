using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyDisplay : MonoBehaviour
{
    [SerializeField]
    private TMP_Text m_Text;

    // Update is called once per frame
    void Update()
    {
        m_Text.text = PlayerController.instance.moneyAmount.ToString();
    }
}
