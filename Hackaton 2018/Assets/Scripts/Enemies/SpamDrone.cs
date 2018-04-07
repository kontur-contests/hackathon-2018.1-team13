using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpamDrone : Drone
{
    [SerializeField]
    Text m_spamText;

    [SerializeField]
    Image m_background;

    [SerializeField]
    float m_showTime = 20;

    [SerializeField]
    float m_hideTime = 0.2f;

    float curTimer = 0;

    int spamIndex = 0;

    bool shown = true;

    public string[]  spamText = {"BUY", "BITCOINS" } ;

    public string[] aggroText = { "DIE", "SUFFER" };

    private void Start()
    {
        spamIndex = Random.Range(0, spamText.Length);
    }


    protected  override void Update()
    {
        base.Update();
        curTimer -= Time.deltaTime;
        if (curTimer < 0)
        {
            if (shown)
            {
                m_spamText.text = "";
                shown = false;
                curTimer = m_hideTime;
            }
            else
            {
                ShowText();
            }
        }

        m_background.color = GameCore.instance.aggrsive ? Color.red : Color.green;
    }

    void ShowText()
    {
        if (!GameCore.instance.aggrsive)
        {
            spamIndex = (spamIndex + 1) % spamText.Length;
            m_spamText.text = spamText[spamIndex];
        }
        else
        {
            spamIndex = (spamIndex + 1) % aggroText.Length;
            m_spamText.text = aggroText[spamIndex];
        }

        shown = true;
        curTimer = m_showTime;
    }

}
