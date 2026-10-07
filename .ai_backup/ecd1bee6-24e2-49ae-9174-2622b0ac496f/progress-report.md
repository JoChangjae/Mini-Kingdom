# 👑 미니왕국 프로젝트 — 개발 진행 보고서

## 📊 프로젝트 현황

| 항목 | 수치 |
|------|------|
| **총 파일 수** | 64개 |
| **C# 스크립트** | 58개 (128.5 KB) |
| **기획 문서** | 5개 (69.6 KB) |

---

## ✅ 완료된 작업

### 1. 📝 기획 문서 개선 (5개 파일)

**주요 변경사항:**
- ❌ 에너지(티켓) 시스템 **완전 제거** → ✅ 일일 보너스 시스템 (무제한 플레이!)
- ➕ **왕의 칙령** 시스템 추가 (매일 3택 중 1개 버프 선택)
- ➕ **왕국 방어 이벤트** 추가 (주 1회, 건설 동기 부여)
- ➕ **발견의 서** 도감 시스템 추가 (수집 → 영구 보너스)
- ➕ **완벽 회피** 메카닉 추가 (타이밍 회피 → 슬로모션 + 2배 데미지)
- 🔧 원소 4종 → **3타입(물리/마법/자연)**으로 간소화
- ❌ 길드 시스템 제거 → ✅ **왕국 방문** (가벼운 소셜)
- 🎨 컬러 팔레트를 **파스텔/따뜻한 톤**으로 변경
- 🎵 **사운드 디자인** 가이드 섹션 추가
- 💰 수익화에서 에너지 관련 항목 모두 제거, **배틀패스 비중 45%**로 상향

### 2. 🏗️ 코어 프레임워크 (11개 파일)

| 파일 | 설명 |
|------|------|
| `GameManager.cs` | 게임 상태 관리 싱글톤 |
| `EventBus.cs` | 제네릭 이벤트 발행/구독 시스템 |
| `GameEvents.cs` | 모든 게임 이벤트 구조체 정의 |
| `SaveManager.cs` | JSON 기반 로컬 저장/로드 + 백업 |
| `GameConfig.cs` | 글로벌 게임 설정 (ScriptableObject) |
| `DailyBonusSystem.cs` | 일일 보너스 런 관리 (첫 5런 1.5배) |
| `RoyalDecreeSystem.cs` | 왕의 칙령 일일 버프 시스템 |
| `Singleton.cs` | 제네릭 싱글톤 베이스 클래스 |
| `ObjectPool.cs` | 자동 확장 오브젝트 풀 |
| `Extensions.cs` | 유틸리티 확장 메서드 |
| `WeightedRandomPicker.cs` | 가중치 랜덤 선택기 |

### 3. 📦 데이터 레이어 (13개 파일)

| 파일 | 설명 |
|------|------|
| `ResourceType.cs` | 자원 타입 열거형 |
| `BuildingData.cs` | 시설 데이터 정의 (SO) |
| `EnemyData.cs` | 몬스터 데이터 정의 (SO) |
| `DungeonData.cs` | 던전 설정 데이터 (SO) |
| `SkillData.cs` | 스킬 데이터 정의 (SO) |
| `EquipmentData.cs` | 장비 데이터 정의 (SO) |
| `EquipmentSetData.cs` | 세트 장비 보너스 (SO) |
| `UpgradeData.cs` | 런 내 업그레이드 옵션 (SO) |
| `SynergyData.cs` | 시너지 보너스 정의 (SO) |
| `DiscoveryBookEntry.cs` | 도감 항목 정의 (SO) |
| `RoyalDecreeData.cs` | 왕의 칙령 데이터 (SO) |
| `StatModifier.cs` | 스탯 수정자 시스템 |
| `LootTable.cs` | 드롭 테이블 시스템 |

### 4. ⚔️ 게임플레이 시스템 (18개 파일)

