using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    
    //public static UIManager Instance
    //{
    //    get
    //    {
    //        if (instance == null)
    //        {
    //            instance = FindObjectOfType<UIManager>();
    //        }

    //        return instance; 
    //    }
    //}

    private void Awake()
    {
        instance = this;
    }

    [SerializeField] TMP_Text coinText;

    public void SetCoin(int coinText)
    {
        this.coinText.text = coinText.ToString();
    }
}
