using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KaimiraGames;
using Random = System.Random;
using UnityEditor;

[Serializable]
public class WeightedListContainer<T> : ISerializationCallbackReceiver
{

    #region Inspector Stuff
    [AllowNesting, SerializeField, ReadOnly, Label("Total Weight")]
    private int InspectorTotalWeight = 0;

    public List<WeightedItem<T>> Items;
    #endregion

    [SerializeField]
    private WeightedList<T> _weightedList = new WeightedList<T>();
    public int TotalWeight => _weightedList.TotalWeight;


    public void Init(int seed = 0)
    {
        Random seededRanom = seed == 0? null : new System.Random(seed);

        _weightedList = new(Items.Select(WI => new WeightedListItem<T>(WI.Item, WI.Weight)).ToList(), seededRanom);
    }
    public void Init(Random random)
    {
        _weightedList = new(Items.Select(WI => new WeightedListItem<T>(WI.Item, WI.Weight)).ToList(), random);
    }

    void OnValidate()
    {
        if(!Application.isPlaying)
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
                var weightInList = _weightedList.GetWeightOf(item.Item);
                item.InspectorWeight = weightInList.ToString();
                item.PercentInList = $" ({((float)weightInList / TotalWeight * 100).ToString("##")}%)";
            }
        }
    }

    public void AddWeight(T item, int weight)
    {
        if (!_weightedList.Contains(item)) return;

        var previousWeight = _weightedList.GetWeightOf(item);
        _weightedList.SetWeight(item, previousWeight + weight);
    }

    public void AddWeightAt(int index, int weight)
    {
        if (index < _weightedList.Count) return;

        var previousWeight = _weightedList.GetWeightAtIndex(index);
        _weightedList.SetWeightAtIndex(index, previousWeight + weight);
    }


    public T GetRandomItem()
    {
        return _weightedList.Next();
    }


    [Serializable]
    public class WeightedItem<T> : ISerializationCallbackReceiver
    {
        [HideInInspector] public string Name;
        [HideInInspector] public string PercentInList;
        [HideInInspector] public string InspectorWeight;

        public int Weight;
        public T Item;

        void OnValidate()
        {
            if (Item is UnityEngine.Object obj)
                Name = $"{obj.name}";
            else
                Name = $"{Item.ToString()}";

            Name += $".     [W:{InspectorWeight}]{PercentInList}";

        }
        void ISerializationCallbackReceiver.OnBeforeSerialize() => this.OnValidate();
        void ISerializationCallbackReceiver.OnAfterDeserialize() { }

    }
}


