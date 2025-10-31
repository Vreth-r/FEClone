using UnityEngine;
using TMPro;

public class TileInfo : MonoBehaviour
{
    public TextMeshProUGUI terrainName;
    public TextMeshProUGUI cost;

    void Start()
    {
        gameObject.SetActive(false);
    }

    public void UpdateInfo(string terrain, int c)
    {
        terrainName.text = terrain;
        this.cost.text = "Cost: " + c.ToString();
    }
    
}
