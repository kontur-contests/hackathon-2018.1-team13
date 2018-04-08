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
    GameObject m_display;

    [SerializeField]
    float m_showTime = 20;

    [SerializeField]
    float m_hideTime = 0.2f;

    [SerializeField]
    int m_threatsTillAttack = 6;

    float curTimer = 0;

    int spamIndex = 0;

    int threatCounter = 0;

    bool shown = true;

    public string[]  spamText = {"BUY", "BITCOINS" } ;

    private static string[] aggroText = { "DIE", "SUFFER" };

    private void Start()
    {
        //spamIndex = Random.Range(0, spamText.Length);
        droneAnimator.SetInteger("State", 0);
    }

    protected override bool IsAgressive()
    {
        return base.IsAgressive() && fullAgressor;
    }

    bool fullAgressor = false;

    protected  override void Update()
    {
        base.Update();

        if (fullAgressor || !inFireRange)
            return;

        if (threatCounter > m_threatsTillAttack)
        {
            m_display.SetActive(false);
            droneAnimator.SetInteger("State", 2);
            fullAgressor = true;
            return;
        }

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

    public override bool OnTakeDamage(BodyPart part, AttackInfo aInfo)
    {
        if (!GameCore.instance.aggrsive)
            GameCore.instance.aggrsive = true;
        return base.OnTakeDamage(part, aInfo);
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
            threatCounter++;
        }

        shown = true;
        curTimer = m_showTime;
    }

}
