using UnityEngine;
using TMPro;

public class UnitInfo : MonoBehaviour
{
    public TextMeshProUGUI unitName;
    public TextMeshProUGUI chp;
    public TextMeshProUGUI mhp;

    void Start()
    {
        gameObject.SetActive(false);
    }

    public void UpdateInfo(string unitN, int c, int m)
    {
        unitName.text = unitN;
        chp.text = c.ToString();
        mhp.text = m.ToString();
    }
    
}