using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;
using TMPro;
using MiniKingdom.Core;
using MiniKingdom.Combat;
using MiniKingdom.Kingdom;
using MiniKingdom.Player;
using MiniKingdom.Enemy;
using MiniKingdom.Dungeon;
using MiniKingdom.Data;
using MiniKingdom.UI;
using MiniKingdom.UI.Screens;

namespace MiniKingdom.Editor
{
    [InitializeOnLoad]
    public class ProjectBuilder
    {
        static ProjectBuilder()
        {
            EditorApplication.delayCall += AutoSetupFontOnLoad;
        }

        private static void AutoSetupFontOnLoad()
        {
            if (!File.Exists("Assets/Fonts/MalgunGothic SDF.asset"))
            {
                Debug.Log("[AutoSetup] 한글 폰트 미생성 감지 -> SetupKoreanFont 자동 실행");
                SetupKoreanFont();
            }
        }

        [MenuItem("Mini Kingdom/🚀 1. 기본 씬 및 프리팹 자동 생성", false, 1)]
        public static void SetupProject()
        {
            Debug.Log("미니왕국 프로젝트 자동 구성을 시작합니다...");

            // 1. 필요한 폴더 확인 및 생성
            string[] folders = {
                "Assets/Scenes",
                "Assets/Prefabs",
                "Assets/Prefabs/Core",
                "Assets/Prefabs/UI",
                "Assets/Prefabs/Entities",
                "Assets/Data",
                "Assets/Data/Dungeons",
                "Assets/Data/Enemies",
                "Assets/Data/Skills",
                "Assets/Data/Buildings",
                "Assets/Data/Relics",
                "Assets/Data/Decrees",
                "Assets/Data/Weather"
            };

            foreach (string folder in folders)
            {
                EnsureFolder(folder);
            }

            // 2. 기본 프리팹 생성
            CreateGameCorePrefab();
            CreatePlayerPrefab();
            CreateEnemyPrefabs();
            CreateFloatingTextPrefab();

            // 3. 한글 폰트 설정 (씬 생성 전 미리 에셋 생성 및 TMP Settings 등록)
            SetupKoreanFont();

            // 4. 씬(Scene) 생성 및 오브젝트 구성
            CreateMainMenuScene();
            CreateKingdomScene();
            CreateDungeonScene();

            // 4. Build Settings에 씬 등록
            string[] scenePaths = {
                "Assets/Scenes/MainMenu.unity",
                "Assets/Scenes/Kingdom.unity",
                "Assets/Scenes/Dungeon.unity"
            };
            List<EditorBuildSettingsScene> buildScenes = new List<EditorBuildSettingsScene>();
            foreach (var path in scenePaths)
            {
                buildScenes.Add(new EditorBuildSettingsScene(path, true));
            }
            EditorBuildSettings.scenes = buildScenes.ToArray();
            Debug.Log("✅ Build Settings에 3개 씬 등록 완료 (MainMenu, Kingdom, Dungeon)");

            // 5. 한글 폰트 설정
            SetupKoreanFont();

            // 6. 기본으로 Kingdom 씬 열기
            EditorSceneManager.OpenScene("Assets/Scenes/Kingdom.unity");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("🎉 미니왕국 기본 씬 및 프리팹 자동 생성이 완료되었습니다!");
        }

        [MenuItem("Mini Kingdom/🏰 왕국 씬 다시 생성 (Fix Kingdom Scene)", false, 11)]
        public static void RebuildKingdomScene()
        {
            SetupKoreanFont();
            CreateKingdomScene();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("🎉 Kingdom 씬 재생성 완료 (Dungeon 출정 버튼 연결 완료)");
        }

