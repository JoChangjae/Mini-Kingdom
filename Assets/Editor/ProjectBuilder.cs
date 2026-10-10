using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;
using TMPro;
using UnityEngine.TextCore;
using UnityEngine.TextCore.LowLevel;
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
    public class ProjectBuilder
    {
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

            // 5. 기본으로 Kingdom 씬 열기
            EditorSceneManager.OpenScene("Assets/Scenes/Kingdom.unity");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("🎉 미니왕국 기본 씬 및 프리팹 자동 생성이 완료되었습니다!");
        }

        [MenuItem("Mini Kingdom/🏰 왕국 씬 다시 생성 (Fix Kingdom Scene)", false, 11)]
        public static void RebuildKingdomScene()
        {
            CreateKingdomScene();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("🎉 Kingdom 씬 재생성 완료 (건물 카드 및 강화 시스템 연동 완료)");
        }

        [MenuItem("Mini Kingdom/⚔️ 2. 던전 전투 및 몬스터 일괄 재구축 (Rebuild Combat)", false, 12)]
        public static void RebuildDungeonCombat()
        {
            GenerateGameSprites();
            CreateEnemyPrefabs();
            CreatePlayerPrefab();
            CreateFloatingTextPrefab();
            CreateDungeonScene();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("🎉 던전 전투 시스템, 몬스터 스프라이트, 보스전 및 HUD 완벽 재구축 완료!");
        }

        [MenuItem("Mini Kingdom/🔤 3. 한글 폰트(TMP Font Asset) 안내", false, 3)]
        public static void SetupKoreanFont()
        {
            Debug.Log("ℹ️ TMP Settings가 안정적인 기본 폰트로 설정되어 있습니다.");
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
            blacksmith.BuffModifiers = new List<StatModifier> {
                new StatModifier { statType = MiniKingdom.Data.StatType.ATK, value = 5f, isPercentage = false, source = ModifierSource.Building }
            };
            blacksmith.levelData = new[] {
                new BuildingLevelData { buildTime = 5f, kingdomEffect = "플레이어 공격력 +5" },
                new BuildingLevelData { buildTime = 15f, kingdomEffect = "플레이어 공격력 +10" },
                new BuildingLevelData { buildTime = 30f, kingdomEffect = "플레이어 공격력 +15" }
            };
            EditorUtility.SetDirty(blacksmith);

            var barracks = CreateOrLoadAsset<BuildingData>("Assets/Data/Buildings/Building_Barracks.asset");
            barracks.buildingId = "building_barracks";
            barracks.buildingName = "훈련소";
            barracks.description = "기초 체력과 방어력을 훈련하여 던전 생존력을 영구 강화합니다.";
            barracks.category = BuildingCategory.Military;
            barracks.maxLevel = 5;
            barracks.unlockKingdomLevel = 1;
            barracks.BuffModifiers = new List<StatModifier> {
                new StatModifier { statType = MiniKingdom.Data.StatType.HP, value = 25f, isPercentage = false, source = ModifierSource.Building },
                new StatModifier { statType = MiniKingdom.Data.StatType.DEF, value = 2f, isPercentage = false, source = ModifierSource.Building }
            };
            barracks.levelData = new[] {
                new BuildingLevelData { buildTime = 5f, kingdomEffect = "체력 +25, 방어력 +2" },
                new BuildingLevelData { buildTime = 15f, kingdomEffect = "체력 +50, 방어력 +4" }
            };
            EditorUtility.SetDirty(barracks);

            var magicTower = CreateOrLoadAsset<BuildingData>("Assets/Data/Buildings/Building_MagicTower.asset");
            magicTower.buildingId = "building_magic_tower";
            magicTower.buildingName = "마법탑";
            magicTower.description = "추가 마법 스킬 슬롯을 해금하고 마법 연구를 지원합니다.";
            magicTower.category = BuildingCategory.Magic;
            magicTower.maxLevel = 5;
            magicTower.unlockKingdomLevel = 1;
            magicTower.BuffModifiers = new List<StatModifier> {
                new StatModifier { statType = MiniKingdom.Data.StatType.SPD, value = 0.10f, isPercentage = true, source = ModifierSource.Building }
            };
            magicTower.levelData = new[] {
                new BuildingLevelData { buildTime = 8f, kingdomEffect = "공격 속도 +10%" },
                new BuildingLevelData { buildTime = 20f, kingdomEffect = "공격 속도 +20%" }
            };
            EditorUtility.SetDirty(magicTower);

            var farm = CreateOrLoadAsset<BuildingData>("Assets/Data/Buildings/Building_Farm.asset");
            farm.buildingId = "building_farm";
            farm.buildingName = "농장";
            farm.description = "던전 입장 시 체력 음식 버프를 부여하고 식량을 자동 생산합니다.";
            farm.category = BuildingCategory.Production;
            farm.maxLevel = 5;
            farm.unlockKingdomLevel = 1;
            farm.BuffModifiers = new List<StatModifier> {
                new StatModifier { statType = MiniKingdom.Data.StatType.HP, value = 15f, isPercentage = false, source = ModifierSource.Building }
            };
            farm.levelData = new[] {
                new BuildingLevelData { buildTime = 5f, kingdomEffect = "던전 입장 시 HP +15 버프" }
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

        public static void GenerateGameSprites()
        {
            EnsureFolder("Assets/Sprites");

            CreatePlayerSprite("Assets/Sprites/spr_player.png");
            CreateSlimeSprite("Assets/Sprites/spr_slime.png");
            CreateWolfSprite("Assets/Sprites/spr_wolf.png");
            CreateBossTrollSprite("Assets/Sprites/spr_boss_troll.png");
            CreateSlashSprite("Assets/Sprites/spr_slash.png");

            AssetDatabase.Refresh();
        }

        private static void SaveSpritePNG(Texture2D tex, string path, int ppu = 32)
        {
            byte[] bytes = tex.EncodeToPNG();
            File.WriteAllBytes(path, bytes);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spritePixelsPerUnit = ppu;
                importer.filterMode = FilterMode.Point;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.SaveAndReimport();
            }
        }

        private static void CreatePlayerSprite(string path)
        {
            if (File.Exists(path)) return;
            int size = 64;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Color[] colors = new Color[size * size];
            for (int i = 0; i < colors.Length; i++) colors[i] = Color.clear;

            Color gold = new Color(1f, 0.84f, 0.0f);
            Color crownJewel = new Color(0.9f, 0.2f, 0.2f);
            Color silver = new Color(0.75f, 0.8f, 0.85f);
            Color blueTunic = new Color(0.18f, 0.45f, 0.72f);
            Color cape = new Color(0.7f, 0.15f, 0.15f);
            Color skin = new Color(1f, 0.85f, 0.72f);
            Color dark = new Color(0.15f, 0.18f, 0.22f);
            Color steel = new Color(0.85f, 0.9f, 0.95f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    if (y >= 14 && y <= 40 && ((x >= 14 && x <= 22) || (x >= 42 && x <= 50)))
                        colors[y * size + x] = cape;
                    else if (y >= 6 && y <= 16 && ((x >= 24 && x <= 29) || (x >= 35 && x <= 40)))
                        colors[y * size + x] = dark;
                    else if (y >= 16 && y <= 34 && x >= 22 && x <= 42)
                        colors[y * size + x] = blueTunic;
                    else if (y >= 35 && y <= 46 && x >= 24 && x <= 40)
                        colors[y * size + x] = (y <= 40 && x >= 27 && x <= 37) ? skin : silver;
                    else if (y >= 47 && y <= 54 && x >= 23 && x <= 41)
                        colors[y * size + x] = (y >= 52 && (x == 24 || x == 32 || x == 40)) ? crownJewel : gold;
                    else if (y >= 16 && y <= 48 && x >= 45 && x <= 48)
                        colors[y * size + x] = (y <= 22) ? gold : steel;
                    else if (y >= 20 && y <= 34 && x >= 14 && x <= 21)
                        colors[y * size + x] = gold;
                }
            }

            tex.SetPixels(colors);
            tex.Apply();
            SaveSpritePNG(tex, path, 32);
        }

        private static void CreateSlimeSprite(string path)
        {
            if (File.Exists(path)) return;
            int size = 64;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Color[] colors = new Color[size * size];
            for (int i = 0; i < colors.Length; i++) colors[i] = Color.clear;

            Color slimeGreen = new Color(0.18f, 0.8f, 0.44f);
            Color slimeDark = new Color(0.12f, 0.6f, 0.32f);
            Color slimeLight = new Color(0.6f, 0.95f, 0.72f);
            Color eyeDark = new Color(0.08f, 0.2f, 0.12f);
            Color white = Color.white;

            Vector2 center = new Vector2(32f, 26f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = (x - center.x) / 19f;
                    float dy = (y - center.y) / (y < center.y ? 14f : 20f);
                    float distSq = dx * dx + dy * dy;

                    if (distSq <= 1.0f)
                    {
                        if (y >= 27 && y <= 33 && (x == 26 || x == 27 || x == 37 || x == 38))
                        {
                            if (y == 32 && (x == 27 || x == 38)) colors[y * size + x] = white;
                            else colors[y * size + x] = eyeDark;
                        }
                        else if (distSq < 0.6f && x < 28 && y > 30) colors[y * size + x] = slimeLight;
                        else if (y < 18) colors[y * size + x] = slimeDark;
                        else colors[y * size + x] = slimeGreen;
                    }
                }
            }

            tex.SetPixels(colors);
            tex.Apply();
            SaveSpritePNG(tex, path, 32);
        }

        private static void CreateWolfSprite(string path)
        {
            if (File.Exists(path)) return;
            int size = 64;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Color[] colors = new Color[size * size];
            for (int i = 0; i < colors.Length; i++) colors[i] = Color.clear;

            Color wolfGray = new Color(0.35f, 0.4f, 0.45f);
            Color wolfDark = new Color(0.2f, 0.24f, 0.28f);
            Color redEye = new Color(0.95f, 0.2f, 0.15f);
            Color white = Color.white;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    if (y >= 6 && y <= 18 && ((x >= 18 && x <= 22) || (x >= 26 && x <= 29) || (x >= 38 && x <= 42) || (x >= 46 && x <= 50)))
                        colors[y * size + x] = wolfDark;
                    else if (y >= 22 && y <= 36 && x >= 8 && x <= 18 && (y - x <= 20))
                        colors[y * size + x] = wolfDark;
                    else if (y >= 16 && y <= 34 && x >= 16 && x <= 46)
                        colors[y * size + x] = wolfGray;
                    else if (y >= 24 && y <= 40 && x >= 40 && x <= 56)
                    {
                        if (y >= 32 && y <= 35 && (x == 48 || x == 49)) colors[y * size + x] = redEye;
                        else if (y == 25 && x >= 52 && x <= 54) colors[y * size + x] = white;
                        else colors[y * size + x] = wolfGray;
                    }
                    else if (y >= 40 && y <= 50 && (x >= 42 && x <= 46))
                        colors[y * size + x] = wolfDark;
                }
            }

            tex.SetPixels(colors);
            tex.Apply();
            SaveSpritePNG(tex, path, 32);
        }

        private static void CreateBossTrollSprite(string path)
        {
            if (File.Exists(path)) return;
            int size = 128;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Color[] colors = new Color[size * size];
            for (int i = 0; i < colors.Length; i++) colors[i] = Color.clear;

            Color trollMoss = new Color(0.3f, 0.42f, 0.28f);
            Color trollDark = new Color(0.2f, 0.28f, 0.18f);
            Color rock = new Color(0.48f, 0.5f, 0.52f);
            Color clubWood = new Color(0.42f, 0.24f, 0.12f);
            Color clubIron = new Color(0.72f, 0.74f, 0.76f);
            Color rageEye = new Color(1f, 0.55f, 0.05f);
            Color bone = new Color(0.95f, 0.95f, 0.9f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    if (x >= 92 && x <= 116 && y >= 20 && y <= 118)
                    {
                        colors[y * size + x] = ((x == 92 || x == 116) && y % 14 >= 10) ? clubIron : clubWood;
                    }
                    else if (y >= 10 && y <= 36 && ((x >= 34 && x <= 52) || (x >= 64 && x <= 82)))
                        colors[y * size + x] = trollDark;
                    else if (y >= 36 && y <= 84 && x >= 30 && x <= 86)
                        colors[y * size + x] = trollMoss;
                    else if (y >= 74 && y <= 98 && ((x >= 20 && x <= 38) || (x >= 78 && x <= 96)))
                        colors[y * size + x] = rock;
                    else if (y >= 80 && y <= 114 && x >= 42 && x <= 74)
                    {
                        if (y >= 104 && (x <= 48 || x >= 68)) colors[y * size + x] = rock;
                        else if (y >= 94 && y <= 98 && (x == 52 || x == 53 || x == 63 || x == 64)) colors[y * size + x] = rageEye;
                        else if (y >= 84 && y <= 90 && (x == 49 || x == 67)) colors[y * size + x] = bone;
                        else colors[y * size + x] = trollDark;
                    }
                }
            }

            tex.SetPixels(colors);
            tex.Apply();
            SaveSpritePNG(tex, path, 48);
        }

        private static void CreateSlashSprite(string path)
        {
            if (File.Exists(path)) return;
            int size = 64;
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Color[] colors = new Color[size * size];
            for (int i = 0; i < colors.Length; i++) colors[i] = Color.clear;

            Color white = Color.white;
            Color cyan = new Color(0.2f, 0.9f, 1f, 0.9f);
            Color glow = new Color(0f, 0.6f, 1f, 0.5f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float targetY = 32f + 24f * Mathf.Sin((x - 10f) / 44f * Mathf.PI);
                    float diff = Mathf.Abs(y - targetY);
                    if (x >= 10 && x <= 54)
                    {
                        if (diff <= 1.5f) colors[y * size + x] = white;
                        else if (diff <= 4f) colors[y * size + x] = cyan;
                        else if (diff <= 7f) colors[y * size + x] = glow;
                    }
                }
            }

            tex.SetPixels(colors);
            tex.Apply();
            SaveSpritePNG(tex, path, 32);
        }

        private static void CreatePlayerPrefab()
        {
            GenerateGameSprites();

            var playerSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/spr_player.png");
            string path = "Assets/Prefabs/Entities/Player.prefab";

            GameObject player = new GameObject("Player");
            player.tag = "Player";

            var rb = player.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.freezeRotation = true;

            var col = player.AddComponent<CircleCollider2D>();
            col.radius = 0.5f;

            var sr = player.AddComponent<SpriteRenderer>();
            sr.sprite = playerSprite;
            sr.sortingOrder = 10;

            player.AddComponent<PlayerStats>();
            player.AddComponent<PlayerController>();
            player.AddComponent<PlayerInventory>();

            PrefabUtility.SaveAsPrefabAsset(player, path);
            GameObject.DestroyImmediate(player);
            Debug.Log("✅ Player.prefab 생성 완료");
        }

        private static void CreateEnemyPrefabs()
        {
            GenerateGameSprites();

            var slimeSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/spr_slime.png");
            var wolfSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/spr_wolf.png");
            var bossSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/spr_boss_troll.png");

            var slimeData = AssetDatabase.LoadAssetAtPath<EnemyData>("Assets/Data/Enemies/Enemy_Slime.asset");
            var wolfData = AssetDatabase.LoadAssetAtPath<EnemyData>("Assets/Data/Enemies/Enemy_Wolf.asset");
            var bossData = AssetDatabase.LoadAssetAtPath<EnemyData>("Assets/Data/Enemies/Boss_Troll.asset");

            // 1. Slime
            string slimePath = "Assets/Prefabs/Entities/Enemy_Slime.prefab";
            GameObject slime = new GameObject("Enemy_Slime");
            slime.tag = "Enemy";
            var rbS = slime.AddComponent<Rigidbody2D>();
            rbS.gravityScale = 0f;
            rbS.freezeRotation = true;
            var colS = slime.AddComponent<CircleCollider2D>();
            colS.radius = 0.45f;
            var srS = slime.AddComponent<SpriteRenderer>();
            srS.sprite = slimeSprite;
            srS.sortingOrder = 5;
            var ctrlS = slime.AddComponent<EnemyController>();
            ctrlS.SetData(slimeData);
            slime.AddComponent<MiniKingdom.Items.LootManager>();
            PrefabUtility.SaveAsPrefabAsset(slime, slimePath);
            GameObject.DestroyImmediate(slime);

            // 2. Wolf
            string wolfPath = "Assets/Prefabs/Entities/Enemy_Wolf.prefab";
            GameObject wolf = new GameObject("Enemy_Wolf");
            wolf.tag = "Enemy";
            var rbW = wolf.AddComponent<Rigidbody2D>();
            rbW.gravityScale = 0f;
            rbW.freezeRotation = true;
            var colW = wolf.AddComponent<CircleCollider2D>();
            colW.radius = 0.5f;
            var srW = wolf.AddComponent<SpriteRenderer>();
            srW.sprite = wolfSprite;
            srW.sortingOrder = 5;
            var ctrlW = wolf.AddComponent<WolfEnemyController>();
            ctrlW.SetData(wolfData);
            wolf.AddComponent<MiniKingdom.Items.LootManager>();
            PrefabUtility.SaveAsPrefabAsset(wolf, wolfPath);
            GameObject.DestroyImmediate(wolf);

            // 3. Boss Troll
            string trollPath = "Assets/Prefabs/Entities/Boss_Troll.prefab";
            GameObject troll = new GameObject("Boss_Troll");
            troll.tag = "Enemy";
            var rbT = troll.AddComponent<Rigidbody2D>();
            rbT.gravityScale = 0f;
            rbT.freezeRotation = true;
            var colT = troll.AddComponent<CircleCollider2D>();
            colT.radius = 1.0f;
            var srT = troll.AddComponent<SpriteRenderer>();
            srT.sprite = bossSprite;
            srT.sortingOrder = 6;
            var ctrlT = troll.AddComponent<BossController>();
            ctrlT.SetData(bossData);
            troll.AddComponent<MiniKingdom.Items.LootManager>();
            PrefabUtility.SaveAsPrefabAsset(troll, trollPath);
            GameObject.DestroyImmediate(troll);

            Debug.Log("✅ Enemy_Slime, Enemy_Wolf, Boss_Troll 프리팹 생성 완료");
        }

        private static void CreateFloatingTextPrefab()
        {
            string path = "Assets/Prefabs/UI/FloatingText.prefab";

            GameObject ftObj = new GameObject("FloatingText");
            var rect = ftObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(160, 40);

            var textMesh = ftObj.AddComponent<TextMeshProUGUI>();
            textMesh.fontSize = 24;
            textMesh.fontStyle = FontStyles.Bold;
            textMesh.alignment = TextAlignmentOptions.Center;
            textMesh.raycastTarget = false;

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
            titleText.fontStyle = FontStyles.Bold;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.raycastTarget = false;
            var titleRect = titleGO.GetComponent<RectTransform>();
            titleRect.anchoredPosition = new Vector2(0, 360);
            titleRect.sizeDelta = new Vector2(600, 50);

            // Level & Resource Summary Text
            GameObject resSummaryGO = new GameObject("ResourceSummary");
            resSummaryGO.transform.SetParent(canvasGO.transform, false);
            var resSummaryText = resSummaryGO.AddComponent<TextMeshProUGUI>();
            resSummaryText.text = "왕국 레벨: Lv. 1  |  골드: 1,000  |  목재: 200  |  석재: 100";
            resSummaryText.fontSize = 18;
            resSummaryText.color = new Color(1f, 0.95f, 0.65f);
            resSummaryText.alignment = TextAlignmentOptions.Center;
            resSummaryText.raycastTarget = false;
            var resRect = resSummaryGO.GetComponent<RectTransform>();
            resRect.anchoredPosition = new Vector2(0, 310);
            resRect.sizeDelta = new Vector2(600, 35);

            // Wire KingdomScreen fields
            var fieldLvl = typeof(KingdomScreen).GetField("kingdomLevelText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (fieldLvl != null) fieldLvl.SetValue(kingdomScreen, resSummaryText);

            var fieldRes = typeof(KingdomScreen).GetField("resourceSummaryText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (fieldRes != null) fieldRes.SetValue(kingdomScreen, resSummaryText);

            // Create 4 Core Building Cards
            var cardsList = new List<BuildingCardUI>();
            cardsList.Add(CreateBuildingCard(canvasGO.transform, "building_blacksmith", "대장간 (Blacksmith)", "영구 공격력(ATK) +5 증가", new Vector2(0, 220)));
            cardsList.Add(CreateBuildingCard(canvasGO.transform, "building_barracks", "훈련소 (Barracks)", "영구 최대체력 +25, 방어력 +2 증가", new Vector2(0, 135)));
            cardsList.Add(CreateBuildingCard(canvasGO.transform, "building_magic_tower", "마법탑 (Magic Tower)", "영구 공격속도 +10% 증가", new Vector2(0, 50)));
            cardsList.Add(CreateBuildingCard(canvasGO.transform, "building_farm", "농장 (Farm)", "영구 최대체력 +15 증가 & 세금 수입 상승", new Vector2(0, -35)));

            var fieldCards = typeof(KingdomScreen).GetField("buildingCards", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (fieldCards != null) fieldCards.SetValue(kingdomScreen, cardsList);

            // Treasury / Tax Button
            GameObject treasuryBtnGO = new GameObject("TreasuryButton");
            treasuryBtnGO.transform.SetParent(canvasGO.transform, false);
            var treasuryImg = treasuryBtnGO.AddComponent<Image>();
            treasuryImg.color = new Color(0.85f, 0.62f, 0.18f);
            var treasuryBtn = treasuryBtnGO.AddComponent<Button>();
            var treasuryRect = treasuryBtnGO.GetComponent<RectTransform>();
            treasuryRect.anchoredPosition = new Vector2(0, -135);
            treasuryRect.sizeDelta = new Vector2(360, 50);

            GameObject tTextGO = new GameObject("Text");
            tTextGO.transform.SetParent(treasuryBtnGO.transform, false);
            var tText = tTextGO.AddComponent<TextMeshProUGUI>();
            tText.text = "일일 세금 징수 (Collect Taxes)";
            tText.fontSize = 20;
            tText.alignment = TextAlignmentOptions.Center;
            tText.raycastTarget = false;

            UnityEditor.Events.UnityEventTools.AddPersistentListener(treasuryBtn.onClick, kingdomScreen.OnTreasuryClicked);
            var fieldTreasury = typeof(KingdomScreen).GetField("treasuryButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (fieldTreasury != null) fieldTreasury.SetValue(kingdomScreen, treasuryBtn);

            // Explore / Dungeon Go Button
            GameObject goDungeonBtnGO = new GameObject("ExploreButton");
            goDungeonBtnGO.transform.SetParent(canvasGO.transform, false);
            var goDungeonImg = goDungeonBtnGO.AddComponent<Image>();
            goDungeonImg.color = new Color(0.85f, 0.28f, 0.22f);
            var goDungeonBtn = goDungeonBtnGO.AddComponent<Button>();
            var goDungeonRect = goDungeonBtnGO.GetComponent<RectTransform>();
            goDungeonRect.anchoredPosition = new Vector2(0, -215);
            goDungeonRect.sizeDelta = new Vector2(360, 65);

            GameObject dTextGO = new GameObject("Text");
            dTextGO.transform.SetParent(goDungeonBtnGO.transform, false);
            var dText = dTextGO.AddComponent<TextMeshProUGUI>();
            dText.text = "던전 출정하기 (Dungeon)";
            dText.fontSize = 22;
            dText.fontStyle = FontStyles.Bold;
            dText.alignment = TextAlignmentOptions.Center;
            dText.raycastTarget = false;

            // Wire up persistent onClick listener with KingdomSceneController
            var kingdomCtrl = goDungeonBtnGO.AddComponent<KingdomSceneController>();
            UnityEditor.Events.UnityEventTools.AddPersistentListener(goDungeonBtn.onClick, kingdomCtrl.OnClickDepartDungeon);

            // Wire up explore tab button in kingdomScreen
            var fieldExplore = typeof(KingdomScreen).GetField("exploreTabBtn", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (fieldExplore != null) fieldExplore.SetValue(kingdomScreen, goDungeonBtn);

            EditorUtility.SetDirty(kingdomScreen);

            InstantiateGameCoreInScene();

            EditorSceneManager.SaveScene(scene, scenePath);
            Debug.Log("✅ Kingdom 씬 생성 완료");
        }

        private static BuildingCardUI CreateBuildingCard(Transform parent, string buildingId, string initialName, string initialEffect, Vector2 pos)
        {
            GameObject cardGO = new GameObject($"Card_{buildingId}");
            cardGO.transform.SetParent(parent, false);

            var cardImg = cardGO.AddComponent<Image>();
            cardImg.color = new Color(0.12f, 0.15f, 0.22f, 0.95f);

            var rect = cardGO.GetComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = new Vector2(520, 74);

            var cardUI = cardGO.AddComponent<BuildingCardUI>();

            // Title Text
            GameObject titleGO = new GameObject("TitleText");
            titleGO.transform.SetParent(cardGO.transform, false);
            var titleText = titleGO.AddComponent<TextMeshProUGUI>();
            titleText.text = $"{initialName}  Lv. 1";
            titleText.fontSize = 18;
            titleText.fontStyle = FontStyles.Bold;
            titleText.alignment = TextAlignmentOptions.Left;
            titleText.raycastTarget = false;
            var tRect = titleGO.GetComponent<RectTransform>();
            tRect.anchoredPosition = new Vector2(-90, 16);
            tRect.sizeDelta = new Vector2(300, 28);

            // Effect Text
            GameObject effectGO = new GameObject("EffectText");
            effectGO.transform.SetParent(cardGO.transform, false);
            var effectText = effectGO.AddComponent<TextMeshProUGUI>();
            effectText.text = initialEffect;
            effectText.fontSize = 13;
            effectText.color = new Color(0.75f, 0.9f, 0.85f);
            effectText.alignment = TextAlignmentOptions.Left;
            effectText.raycastTarget = false;
            var eRect = effectGO.GetComponent<RectTransform>();
            eRect.anchoredPosition = new Vector2(-40, -16);
            eRect.sizeDelta = new Vector2(400, 24);

            // Cost Text
            GameObject costGO = new GameObject("CostText");
            costGO.transform.SetParent(cardGO.transform, false);
            var costText = costGO.AddComponent<TextMeshProUGUI>();
            costText.text = "비용: 골드 100";
            costText.fontSize = 13;
            costText.color = new Color(1f, 0.85f, 0.4f);
            costText.alignment = TextAlignmentOptions.Right;
            costText.raycastTarget = false;
            var cRect = costGO.GetComponent<RectTransform>();
            cRect.anchoredPosition = new Vector2(45, 16);
            cRect.sizeDelta = new Vector2(170, 28);

            // Upgrade Button
            GameObject btnGO = new GameObject("UpgradeBtn");
            btnGO.transform.SetParent(cardGO.transform, false);
            var btnImg = btnGO.AddComponent<Image>();
            btnImg.color = new Color(0.2f, 0.55f, 0.75f);
            var btn = btnGO.AddComponent<Button>();
            var bRect = btnGO.GetComponent<RectTransform>();
            bRect.anchoredPosition = new Vector2(205, 0);
            bRect.sizeDelta = new Vector2(85, 48);

            GameObject btnTxtGO = new GameObject("Text");
            btnTxtGO.transform.SetParent(btnGO.transform, false);
            var btnTxt = btnTxtGO.AddComponent<TextMeshProUGUI>();
            btnTxt.text = "강화";
            btnTxt.fontSize = 17;
            btnTxt.alignment = TextAlignmentOptions.Center;
            btnTxt.raycastTarget = false;

            UnityEditor.Events.UnityEventTools.AddPersistentListener(btn.onClick, cardUI.OnClickUpgrade);

            cardUI.SetReferences(buildingId, titleText, effectText, costText, btn, btnTxt);
            EditorUtility.SetDirty(cardUI);

            return cardUI;
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
            camGO.AddComponent<CameraShake>();
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

            // Wire enemy prefabs into spawner
            var enemySlimePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Entities/Enemy_Slime.prefab");
            var enemyWolfPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Entities/Enemy_Wolf.prefab");
            var bossTrollPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Entities/Boss_Troll.prefab");

            var fieldDefEnemy = typeof(EnemySpawner).GetField("defaultEnemyPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (fieldDefEnemy != null && enemySlimePrefab != null) fieldDefEnemy.SetValue(spawner, enemySlimePrefab);

            var fieldWolfEnemy = typeof(EnemySpawner).GetField("wolfEnemyPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (fieldWolfEnemy != null && enemyWolfPrefab != null) fieldWolfEnemy.SetValue(spawner, enemyWolfPrefab);

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
            var hudScreen = hudGO.AddComponent<DungeonHUDScreen>();

            // 1. Player HP Bar (Top Left)
            GameObject playerHpGO = new GameObject("PlayerHpBar");
            playerHpGO.transform.SetParent(hudGO.transform, false);
            var hpRect = playerHpGO.AddComponent<RectTransform>();
            hpRect.anchoredPosition = new Vector2(-220, 350);
            hpRect.sizeDelta = new Vector2(240, 26);
            var hpSlider = playerHpGO.AddComponent<Slider>();
            hpSlider.minValue = 0f;
            hpSlider.maxValue = 1f;
            hpSlider.value = 1f;

            var hpBg = playerHpGO.AddComponent<Image>();
            hpBg.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

            GameObject fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(playerHpGO.transform, false);
            var faRect = fillArea.AddComponent<RectTransform>();
            faRect.anchorMin = Vector2.zero;
            faRect.anchorMax = Vector2.one;
            faRect.sizeDelta = Vector2.zero;

            GameObject fillObj = new GameObject("Fill");
            fillObj.transform.SetParent(fillArea.transform, false);
            var fillRect = fillObj.AddComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.sizeDelta = Vector2.zero;
            var fillImg = fillObj.AddComponent<Image>();
            fillImg.color = new Color(0.2f, 0.8f, 0.35f);
            hpSlider.fillRect = fillRect;

            GameObject hpTextGO = new GameObject("HpText");
            hpTextGO.transform.SetParent(playerHpGO.transform, false);
            var hpText = hpTextGO.AddComponent<TextMeshProUGUI>();
            hpText.text = "100 / 100";
            hpText.fontSize = 16;
            hpText.alignment = TextAlignmentOptions.Center;
            hpText.raycastTarget = false;

            // 2. Boss HP Panel (Top Center)
            GameObject bossPanelGO = new GameObject("BossHpPanel");
            bossPanelGO.transform.SetParent(hudGO.transform, false);
            var bpRect = bossPanelGO.AddComponent<RectTransform>();
            bpRect.anchoredPosition = new Vector2(0, 345);
            bpRect.sizeDelta = new Vector2(500, 48);

            GameObject bossNameGO = new GameObject("BossNameText");
            bossNameGO.transform.SetParent(bossPanelGO.transform, false);
            var bnText = bossNameGO.AddComponent<TextMeshProUGUI>();
            bnText.text = "숲 트롤 (보스)";
            bnText.fontSize = 18;
            bnText.fontStyle = FontStyles.Bold;
            bnText.alignment = TextAlignmentOptions.Center;
            bnText.raycastTarget = false;
            var bnRect = bossNameGO.GetComponent<RectTransform>();
            bnRect.anchoredPosition = new Vector2(0, 14);
            bnRect.sizeDelta = new Vector2(400, 24);

            GameObject bossSliderGO = new GameObject("BossHpSlider");
            bossSliderGO.transform.SetParent(bossPanelGO.transform, false);
            var bsRect = bossSliderGO.AddComponent<RectTransform>();
            bsRect.anchoredPosition = new Vector2(0, -12);
            bsRect.sizeDelta = new Vector2(460, 20);
            var bossSlider = bossSliderGO.AddComponent<Slider>();
            var bsBg = bossSliderGO.AddComponent<Image>();
            bsBg.color = new Color(0.2f, 0.1f, 0.1f, 0.85f);

            GameObject bFillArea = new GameObject("Fill Area");
            bFillArea.transform.SetParent(bossSliderGO.transform, false);
            var bfaRect = bFillArea.AddComponent<RectTransform>();
            bfaRect.anchorMin = Vector2.zero;
            bfaRect.anchorMax = Vector2.one;
            bfaRect.sizeDelta = Vector2.zero;

            GameObject bFillObj = new GameObject("Fill");
            bFillObj.transform.SetParent(bFillArea.transform, false);
            var bFillRect = bFillObj.AddComponent<RectTransform>();
            bFillRect.anchorMin = Vector2.zero;
            bFillRect.anchorMax = Vector2.one;
            bFillRect.sizeDelta = Vector2.zero;
            var bFillImg = bFillObj.AddComponent<Image>();
            bFillImg.color = new Color(0.9f, 0.2f, 0.15f);
            bossSlider.fillRect = bFillRect;
            bossSlider.value = 1f;

            bossPanelGO.SetActive(false);

            // 3. Perfect Dodge Flash
            GameObject dodgeFlashGO = new GameObject("PerfectDodgeFlash");
            dodgeFlashGO.transform.SetParent(hudGO.transform, false);
            var flashImg = dodgeFlashGO.AddComponent<Image>();
            flashImg.color = Color.clear;
            flashImg.raycastTarget = false;
            var flashRect = dodgeFlashGO.GetComponent<RectTransform>();
            flashRect.anchorMin = Vector2.zero;
            flashRect.anchorMax = Vector2.one;
            flashRect.sizeDelta = Vector2.zero;

            // 4. Combo Panel
            GameObject comboPanelGO = new GameObject("ComboPanel");
            comboPanelGO.transform.SetParent(hudGO.transform, false);
            var cpRect = comboPanelGO.AddComponent<RectTransform>();
            cpRect.anchoredPosition = new Vector2(220, 120);
            cpRect.sizeDelta = new Vector2(160, 40);

            GameObject comboTextGO = new GameObject("ComboText");
            comboTextGO.transform.SetParent(comboPanelGO.transform, false);
            var comboText = comboTextGO.AddComponent<TextMeshProUGUI>();
            comboText.text = "3 Hits!";
            comboText.fontSize = 24;
            comboText.fontStyle = FontStyles.Bold;
            comboText.color = new Color(1f, 0.85f, 0.2f);
            comboText.alignment = TextAlignmentOptions.Center;
            comboText.raycastTarget = false;

            comboPanelGO.SetActive(false);

            // Wire HUD Screen fields via reflection
            typeof(DungeonHUDScreen).GetField("hpBar", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(hudScreen, hpSlider);
            typeof(DungeonHUDScreen).GetField("hpText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(hudScreen, hpText);
            typeof(DungeonHUDScreen).GetField("bossHpPanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(hudScreen, bossPanelGO);
            typeof(DungeonHUDScreen).GetField("bossHpBar", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(hudScreen, bossSlider);
            typeof(DungeonHUDScreen).GetField("bossNameText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(hudScreen, bnText);
            typeof(DungeonHUDScreen).GetField("perfectDodgeFlash", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(hudScreen, flashImg);
            typeof(DungeonHUDScreen).GetField("comboPanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(hudScreen, comboPanelGO);
            typeof(DungeonHUDScreen).GetField("comboText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(hudScreen, comboText);

            EditorUtility.SetDirty(hudScreen);

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
