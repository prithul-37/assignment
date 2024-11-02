using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers : MonoBehaviour
{
    [SerializeField] LayerMask solidObjLayer;
    [SerializeField] LayerMask interactableLayer;
    [SerializeField] LayerMask LongGrass;
    [SerializeField] LayerMask playerLayer;


    public static GameLayers i { get; set; }

    private void Awake()
    {
        i=this;
    }
    public LayerMask SolidLayer { get => solidObjLayer; }
    public LayerMask InteractableLayer { get => interactableLayer; }
    public LayerMask GrassLayer { get => LongGrass; }

    public LayerMask PlayerLayer { get => playerLayer; }


}
