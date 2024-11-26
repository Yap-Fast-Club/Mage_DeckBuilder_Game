using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KaimiraGames;


[Serializable]
public class WeightedListContainer<T> : ISerializationCallbackReceiver
{

    #region Inspector Stuff
    [AllowNesting, SerializeField, ReadOnly, Label("Total Weight")]
    private int InspectorTotalWeight = 0;

    public List<WeightedItem<T>> Items;
    //public int TotalWeight { get => Items.Sum(i => i.Weight); }
    #endregion

    private WeightedList<T> _weightedList = new WeightedList<T>();
    public int TotalWeight => _weightedList.TotalWeight;


    void OnValidate()
    {
        UpdateWeightedList();
        UpdateInspectorPercentages();
    }
    void ISerializationCallbackReceiver.OnBeforeSerialize() => this.OnValidate();
    void ISerializationCallbackReceiver.OnAfterDeserialize() => this.UpdateInspectorPercentages();

    private void UpdateWeightedList()
    {
        _weightedList = new(Items.Select(WI => new WeightedListItem<T>(WI.Item, WI.Weight)).ToList());
    }
    private void UpdateInspectorPercentages()
    {
        InspectorTotalWeight = TotalWeight;

        if (TotalWeight > 0)
        {
            foreach (var item in Items)
            {
                item.PercentInList = $" ({((float)item.Weight / TotalWeight * 100).ToString("##")}%)";
            }
        }
    }
    public T GetRandomItem()
    {
        return _weightedList.Next();
    }

    public WeightedItem<T> GetRandomItem(int attempts = 100)
    {
        if (Items.Count != _weightedList.Count) UpdateWeightedList();

        List<WeightedItem<T>> reservoir = new List<WeightedItem<T>>(Items.Count);

        for (int i = 0; i < Items.Count; i++)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            if (j < reservoir.Count)
                reservoir[j] = Items[i];
            else
                reservoir.Add(Items[i]);

            if (--attempts == 0 || i == Items.Count - 1)
            {
                // Select a random item from the reservoir
                return reservoir[UnityEngine.Random.Range(0, reservoir.Count)];
            }
        }

        return null;
    }

    [Serializable]
    public class WeightedItem<T> : ISerializationCallbackReceiver
    {
        [HideInInspector] public string Name;
        [HideInInspector] public string PercentInList;

        public int Weight;
        public T Item;

        void OnValidate()
        {
            if (Item is UnityEngine.Object obj)
                Name = $"{obj.name}";
            else
                Name = $"{Item.ToString()}";

            Name += $".     [W:{Weight}]{PercentInList}";

        }
        void ISerializationCallbackReceiver.OnBeforeSerialize() => this.OnValidate();
        void ISerializationCallbackReceiver.OnAfterDeserialize() { }

    }
}


