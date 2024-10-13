using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card : MonoBehaviour
{
    //VisualsProvider

    //DrawLogic

    //PlayLogic

    //DATA
    [SerializeField]
    private CardSO _data;

    //VISUALS
    [SerializeField]
    private RectTransform _transform;


    //STATE
    public void Highlight()
    {
        _transform.localPosition += new Vector3(0, 50, 0);
    }


    public virtual void Play()
    {
        Debug.Log("Card played");
    }



}


public interface IPlayBehaviour
{
    public void Play();
}

