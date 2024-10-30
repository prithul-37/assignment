using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] Dialog dialog;

    public void  Interact()
    {
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog));
    }
}
