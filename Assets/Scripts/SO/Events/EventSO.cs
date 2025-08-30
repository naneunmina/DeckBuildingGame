using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Deckaroon/Event")]
public class EventSO : ScriptableObject
{
    [Header("Display")]
    public string title;
    [TextArea] public string description;
    public Sprite art;

    [Header("Choice 1")]
    [TextArea] public string choice1Desc;
    public Sprite choice1Art;
    [TextArea] public string choice1Result;

    [Header("Choice 2")]
    [TextArea] public string choice2Desc;
    public Sprite choice2Art;
    [TextArea] public string choice2Result;

    public virtual EventResult OnChoose1(TurnManager turnManager,
                            ResourceManager resourceManager,
                            ShopManager shopManager,
                            MacaronManager macaronManager,
                            FacilityManager facilityManager)
    => default;
    public virtual EventResult OnChoose2(TurnManager turnManager,
                            ResourceManager resourceManager,
                            ShopManager shopManager,
                            MacaronManager macaronManager,
                            FacilityManager facilityManager)
    => default;
}

public struct EventResult
{
    public Sprite art;
    public string text;
}