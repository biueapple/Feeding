# 🍽️ Feeding

> 용사의 **집사**가 되어 먹이고, 입히고, 전투에 내보내는 방치형 RPG

---

## 🎮 게임 소개

**Feeding**은 플레이어가 용사를 직접 조작하지 않는 방치형 RPG입니다. 용사가 던전에서 자동으로 싸우는 동안 플레이어는 집에서 방문객과 거래하고, 다음 날을 위해 장비와 음식을 준비합니다.

하루는 다음과 같은 사이클로 진행됩니다:

```
아침
  → 용사가 상자에서 장비 챙김
  → 던전으로 출발
      ↓
  [용사 자동 전투가 진행되는 동안 방문객과 거래]
      ↓
저녁
  → 용사 귀환
  → 장비 해제 & 전리품 회수
  → 식사
  → 취침 → 다음 날
```

---

## ✨ 주요 기능

### ⚔️ 자동 전투 시스템
- 용사와 적이 공격속도(`AS` 스탯) 기반 타이머로 자동 공격
- 데미지 이벤트 파이프라인: `OnAttackBefore → Hit → OnTakeDamageBefore → CalculateDamage → OnTakeDamageAfter`
- **물리 / 마법 / 고정** 데미지 타입, 각각 `DEF` / `RES` 스탯으로 감소
- 치명타 확률(`CC`) / 치명타 피해(`CD`) 스탯 기반 크리티컬 시스템

### 🧪 버프 & 디버프 시스템
- `Buff` ScriptableObject 기반 추상 클래스 + `BuffAdministrator`로 인스턴스 관리
- `IDisposable` 기반 구독 관리 — 버프 제거 시 이벤트 자동 해제
- 구현된 상태이상:
  - **DOT** — 초당 지속 피해 (기본)
  - **독(Poison)** — 스택 DOT + 최대 30스택까지 치유량 감소
  - **출혈(Bleeding)** — 공격 시 발동, 스택 소모
  - **화상(Burn)** — 피격 시 발동
  - **감전(Electrocute)** — 재적용 시 누적 스택 전량 폭발
  - **스탯 버프(Buff_StatModifier)** — 밤이 되면 만료되는 임시 스탯 상승

### 🎒 인벤토리 & 장비 시스템
- 슬롯 기반 인벤토리 + `ItemSlot` 조건 콜백 (타입 필터링, 드래그앤드롭 유효성 검사)
- 장비 부위 6종: 투구 / 흉갑 / 다리 / 신발 / 장신구 / 무기
- **세트 효과 시스템**: 2세트 / 4세트 효과 자동 추적 및 적용/제거
- `EquipmentEffect` ScriptableObject 기반 모듈형 장비 효과:
  - `AddDamageEffect` — 공격 시 고정 피해 추가 (크리티컬 적용)
  - `AddAttackEffect` — 피격 시 추가 공격 발동
  - `AddDerivationEffect` — 장착 시 스탯 수치 상승

### 🛒 거래 & 흥정 시스템
- 방문객이 줄을 서서(`LineUP<Visitor>`) 카운터까지 걸어와 거래 시작
- 거래 모드 2종: **판매** (방문객이 플레이어에게 팜) / **구매** (방문객이 플레이어 물건을 삼)
- `MaxRounds`, `Generosity`, `ConcedePerRound` 기반의 흥정 세션
- 가격 책정 파이프라인 (`FlatADD` / `PercentADD` / `PercentMul` 연산자 조합)
- 가격 이벤트 예시: **가뭄** (곡물 +20%), **축제 할인** (무기 고정 할인), **희귀도 추가금**

### 💬 대화 시스템
- `VisitorDialoguePack` + `DialogueService`의 규칙 기반 대사 선택
- 필터 조건: 이벤트 종류, 거래 타입, 카테고리 선호도 (선호/비선호/중립), 거래 결과, 시도 횟수
- 우선순위 정렬 후 동점 중 랜덤 선택
- 토큰 치환: `{visitor}`, `{item}`, `{price}`, `{spread}`, `{gen}`, `{diff}` 등

### 🗺️ 마을 시스템
- `WorldContext`로 여러 마을 중 하나를 선택해 모험 지역 결정
- 마을마다 등장 적 / 수출 아이템 (저렴) / 수입 아이템 (비쌈) 설정
- 툴팁으로 마을 정보 (등장 몬스터, 드랍 아이템, 수출입) 미리 확인 가능

