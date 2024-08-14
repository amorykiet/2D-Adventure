using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CombatText : MonoBehaviour
{
    [SerializeField] private TMP_Text damageText;

    public void OnInit(float damage)
    {
        damageText.text = damage.ToString();
        Invoke(nameof(onDespawn), 1f);
    }

    public void onDespawn()
    {
        Destroy(gameObject);
    }
}