        [MenuItem("Mini Kingdom/🔤 3. 한글 폰트(TMP Font Asset) 자동 생성 및 등록", false, 3)]
        public static void SetupKoreanFont()
        {
            Debug.Log("한글 폰트(Malgun Gothic) 자동 설정을 시작합니다...");
            string fontPath = "Assets/Fonts/MalgunGothic.ttf";
            string assetPath = "Assets/Fonts/MalgunGothic SDF.asset";

            if (!File.Exists(fontPath))
            {
                EnsureFolder("Assets/Fonts");
                if (File.Exists("C:/Windows/Fonts/malgun.ttf"))
                {
                    File.Copy("C:/Windows/Fonts/malgun.ttf", fontPath, true);
                    AssetDatabase.Refresh();
                }
            }

            var font = AssetDatabase.LoadAssetAtPath<Font>(fontPath);
            if (font == null)
            {
                Debug.LogWarning("Windows Malgun Gothic 폰트를 찾을 수 없습니다.");
                return;
            }

            var fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(assetPath);
            if (fontAsset == null)
            {
                fontAsset = TMP_FontAsset.CreateFontAsset(font, 36, 4, UnityEngine.TextCore.LowLevel.GlyphRenderMode.SDFAA, 512, 512, AtlasPopulationMode.Dynamic);
                AssetDatabase.CreateAsset(fontAsset, assetPath);
                AssetDatabase.SaveAssets();
                Debug.Log("✅ Dynamic MalgunGothic SDF 폰트 에셋 생성 완료!");
            }

            // TMP Settings에 Fallback 폰트로 등록
            var tmpSettings = Resources.Load<TMP_Settings>("TMP Settings");
            if (tmpSettings != null && fontAsset != null)
            {
                var fallbackField = typeof(TMP_Settings).GetField("m_fallbackFontAssets", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (fallbackField != null)
                {
                    var list = (List<TMP_FontAsset>)fallbackField.GetValue(tmpSettings);
                    if (list == null) list = new List<TMP_FontAsset>();
                    if (!list.Contains(fontAsset))
                    {
                        list.Add(fontAsset);
                        fallbackField.SetValue(tmpSettings, list);
                        EditorUtility.SetDirty(tmpSettings);
                    }
                }

                // Default Font로도 지정
                var defaultField = typeof(TMP_Settings).GetField("m_defaultFontAsset", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (defaultField != null)
                {
                    defaultField.SetValue(tmpSettings, fontAsset);
                    EditorUtility.SetDirty(tmpSettings);
                }

                AssetDatabase.SaveAssets();
                Debug.Log("✅ TMP Settings에 MalgunGothic SDF 기본 및 Fallback 폰트로 등록 완료!");
            }

            // 현재 씬의 모든 TextMeshProUGUI 컴포넌트 폰트 즉시 갱신
            if (fontAsset != null)
            {
                foreach (var tmp in Object.FindObjectsByType<TextMeshProUGUI>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                {
                    tmp.font = fontAsset;
                    EditorUtility.SetDirty(tmp);
                }
            }
        }

        [MenuItem("Mini Kingdom/📦 2. 기본 데이터 에셋(.asset) 일괄 생성", false, 2)]
        public static void GenerateDefaultDataAssets()
        {
            Debug.Log("미니왕국 기본 ScriptableObject 데이터 에셋 생성을 시작합니다...");

            // 1. 적(Enemy) 데이터 생성
            var slimeData = CreateOrLoadAsset<EnemyData>("Assets/Data/Enemies/Enemy_Slime.asset");
            slimeData.enemyId = "enemy_slime";
            slimeData.enemyName = "수풀 슬라임";
            slimeData.description = "자연 속성의 느리지만 끈질긴 슬라임.";
            slimeData.grade = EnemyGrade.Normal;
            slimeData.hp = 50f;
            slimeData.atk = 6f;
            slimeData.def = 2f;
            slimeData.moveSpeed = 2f;
            slimeData.attackSpeed = 0.8f;
            slimeData.weakness = DamageType.Physical;
            slimeData.resistance = DamageType.Nature;
            EditorUtility.SetDirty(slimeData);

            var wolfData = CreateOrLoadAsset<EnemyData>("Assets/Data/Enemies/Enemy_Wolf.asset");
            wolfData.enemyId = "enemy_wolf";
            wolfData.enemyName = "야생 늑대";
            wolfData.description = "물리 속성의 재빠른 돌진 몬스터.";
            wolfData.grade = EnemyGrade.Normal;
            wolfData.hp = 80f;
            wolfData.atk = 12f;
            wolfData.def = 4f;
            wolfData.moveSpeed = 3.5f;
            wolfData.attackSpeed = 1.2f;
            wolfData.weakness = DamageType.Magic;
            wolfData.resistance = DamageType.Physical;
            EditorUtility.SetDirty(wolfData);

            var bossTrollData = CreateOrLoadAsset<EnemyData>("Assets/Data/Enemies/Boss_Troll.asset");
            bossTrollData.enemyId = "boss_troll";
            bossTrollData.enemyName = "숲 트롤 (보스)";
            bossTrollData.description = "거대한 통나무를 휘두르는 수풀 숲의 수호자.";
            bossTrollData.grade = EnemyGrade.Boss;
            bossTrollData.hp = 600f;
            bossTrollData.atk = 24f;
            bossTrollData.def = 8f;
            bossTrollData.moveSpeed = 1.8f;
            bossTrollData.attackSpeed = 0.7f;
            bossTrollData.weakness = DamageType.Magic;
            bossTrollData.resistance = DamageType.Nature;
            EditorUtility.SetDirty(bossTrollData);

            // 2. 스킬(Skill) 데이터 생성
            var flameSword = CreateOrLoadAsset<SkillData>("Assets/Data/Skills/Skill_FlameSword.asset");
            flameSword.skillId = "skill_flame_sword";
            flameSword.skillName = "화염검";
            flameSword.description = "검에 화염을 둘러 공격력 +20% 증가 및 마법 화염 피해 부여.";
            flameSword.skillType = SkillType.Attack;
            flameSword.damageType = DamageType.Magic;
            flameSword.cooldown = 4f;
            flameSword.damageMultiplier = 1.5f;
            flameSword.maxLevel = 5;
            flameSword.isFusion = false;
            EditorUtility.SetDirty(flameSword);

            var whirlwind = CreateOrLoadAsset<SkillData>("Assets/Data/Skills/Skill_Whirlwind.asset");
            whirlwind.skillId = "skill_whirlwind";
            whirlwind.skillName = "회전 베기";
            whirlwind.description = "주변의 모든 적을 빠르게 베어 넘기는 물리 공격.";
            whirlwind.skillType = SkillType.Attack;
            whirlwind.damageType = DamageType.Physical;
            whirlwind.cooldown = 5f;
            whirlwind.damageMultiplier = 1.8f;
            whirlwind.maxLevel = 5;
            whirlwind.isFusion = false;
            EditorUtility.SetDirty(whirlwind);

            var kingWrath = CreateOrLoadAsset<SkillData>("Assets/Data/Skills/Skill_KingWrath.asset");
            kingWrath.skillId = "skill_king_wrath";
            kingWrath.skillName = "왕의 분노";
            kingWrath.description = "전방 부채꼴 범위에 공격력 300%의 강력한 일격을 날립니다.";
            kingWrath.skillType = SkillType.Attack;
            kingWrath.damageType = DamageType.Physical;
            kingWrath.cooldown = 12f;
            kingWrath.damageMultiplier = 3.0f;
            kingWrath.maxLevel = 5;
            kingWrath.isFusion = false;
            EditorUtility.SetDirty(kingWrath);

            var royalBarrier = CreateOrLoadAsset<SkillData>("Assets/Data/Skills/Skill_RoyalBarrier.asset");
            royalBarrier.skillId = "skill_royal_barrier";
            royalBarrier.skillName = "왕실 방벽";
            royalBarrier.description = "3초 동안 받는 피해량을 80% 감소시키는 보호막을 전개합니다.";
            royalBarrier.skillType = SkillType.Defense;
            royalBarrier.cooldown = 15f;
            royalBarrier.effectDuration = 3f;
            royalBarrier.maxLevel = 5;
            royalBarrier.isFusion = false;
            EditorUtility.SetDirty(royalBarrier);

            // 3. 융합 스킬(Skill Fusion) 생성
            var flameTornado = CreateOrLoadAsset<SkillData>("Assets/Data/Skills/Skill_FlameTornado.asset");
            flameTornado.skillId = "skill_fusion_flame_tornado";
            flameTornado.skillName = "🔥 화염 토네이도 (융합)";
            flameTornado.description = "화염검 + 회전 베기 융합! 주변을 불사르는 거대한 불꽃 폭풍 소환.";
            flameTornado.skillType = SkillType.Attack;
            flameTornado.damageType = DamageType.Magic;
            flameTornado.cooldown = 10f;
            flameTornado.damageMultiplier = 4.5f;
            flameTornado.maxLevel = 1;
            flameTornado.isFusion = true;
            EditorUtility.SetDirty(flameTornado);

            var fusionRecipe = CreateOrLoadAsset<SkillFusionData>("Assets/Data/Skills/Fusion_FlameTornado.asset");
            fusionRecipe.requiredSkill1_Id = "skill_flame_sword";
            fusionRecipe.requiredSkill2_Id = "skill_whirlwind";
            fusionRecipe.resultingSkill_Id = "skill_fusion_flame_tornado";
            EditorUtility.SetDirty(fusionRecipe);

            // 4. 던전(Dungeon) 데이터 생성
            var forestDungeon = CreateOrLoadAsset<DungeonData>("Assets/Data/Dungeons/Forest_Dungeon.asset");
            forestDungeon.dungeonId = "dungeon_forest";
            forestDungeon.dungeonName = "수풀 숲 (Forest)";
            forestDungeon.description = "울창한 숲속에 도사린 몬스터들을 물리치고 목재와 약초를 수집하세요.";
            forestDungeon.theme = DungeonTheme.Forest;
            forestDungeon.difficulty = 1;
            forestDungeon.unlockKingdomLevel = 1;
            forestDungeon.roomCountRange = new Vector2Int(10, 15);
            forestDungeon.primaryResources = new[] { MiniKingdom.Data.ResourceType.Wood, MiniKingdom.Data.ResourceType.Gold };
            forestDungeon.enemyPool = new[] { slimeData, wolfData };
            forestDungeon.boss = bossTrollData;
            forestDungeon.specialMechanicDescription = "덤불 지형 및 독성 버섯 기믹";
            EditorUtility.SetDirty(forestDungeon);

            // 5. 건물(Building) 데이터 생성
            var blacksmith = CreateOrLoadAsset<BuildingData>("Assets/Data/Buildings/Building_Blacksmith.asset");
            blacksmith.buildingId = "building_blacksmith";
            blacksmith.buildingName = "대장간";
            blacksmith.description = "시작 무기 등급을 상승시키고 장비 제작을 해금합니다.";
            blacksmith.category = BuildingCategory.Military;
            blacksmith.maxLevel = 5;
            blacksmith.unlockKingdomLevel = 1;
            blacksmith.levelData = new[] {
                new BuildingLevelData { buildTime = 5f, kingdomEffect = "시작 무기: 일반 등급" },
                new BuildingLevelData { buildTime = 15f, kingdomEffect = "시작 무기: 고급 등급" },
                new BuildingLevelData { buildTime = 30f, kingdomEffect = "시작 무기: 희귀 등급" }
            };
            EditorUtility.SetDirty(blacksmith);

            var magicTower = CreateOrLoadAsset<BuildingData>("Assets/Data/Buildings/Building_MagicTower.asset");
            magicTower.buildingId = "building_magic_tower";
            magicTower.buildingName = "마법탑";
            magicTower.description = "추가 마법 스킬 슬롯을 해금하고 마법 연구를 지원합니다.";
            magicTower.category = BuildingCategory.Magic;
            magicTower.maxLevel = 5;
            magicTower.unlockKingdomLevel = 1;
            magicTower.levelData = new[] {
                new BuildingLevelData { buildTime = 8f, kingdomEffect = "마법 스킬 슬롯 1개 해금" },
                new BuildingLevelData { buildTime = 20f, kingdomEffect = "마법 스킬 슬롯 2개 해금" }
            };
            EditorUtility.SetDirty(magicTower);

            var farm = CreateOrLoadAsset<BuildingData>("Assets/Data/Buildings/Building_Farm.asset");
            farm.buildingId = "building_farm";
            farm.buildingName = "농장";
            farm.description = "던전 입장 시 체력 음식 버프를 부여하고 식량을 자동 생산합니다.";
            farm.category = BuildingCategory.Production;
            farm.maxLevel = 5;
            farm.unlockKingdomLevel = 1;
            farm.levelData = new[] {
                new BuildingLevelData { buildTime = 5f, kingdomEffect = "던전 입장 시 HP +20 버프" }
            };
            EditorUtility.SetDirty(farm);

            // 6. 왕의 칙령(Decree) 데이터 생성
            var decreeWood = CreateOrLoadAsset<RoyalDecreeData>("Assets/Data/Decrees/Decree_WoodDay.asset");
            decreeWood.decreeId = "decree_wood_day";
            decreeWood.decreeName = "목재의 날";
            decreeWood.description = "오늘 하루 동안 던전에서 획득하는 목재 수량이 2배로 증가합니다.";
            decreeWood.effectsDescription = "목재 획득량 2배";
            decreeWood.buffValues = new[] { 2.0f };
            EditorUtility.SetDirty(decreeWood);

            var decreeWarrior = CreateOrLoadAsset<RoyalDecreeData>("Assets/Data/Decrees/Decree_WarriorDay.asset");
            decreeWarrior.decreeId = "decree_warrior_day";
            decreeWarrior.decreeName = "전사의 날";
            decreeWarrior.description = "오늘 하루 동안 모든 던전에서 기본 공격력이 20% 증가합니다.";
            decreeWarrior.effectsDescription = "기본 공격력 +20%";
            decreeWarrior.buffValues = new[] { 0.2f };
            EditorUtility.SetDirty(decreeWarrior);

            var decreeMerchant = CreateOrLoadAsset<RoyalDecreeData>("Assets/Data/Decrees/Decree_MerchantDay.asset");
            decreeMerchant.decreeId = "decree_merchant_day";
            decreeMerchant.decreeName = "상인의 날";
            decreeMerchant.description = "오늘 하루 동안 던전 클리어 시 골드 획득량이 1.5배로 증가합니다.";
            decreeMerchant.effectsDescription = "골드 획득량 1.5배";
            decreeMerchant.buffValues = new[] { 1.5f };
            EditorUtility.SetDirty(decreeMerchant);

            // 7. 유물(Relic) 데이터 생성
            var relicGlove = CreateOrLoadAsset<RelicData>("Assets/Data/Relics/Relic_WarriorGlove.asset");
            relicGlove.id = "relic_warrior_glove";
            relicGlove.relicName = "전사의 장갑";
            relicGlove.description = "공격력 +15% 증가";
            relicGlove.tier = RelicTier.Common;
            relicGlove.effects = new[] {
                new StatModifier { statType = MiniKingdom.Data.StatType.ATK, value = 0.15f, isPercentage = true }
            };
            EditorUtility.SetDirty(relicGlove);

            var relicGlass = CreateOrLoadAsset<RelicData>("Assets/Data/Relics/Relic_GlassCannon.asset");
            relicGlass.id = "relic_glass_cannon";
            relicGlass.relicName = "유리 대포 (저주받은 유물)";
            relicGlass.description = "공격력 +100% 증가하지만 받는 피해량 +50%";
            relicGlass.tier = RelicTier.Epic;
            relicGlass.effects = new[] {
                new StatModifier { statType = MiniKingdom.Data.StatType.ATK, value = 1.0f, isPercentage = true }
            };
            EditorUtility.SetDirty(relicGlass);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("🎉 미니왕국 기본 데이터 에셋 생성 완료! (Dungeon, Enemy, Skill, Fusion, Building, Decree, Relic)");
        }

        private static void CreateGameCorePrefab()
        {
            string path = "Assets/Prefabs/Core/GameCore.prefab";
            if (File.Exists(path)) return;

            GameObject root = new GameObject("GameCore");
            root.AddComponent<GameManager>();
            root.AddComponent<SaveManager>();
            root.AddComponent<ResourceManager>();
            root.AddComponent<BuildingManager>();
            root.AddComponent<DailyBonusSystem>();
            root.AddComponent<RoyalDecreeSystem>();
            root.AddComponent<KingdomDefenseSystem>();
            root.AddComponent<TaxSystem>();
            root.AddComponent<WeatherSystem>();
            root.AddComponent<CombatSystem>();
            root.AddComponent<LevelUpSystem>();
            root.AddComponent<UIManager>();

            PrefabUtility.SaveAsPrefabAsset(root, path);
            GameObject.DestroyImmediate(root);
            Debug.Log("✅ GameCore.prefab 생성 완료");
        }

        private static void CreatePlayerPrefab()
        {
            string path = "Assets/Prefabs/Entities/Player.prefab";
            if (File.Exists(path)) return;

            GameObject player = new GameObject("Player");
            player.tag = "Player";

            var rb = player.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.freezeRotation = true;

            var col = player.AddComponent<CircleCollider2D>();
            col.radius = 0.5f;

            player.AddComponent<SpriteRenderer>();
            player.AddComponent<PlayerStats>();
            player.AddComponent<PlayerController>();
            player.AddComponent<PlayerInventory>();

            PrefabUtility.SaveAsPrefabAsset(player, path);
            GameObject.DestroyImmediate(player);
            Debug.Log("✅ Player.prefab 생성 완료");
        }

        private static void CreateEnemyPrefabs()
        {
            string slimePath = "Assets/Prefabs/Entities/Enemy_Slime.prefab";
            if (!File.Exists(slimePath))
            {
                GameObject slime = new GameObject("Enemy_Slime");
                slime.tag = "Enemy";
                var rb = slime.AddComponent<Rigidbody2D>();
                rb.gravityScale = 0f;
                rb.freezeRotation = true;
                var col = slime.AddComponent<CircleCollider2D>();
                col.radius = 0.4f;
                slime.AddComponent<SpriteRenderer>();
                slime.AddComponent<EnemyController>();
                slime.AddComponent<MiniKingdom.Items.LootManager>();

                PrefabUtility.SaveAsPrefabAsset(slime, slimePath);
                GameObject.DestroyImmediate(slime);
                Debug.Log("✅ Enemy_Slime.prefab 생성 완료");
            }

            string trollPath = "Assets/Prefabs/Entities/Boss_Troll.prefab";
            if (!File.Exists(trollPath))
            {
                GameObject troll = new GameObject("Boss_Troll");
                troll.tag = "Enemy";
                var rb = troll.AddComponent<Rigidbody2D>();
                rb.gravityScale = 0f;
                rb.freezeRotation = true;
                var col = troll.AddComponent<BoxCollider2D>();
                col.size = new Vector2(1.5f, 1.5f);
                troll.AddComponent<SpriteRenderer>();
                troll.AddComponent<BossController>();
                troll.AddComponent<MiniKingdom.Items.LootManager>();

                PrefabUtility.SaveAsPrefabAsset(troll, trollPath);
                GameObject.DestroyImmediate(troll);
                Debug.Log("✅ Boss_Troll.prefab 생성 완료");
            }
        }

        private static void CreateFloatingTextPrefab()
        {
            string path = "Assets/Prefabs/UI/FloatingText.prefab";
            if (File.Exists(path)) return;

            GameObject ftObj = new GameObject("FloatingText");
            var rect = ftObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(100, 40);

            var textMesh = ftObj.AddComponent<TextMeshProUGUI>();
            textMesh.fontSize = 24;
            textMesh.alignment = TextAlignmentOptions.Center;

            ftObj.AddComponent<FloatingText>();

            PrefabUtility.SaveAsPrefabAsset(ftObj, path);
            GameObject.DestroyImmediate(ftObj);
            Debug.Log("✅ FloatingText.prefab 생성 완료");
        }

        private static void CreateMainMenuScene()
        {
            string scenePath = "Assets/Scenes/MainMenu.unity";
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "MainMenu";

            // Camera
            GameObject camGO = new GameObject("Main Camera");
            var cam = camGO.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 5f;
            cam.backgroundColor = new Color(0.12f, 0.14f, 0.2f);
            cam.clearFlags = CameraClearFlags.SolidColor;
            camGO.AddComponent<AudioListener>();
            camGO.tag = "MainCamera";

            // Canvas & EventSystem
            GameObject canvasGO = new GameObject("Canvas");
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();

            GameObject esGO = new GameObject("EventSystem");
            esGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

            // Title
            GameObject titleGO = new GameObject("TitleText");
            titleGO.transform.SetParent(canvasGO.transform, false);
            var titleText = titleGO.AddComponent<TextMeshProUGUI>();
            titleText.text = "미니왕국 (Mini Kingdom)";
            titleText.fontSize = 44;
            titleText.alignment = TextAlignmentOptions.Center;
            var titleRect = titleGO.GetComponent<RectTransform>();
            titleRect.anchoredPosition = new Vector2(0, 100);
            titleRect.sizeDelta = new Vector2(600, 100);

            // Subtitle
            GameObject subGO = new GameObject("SubText");
            subGO.transform.SetParent(canvasGO.transform, false);
            var subText = subGO.AddComponent<TextMeshProUGUI>();
            subText.text = "왕국의 부흥을 위해 칼을 든 국왕의 모험!";
            subText.fontSize = 20;
            subText.color = new Color(0.8f, 0.8f, 0.8f);
            subText.alignment = TextAlignmentOptions.Center;
            var subRect = subGO.GetComponent<RectTransform>();
            subRect.anchoredPosition = new Vector2(0, 30);
            subRect.sizeDelta = new Vector2(500, 50);

            // Start Button
            GameObject btnGO = new GameObject("StartButton");
            btnGO.transform.SetParent(canvasGO.transform, false);
            var btnImage = btnGO.AddComponent<Image>();
            btnImage.color = new Color(0.2f, 0.65f, 0.35f);
            var btn = btnGO.AddComponent<Button>();
            var menuCtrl = btnGO.AddComponent<MainMenuController>();
            UnityEditor.Events.UnityEventTools.AddPersistentListener(btn.onClick, menuCtrl.OnClickStart);

            var btnRect = btnGO.GetComponent<RectTransform>();
            btnRect.anchoredPosition = new Vector2(0, -70);
            btnRect.sizeDelta = new Vector2(260, 65);

            GameObject btnTextGO = new GameObject("Text");
            btnTextGO.transform.SetParent(btnGO.transform, false);
            var btnText = btnTextGO.AddComponent<TextMeshProUGUI>();
            btnText.text = "왕국 입장하기";
            btnText.fontSize = 24;
            btnText.alignment = TextAlignmentOptions.Center;

            // Instantiate Core Manager
            InstantiateGameCoreInScene();

            EditorSceneManager.SaveScene(scene, scenePath);
            Debug.Log("✅ MainMenu 씬 생성 완료");
        }

        private static void CreateKingdomScene()
        {
            string scenePath = "Assets/Scenes/Kingdom.unity";
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "Kingdom";

            // Camera
            GameObject camGO = new GameObject("Main Camera");
            var cam = camGO.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 6f;
            cam.backgroundColor = new Color(0.18f, 0.25f, 0.2f);
            cam.clearFlags = CameraClearFlags.SolidColor;
            camGO.AddComponent<AudioListener>();
            camGO.tag = "MainCamera";

            // Canvas & EventSystem
            GameObject canvasGO = new GameObject("Canvas");
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();

            GameObject esGO = new GameObject("EventSystem");
            esGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

            var kingdomScreenGO = new GameObject("KingdomScreen");
            kingdomScreenGO.transform.SetParent(canvasGO.transform, false);
            var kingdomScreen = kingdomScreenGO.AddComponent<KingdomScreen>();

            // Title
            GameObject titleGO = new GameObject("KingdomTitle");
            titleGO.transform.SetParent(canvasGO.transform, false);
            var titleText = titleGO.AddComponent<TextMeshProUGUI>();
            titleText.text = "미니 왕국 중앙 광장";
            titleText.fontSize = 36;
            titleText.alignment = TextAlignmentOptions.Center;
            var titleRect = titleGO.GetComponent<RectTransform>();
            titleRect.anchoredPosition = new Vector2(0, 180);
            titleRect.sizeDelta = new Vector2(500, 60);

            // Explore / Dungeon Go Button
            GameObject goDungeonBtnGO = new GameObject("ExploreButton");
            goDungeonBtnGO.transform.SetParent(canvasGO.transform, false);
            var goDungeonImg = goDungeonBtnGO.AddComponent<Image>();
            goDungeonImg.color = new Color(0.85f, 0.3f, 0.25f);
            var goDungeonBtn = goDungeonBtnGO.AddComponent<Button>();
            var goDungeonRect = goDungeonBtnGO.GetComponent<RectTransform>();
            goDungeonRect.anchoredPosition = new Vector2(0, -60);
            goDungeonRect.sizeDelta = new Vector2(300, 75);

            GameObject dTextGO = new GameObject("Text");
            dTextGO.transform.SetParent(goDungeonBtnGO.transform, false);
            var dText = dTextGO.AddComponent<TextMeshProUGUI>();
            dText.text = "던전 출정하기 (Dungeon)";
            dText.fontSize = 22;
            dText.alignment = TextAlignmentOptions.Center;
            dText.raycastTarget = false;

            // Wire up persistent onClick listener with KingdomSceneController
            var kingdomCtrl = goDungeonBtnGO.AddComponent<KingdomSceneController>();
            UnityEditor.Events.UnityEventTools.AddPersistentListener(goDungeonBtn.onClick, kingdomCtrl.OnClickDepartDungeon);

            // Wire up explore tab button
            var fieldExplore = typeof(KingdomScreen).GetField("exploreTabBtn", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (fieldExplore != null) fieldExplore.SetValue(kingdomScreen, goDungeonBtn);

            InstantiateGameCoreInScene();

            EditorSceneManager.SaveScene(scene, scenePath);
            Debug.Log("✅ Kingdom 씬 생성 완료");
        }

        private static void CreateDungeonScene()
        {
            string scenePath = "Assets/Scenes/Dungeon.unity";
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "Dungeon";

            // Camera
            GameObject camGO = new GameObject("Main Camera");
            var cam = camGO.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 6f;
            cam.backgroundColor = new Color(0.08f, 0.08f, 0.12f);
            cam.clearFlags = CameraClearFlags.SolidColor;
            camGO.AddComponent<AudioListener>();
            camGO.tag = "MainCamera";

            // EventSystem
            GameObject esGO = new GameObject("EventSystem");
            esGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

            // Dungeon Systems Root
            GameObject dungeonSys = new GameObject("DungeonRunSystem");
            dungeonSys.AddComponent<DungeonGenerator>();
            dungeonSys.AddComponent<DungeonRunManager>();
            dungeonSys.AddComponent<RoomManager>();
            var spawner = dungeonSys.AddComponent<EnemySpawner>();

            // Wire default enemy prefabs into spawner
            var enemySlimePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Entities/Enemy_Slime.prefab");
            var bossTrollPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Entities/Boss_Troll.prefab");
            var fieldDefEnemy = typeof(EnemySpawner).GetField("defaultEnemyPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (fieldDefEnemy != null && enemySlimePrefab != null) fieldDefEnemy.SetValue(spawner, enemySlimePrefab);
            var fieldBoss = typeof(EnemySpawner).GetField("bossPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (fieldBoss != null && bossTrollPrefab != null) fieldBoss.SetValue(spawner, bossTrollPrefab);

            // Canvas & HUD
            GameObject canvasGO = new GameObject("DungeonCanvas");
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();

            var hudGO = new GameObject("DungeonHUD");
            hudGO.transform.SetParent(canvasGO.transform, false);
            hudGO.AddComponent<DungeonHUDScreen>();

            var levelUpGO = new GameObject("LevelUpScreen");
            levelUpGO.transform.SetParent(canvasGO.transform, false);
            levelUpGO.AddComponent<LevelUpScreen>();
            levelUpGO.SetActive(false);

            var resultGO = new GameObject("RunResultScreen");
            resultGO.transform.SetParent(canvasGO.transform, false);
            resultGO.AddComponent<RunResultScreen>();
            resultGO.SetActive(false);

            var branchGO = new GameObject("BranchSelectionScreen");
            branchGO.transform.SetParent(canvasGO.transform, false);
            branchGO.AddComponent<BranchSelectionScreen>();
            branchGO.SetActive(false);

            var restGO = new GameObject("RestRoomScreen");
            restGO.transform.SetParent(canvasGO.transform, false);
            restGO.AddComponent<RestRoomScreen>();
            restGO.SetActive(false);

            var shopGO = new GameObject("DungeonShopScreen");
            shopGO.transform.SetParent(canvasGO.transform, false);
            shopGO.AddComponent<DungeonShopScreen>();
            shopGO.SetActive(false);

            var fishingGO = new GameObject("FishingMinigameUI");
            fishingGO.transform.SetParent(canvasGO.transform, false);
            fishingGO.AddComponent<FishingMinigameUI>();
            fishingGO.SetActive(false);

            var relicGO = new GameObject("RelicPopupUI");
            relicGO.transform.SetParent(canvasGO.transform, false);
            relicGO.AddComponent<RelicPopupUI>();
            relicGO.SetActive(false);

            // Place Player in scene
            var playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Entities/Player.prefab");
            if (playerPrefab != null)
            {
                var p = (GameObject)PrefabUtility.InstantiatePrefab(playerPrefab);
                p.transform.position = Vector3.zero;
            }

            InstantiateGameCoreInScene();

            EditorSceneManager.SaveScene(scene, scenePath);
            Debug.Log("✅ Dungeon 씬 생성 완료");
        }

        private static void InstantiateGameCoreInScene()
        {
            var corePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Core/GameCore.prefab");
            if (corePrefab != null)
            {
                PrefabUtility.InstantiatePrefab(corePrefab);
            }
        }

        private static void EnsureFolder(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                string parent = Path.GetDirectoryName(path).Replace("\\", "/");
                string folderName = Path.GetFileName(path);
                AssetDatabase.CreateFolder(parent, folderName);
            }
        }

        private static T CreateOrLoadAsset<T>(string path) where T : ScriptableObject
        {
            string dir = Path.GetDirectoryName(path).Replace("\\", "/");
            EnsureFolderRecursive(dir);

            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(asset, path);
            }
            return asset;
        }

        private static void EnsureFolderRecursive(string folderPath)
        {
            if (AssetDatabase.IsValidFolder(folderPath)) return;
            string parent = Path.GetDirectoryName(folderPath).Replace("\\", "/");
            if (!string.IsNullOrEmpty(parent) && !AssetDatabase.IsValidFolder(parent))
            {
                EnsureFolderRecursive(parent);
            }
            string folderName = Path.GetFileName(folderPath);
            AssetDatabase.CreateFolder(parent, folderName);
        }
    }
}