### 🔧 UI 시스템
- **툴팁 시스템**: Header / KeyValueList / BulletList / Footer / Sprite / Divider 렌더러를 오브젝트 풀링으로 조합
- **드래그앤드롭 인벤토리**: 조건 검사 후 슬롯 스왑
- **옷장 UI / 보관함 UI**: 동적 생성, 풀링, 화면 경계 클램핑
- **버프바**: `BuffAdministrator` 이벤트와 자동 동기화
- **HP바**: 이벤트 기반 실시간 갱신

### 🌐 다국어 지원
- `LocalizationManager` + `LanguageTable` ScriptableObject (한국어 / 영어)
- `LocalizedText` 컴포넌트 — 언어 변경 이벤트 구독으로 자동 갱신

### ⚙️ 기타 시스템
- `PlayerSetting` — JSON 기반 설정 저장/로드 (언어, 해상도, 전체화면, 음량)
- `SoundManager` — AudioMixer 기반 볼륨 제어 + enum 키 클립 딕셔너리
- `RarityManager` — 희귀도별 색상 팔레트 (일반 → 전설)
- `ItemCollector` — `OnValidate`로 자동 등록되는 아이템 카탈로그
- `PointerGestureRaycaster` — Unity Input System 기반 클릭 / 더블클릭 / 롱프레스 / 드래그 처리

---

## 🗂️ 프로젝트 구조

```
Assets/Script/
├── Adventure/          # AdventureManager — 전투 루프 코루틴
├── Describes/          # IDescribesSelf, IStatSource 인터페이스
├── Interaction/        # 클릭/드래그/롱프레스 레이캐스터 + Chest/Closet
├── Inventory/          # Inventory, InventoryInterface, InventoryManager, ItemSlot
├── Item/
│   ├── Attribute/      # ItemAttribute, EquipmentAttribute, FoodAttribute
│   ├── Editor/         # Item, EquipmentAttribute 커스텀 인스펙터
│   ├── Effects/        # EquipmentEffect 하위 클래스 (AddDamage, AddAttack, AddDerivation)
│   └── SetSO/          # EquipmentSetSO (2/4세트 효과)
├── Language/           # LocalizationManager, LanguageTable, LocalizedText
├── Setting/            # PlayerSetting, PlayerSettingData
├── Sound/              # SoundManager, UISoundBinder
├── Trade/
│   ├── TradePipe/      # 단계별 거래 흐름 (도착 → 둘러보기 → 활성화 → 제출 → 판정 → 작별)
│   └── ...             # TradeService, HaggleSession, TradeRequest, PricingService
├── UI/
│   ├── Character/      # HPbar, Buffbar, BuffIcon
│   ├── Tooltip/        # 툴팁 렌더러 시스템
│   └── ...             # UIManager, DragSlotUI, ItemSlotUI, StorageUI, ClosetUI 등
├── Unit/
│   ├── Buff/           # Buff, BuffAdministrator, BuffInstance + 상태이상 구현체들
│   ├── Damage/         # DamagePacket, DamageType
│   ├── Enemy/          # Enemy 베이스 + Bat, Crab, Goblin, Rat + LootEntry
│   ├── Equipment/      # Equipment, EquipmentPart, IEquipCondition
│   ├── Payload/        # AttackEventArgs, RecoveryEventArgs, RecoveryPacket
│   └── Stat/           # DerivationStat, FoundationStat, StatModifier, StatCollector
├── Village/            # VillageSO, Village, WorldContext
├── Visitor/
│   ├── Price/          # IPriceModifier, PriceModifierHub, PricingService, PriceQuote
│   └── ...             # Visitor, VisitorSO, VisitorManager, DialogueService, DialoguePack
├── DayCycleManager.cs  # 하루 사이클 오케스트레이터
├── GameManager.cs      # 게임 상태, 입력, 씬 진입점
├── IngestionManager.cs # 식사 페이즈 (음식 슬롯 → FoodAttribute.Apply)
└── LineUP.cs           # 제네릭 방문객 대기열 + 오브젝트 풀링
```

---

## 🛠️ 기술 스택

| 항목 | 내용 |
|------|------|
| 엔진 | Unity (2D) |
| 언어 | C# (.NET) |
| 입력 | Unity Input System |
| 오디오 | Unity AudioMixer |
| UI | Unity uGUI + TextMeshPro |
| 데이터 | ScriptableObject (아이템, 버프, 방문객, 마을, 효과 등) |
| 저장 | `JsonUtility` (설정 데이터) |

---

## 👤 제작

1인 개발 프로젝트 — 프로그래밍, 시스템 설계, 아키텍처 전담 **biueapple**
