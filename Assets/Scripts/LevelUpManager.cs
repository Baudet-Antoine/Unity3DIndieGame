using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUpManager : MonoBehaviour
{
    public Button LeftUpgrade;
    public Button CenterUpgrade;
    public Button RightUpgrade;

    public Image LeftImage;
    public Image CenterImage;
    public Image RightImage;

    public TextMeshProUGUI LeftText;
    public TextMeshProUGUI CenterText;
    public TextMeshProUGUI RightText;

    Upgrade[] up;

    public int DamageUpgrade;
    public float AttackSpeedUpgrade = 1.0f;
    public int maxHealthUpgrade;
    public float MovementSpeedUpgrade;

    public static LevelUpManager Instance;
    public Dictionary<string, Func<Color>> GetColor = new Dictionary<string, Func<Color>>()
    {
        { "Common", () => new Color(0.5f, 0.5f, 0.5f) },
        { "Uncommon", () => new Color(0f, 1f, 0f) },
        { "Rare", () => new Color(0f, 0f, 1f) },
        { "Mythical", () => new Color(0.5f, 0f, 1f) },
        { "Legendary", () => new Color(215f / 255f, 183f / 255f, 64f / 255f) },
        { "Unique", () => new Color(1f, 0f, 0f) },
    };


    void Awake()
    {
        MovementSpeedUpgrade = 1.0f;
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }


    Upgrade[] GenerateUpgrade()
    {
        Upgrade[] chosenUpgrades = new Upgrade[3];

        Upgrade[] upgrades = Resources.LoadAll<Upgrade>("Upgrades");
        if (upgrades.Length > 0)
        {
            for(int i = 0 ; i < 3 ; i++)
            {
                Upgrade selectedUpgrade = upgrades[UnityEngine.Random.Range(0, upgrades.Length)];
                chosenUpgrades[i] = new Upgrade();
                chosenUpgrades[i].Image = selectedUpgrade.Image;
                chosenUpgrades[i].Name = selectedUpgrade.Name;
                chosenUpgrades[i].Description = selectedUpgrade.Description;
                chosenUpgrades[i].Value = selectedUpgrade.Value; 
                chosenUpgrades[i].Rarity = selectedUpgrade.Rarity; 
            }
        }
        return chosenUpgrades;
    }

    public void LevelUp()
    {
        Time.timeScale = 0f;
        foreach (Transform child in this.gameObject.transform)
        {
            child.gameObject.SetActive(true);
        }
        up = GenerateUpgrade();

        LeftText.text = up[0].Rarity + " " + up[0].Name;
        if (GetColor.ContainsKey(up[0].Rarity))
        {
            LeftImage.color = GetColor[up[0].Rarity]();
        }
        CenterText.text = up[1].Rarity + " " + up[1].Name;
        if (GetColor.ContainsKey(up[1].Rarity))
        {
            CenterImage.color = GetColor[up[1].Rarity]();
        }
        RightText.text = up[2].Rarity + " " + up[2].Name;
        if (GetColor.ContainsKey(up[2].Rarity))
        {
            RightImage.color = GetColor[up[2].Rarity]();
        }
    }

    public void ChooseUpgrade(int chosenValue)
    {
        Time.timeScale = 1.0f;

        switch (up[chosenValue].Name)
        {
            case "Base Health":
                maxHealthUpgrade += Mathf.RoundToInt(PlayerController.Instance.maxHealth * up[chosenValue].Value);
                PlayerController.Instance.UpdateStats();
                PlayerController.Instance.HealPlayer(Mathf.RoundToInt(PlayerController.Instance.maxHealth * up[chosenValue].Value));
                break;
            case "Attack Speed":
                AttackSpeedUpgrade /= up[chosenValue].Value;
                PlayerController.Instance.UpdateStats();
                break;
            case "Attack Damage":
                DamageUpgrade += Mathf.RoundToInt(PlayerController.Instance.BaseAttack * up[chosenValue].Value);
                PlayerController.Instance.UpdateStats();
                break;
            case "Movement Speed":
                MovementSpeedUpgrade += up[chosenValue].Value;
                PlayerController.Instance.UpdateStats();
                break;
            default:
                Debug.Log(up[chosenValue].Name + " not found");
                break;
        }

        foreach (Transform child in this.gameObject.transform)
        {
            child.gameObject.SetActive(false);
        }

    }
}
