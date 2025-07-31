// CardInstance.cs
using System;
using UnityEngine;

public class CardInstance
{
    // 원본 카드 데이터
    public CardSO Data { get; }
    // 이 인스턴스 고유 ID
    public string InstanceID { get; }

    public CardInstance(CardSO data)
    {
        Data       = data;
        InstanceID = Guid.NewGuid().ToString();
    }
}
