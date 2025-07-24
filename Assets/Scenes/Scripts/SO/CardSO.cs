// Assets/Scripts/Data/CardSO.cs
using UnityEngine;

public enum CardType
{
    Ingredient,
    Facility,
    Production,
    Enhancement,
    Event,
    Skill
}

[CreateAssetMenu(fileName = "NewCard", menuName = "Deckaroon/Card")]
public class CardSO : ScriptableObject
{
    public string cardID;
    public string cardName;
    public CardType cardType;
    public int cost;             // 상점 구매/사용 골드
    public Sprite icon;          // 카드 아트
    // 필요하다면 재료 수, 생산량 등 세부 필드 추가
    // public int supplyAmount;
    // public int produceAmount;
    // ...

    /// <summary>
    /// 카드를 사용할 때 실행되는 기본 로직.
    /// 파라미터로 필요한 매니저 전달.
    /// </summary>
    public virtual void Play(TurnManager turnManager,
                            ResourceManager resourceManager,
                            HandManager handManager,
                            ShopManager shopManager)
    {
        Debug.Log($"Play card: {cardName}");
        // 서브클래스에서 override 해서 각 카드 타입별 효과 구현
    }
}
