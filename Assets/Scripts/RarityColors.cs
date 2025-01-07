using NaughtyAttributes;
using NueGames.NueDeck.Scripts.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RarityColors : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private Image TitleBG;
    [SerializeField] private Image DescriptionBG;
    [SerializeField] private Image IncantationBorder;
    [SerializeField] private Image SecondIncantationBorder;
    [SerializeField] private Image SpellBorder;


    [SerializeField] private List<ColorsByRarity> rarityColors;

    public void SetColors(RarityType rarityType)
    {
        var colors = rarityColors.Find(c => c.Rarity == rarityType);

        TitleBG.color = colors.TitleBG;
        DescriptionBG.color = colors.DescriptionBG;
        IncantationBorder.color = colors.IncantationBorder;
        SecondIncantationBorder.color = colors.IncantationBorder;
        SpellBorder.color = colors.SpellBorder;
    }


    [SerializeField] private RarityType _testRarity;
    [Button("Test")]
    public void SetColors()
    {
        var colors = rarityColors.Find(c => c.Rarity == _testRarity);

        TitleBG.color = colors.TitleBG;
        DescriptionBG.color = colors.DescriptionBG;
        IncantationBorder.color = colors.IncantationBorder;
        SecondIncantationBorder.color = colors.IncantationBorder;
        SpellBorder.color = colors.SpellBorder;
    }

    [Serializable]
    public class ColorsByRarity
    {
        public RarityType Rarity;

        public Color TitleBG;
        public Color DescriptionBG;
        public Color IncantationBorder;
        public Color SpellBorder;
    }
    
}