| 카테고리 | 파일 | 설명 |
|----------|------|------|
| **Player** | `PlayerController.cs` | 이동, 회피, 완벽 회피 메카닉 |
| | `PlayerStats.cs` | 스탯 계산, 데미지 처리 |
| | `PlayerInventory.cs` | 장비/자원 관리 |
| **Combat** | `CombatSystem.cs` | 데미지 공식, 콤보, 크리티컬 |
| | `SkillSystem.cs` | 스킬 슬롯, 쿨다운, 활성화 |
| | `LevelUpSystem.cs` | 레벨업 3택 + 시너지 시스템 |
| **Enemy** | `EnemyController.cs` | AI 상태 머신, 행동 패턴 |
| | `EnemySpawner.cs` | 웨이브 기반 스폰 |
| | `BossController.cs` | 보스 페이즈 시스템 |
| **Dungeon** | `DungeonGenerator.cs` | 절차적 던전 생성 |
| | `RoomManager.cs` | 방 생명주기 관리 |
| | `DungeonRunManager.cs` | 런 전체 관리 + 보상 계산 |
| **Kingdom** | `BuildingManager.cs` | 건설/업그레이드/왕국 레벨 |
| | `ResourceManager.cs` | 자원 관리 싱글톤 |
| | `KingdomDefenseSystem.cs` | 왕국 방어 이벤트 |
| | `DiscoveryBookManager.cs` | 도감 추적 + 마일스톤 |
| **Items** | `LootManager.cs` | 드롭 처리 + 배율 적용 |
| | `EquipmentManager.cs` | 장비 제작/강화 |

### 5. 🎨 UI 프레임워크 (16개 파일)

| 카테고리 | 파일 | 설명 |
|----------|------|------|
| **Core** | `UIManager.cs` | 화면 스택 관리 |
| | `ScreenBase.cs` | 화면 베이스 클래스 |
| | `PopupManager.cs` | 팝업 큐 시스템 |
| | `UIAnimations.cs` | 트윈 애니메이션 유틸 |
| **Screens** | `KingdomScreen.cs` | 왕국 메인 허브 화면 |
| | `DungeonSelectScreen.cs` | 던전 선택 화면 |
| | `DungeonHUDScreen.cs` | 전투 중 HUD |
| | `LevelUpScreen.cs` | 레벨업 3택 선택 화면 |
| | `RunResultScreen.cs` | 런 결과 정산 화면 |
| | `RoyalDecreeScreen.cs` | 왕의 칙령 선택 화면 |
| | `DiscoveryBookScreen.cs` | 도감 화면 |
| | `BuildingDetailScreen.cs` | 건물 상세 정보 |
| | `SettingsScreen.cs` | 설정 화면 |
| **Components** | `ResourceBar.cs` | 자원 표시 컴포넌트 |
| | `SkillButton.cs` | 스킬 버튼 컴포넌트 |
| | `FloatingText.cs` | 플로팅 데미지/힐 숫자 |

---

## 📂 프로젝트 폴더 구조

```
mini-kingdom/
├── README.md
├── docs/
│   ├── 01-game-design-document.md   ← 개선 완료
│   ├── 02-tech-stack.md
│   ├── 03-game-systems.md           ← 개선 완료
│   ├── 04-ui-ux-guide.md            ← 개선 완료
│   └── 05-monetization.md           ← 개선 완료
└── Assets/Scripts/
    ├── Core/          (11 files)    ← 코어 프레임워크
    ├── Data/          (13 files)    ← ScriptableObject 정의
    ├── Player/         (3 files)    ← 플레이어 시스템
    ├── Combat/         (3 files)    ← 전투 시스템
    ├── Enemy/          (3 files)    ← 적 AI
    ├── Dungeon/        (3 files)    ← 던전 생성
    ├── Kingdom/        (4 files)    ← 왕국 건설
    ├── Items/          (2 files)    ← 장비/루트
    ├── UI/Core/        (4 files)    ← UI 프레임워크
    ├── UI/Screens/     (9 files)    ← 게임 화면들
    ├── UI/Components/  (3 files)    ← 재사용 UI 컴포넌트
    └── Utils/          (4 files)    ← 유틸리티
```

---

## 🚀 다음 단계

> [!IMPORTANT]
> Unity 프로젝트를 시작하려면 다음 순서로 진행하세요:

1. **Unity Hub에서 새 프로젝트 생성** (Unity 2022.3 LTS, 2D URP 템플릿)
2. **생성된 `Assets/Scripts/` 폴더를 Unity 프로젝트에 복사**
3. **필요 패키지 설치**: UniTask, DOTween, TextMeshPro, Cinemachine
4. **ScriptableObject 에셋 생성**: Unity에서 `Create > MiniKingdom > ...` 메뉴 활용

> [!TIP]
> 추가 개발 가능한 작업:
> - 📱 Unity 씬(Scene) 구성 및 프리팹 생성
> - 🎨 픽셀아트 에셋 제작 (Aseprite)
> - 🗺️ 타일맵 기반 던전 비주얼 구현
> - 🔊 사운드/BGM 통합
> - 🧪 밸런스 시뮬레이터 제작
> - 📊 Firebase 백엔드 연동
