# Deckaron (디저트 매장 경영 덱빌딩 게임)

마카롱 매장을 운영하며 재료 수급, 생산, 판매를 카드로 관리하는 턴제 덱빌딩 게임입니다. Unity 6000.0.40f1로 개발했으며, 포트폴리오 공개용 레포입니다.

[![게임 플레이 영상](https://img.youtube.com/vi/q7JgpeA1oGs/0.jpg)](https://youtu.be/q7JgpeA1oGs)

## 게임 개요

- 턴마다 제한 시간 안에 카드를 사용해 재료(아몬드/설탕/계란)를 수급하고, 시설 카드로 마카롱을 생산해 판매합니다.
- 상점에서 카드를 구매해 덱을 강화하고, 매 턴 종료 시 발생하는 랜덤 이벤트에 대응합니다.
- 지정된 턴 수를 마치면 최종 점수로 결과를 확인합니다.

## 주요 시스템

| 영역 | 설명 |
| --- | --- |
| 턴/타이머 | `TurnManager` — 턴 진행, 제한 시간, 턴당 골드 지급 |
| 자원 | `ResourceManager` — 재료(아몬드/설탕/계란) 관리 |
| 카드 | `CardSO` 기반 ScriptableObject 카드 (재료/시설/생산/강화/이벤트/스킬) |
| 생산 | `FacilityManager`, `MacaronManager` — 시설을 통한 마카롱 생산 |
| 상점 | `ShopManager` — 카드 구매 및 덱 강화 |
| 패시브 | `PassiveManager` / `PassiveSO` — 지속 효과 처리 |
| 이벤트 | `EventSO` 기반 랜덤 이벤트 (고양이, 골드, 알바 등) |
| UI | `UIManager`, `CardUI`, `TurnClockUI`, `ScoreBoardUI` 등 |

## 폴더 구조

```
Assets/
├── Scripts/
│   ├── GameScene/        # 인게임 로직 및 UI
│   │   └── Manager/      # 턴/자원/시설/상점/패시브/점수 매니저
│   └── SO/                # 카드·이벤트·패시브 ScriptableObject 정의
├── Scenes/                # MainMenu, GameScene
├── Prefabs/, Sprites/, SFX/, Fonts/, UI Soundpack/
```

## 실행 방법

1. Unity Hub에서 `6000.0.40f1` 에디터로 프로젝트를 엽니다.
2. `Assets/Scenes/MainMenu.unity`를 시작 씬으로 플레이합니다.

> 참고: 이미지·오디오 등 바이너리 에셋 원본은 용량 문제로 이 저장소에는 포함되어 있지 않습니다(`.gitignore` 처리). 소스 코드와 씬 구성, 카드/이벤트 데이터 구조 확인용으로 참고해 주세요.

## 기술 스택

- Unity 6000.0.40f1 (URP)
- C# / ScriptableObject 기반 데이터 아키텍처
- Unity Input System, UI Toolkit / uGUI
