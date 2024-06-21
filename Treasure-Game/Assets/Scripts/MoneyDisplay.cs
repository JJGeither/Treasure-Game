using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyDisplay : MonoBehaviour
{
    [SerializeField]
    public TMP_Text m_OxygenText;
    public TMP_Text m_HealthText;
    public TMP_Text m_CurrencyText;

    // Update is called once per frame
    void Update()
    {
        m_CurrencyText.text = "$: " + PlayerController.instance.playerStatistics.moneyAmount.ToString();
        m_OxygenText.text = "O2: " + PlayerController.instance.playerDrones.OxygenLevel.ToString() + " / " + PlayerController.instance.playerDrones.OxygenMaxLevel.ToString();
        m_HealthText.text = "HP: 100";
    }
}
