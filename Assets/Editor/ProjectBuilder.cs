using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;

namespace MiniKingdom.Editor
{
    public class ProjectBuilder
    {
        [MenuItem("Mini Kingdom/🚀 1. 기본 씬 및 프리팹 자동 생성", false, 1)]
        public static void SetupProject()
        {
            Debug.Log("미니왕국 프로젝트 자동 구성을 시작합니다...");

            // 1. 필요한 폴더 확인 및 생성
            string[] folders = { "Assets/Scenes", "Assets/Prefabs", "Assets/Prefabs/Core", "Assets/Prefabs/UI" };
            foreach (string folder in folders)
            {
                if (!AssetDatabase.IsValidFolder(folder))
                {
                    string parent = Path.GetDirectoryName(folder).Replace("\\", "/");
                    string folderName = Path.GetFileName(folder);
                    AssetDatabase.CreateFolder(parent, folderName);
                }
            }

            // 2. 씬(Scene) 생성
            string[] sceneNames = { "MainMenu", "Kingdom", "Dungeon" };
            List<EditorBuildSettingsScene> buildScenes = new List<EditorBuildSettingsScene>();

            foreach (string sName in sceneNames)
            {
                string scenePath = $"Assets/Scenes/{sName}.unity";
                if (!File.Exists(scenePath))
                {
                    Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
                    newScene.name = sName;
                    EditorSceneManager.SaveScene(newScene, scenePath);
                    Debug.Log($"✅ 씬 생성 완료: {sName}");
                }
                buildScenes.Add(new EditorBuildSettingsScene(scenePath, true));
            }

            // Build Settings에 씬 추가
            EditorBuildSettings.scenes = buildScenes.ToArray();
            Debug.Log("✅ Build Settings에 씬 등록 완료");

            // 3. Core(게임 매니저 등) 프리팹 생성
            string corePrefabPath = "Assets/Prefabs/Core/GameCore.prefab";
            if (!File.Exists(corePrefabPath))
            {
                GameObject coreGO = new GameObject("GameCore");
                
                // 필수 컴포넌트들 부착 (스크립트가 에러 없이 컴파일된 상태여야 함)
                // coreGO.AddComponent<MiniKingdom.Core.GameManager>();
                // coreGO.AddComponent<MiniKingdom.Core.SaveManager>();
                
                GameObject.DontDestroyOnLoad(coreGO);

                PrefabUtility.SaveAsPrefabAsset(coreGO, corePrefabPath);
                GameObject.DestroyImmediate(coreGO);
                Debug.Log("✅ GameCore 프리팹 생성 완료");
            }

            // 4. Kingdom 씬을 기본으로 열기
            EditorSceneManager.OpenScene("Assets/Scenes/Kingdom.unity");
            
            AssetDatabase.Refresh();
            Debug.Log("🎉 미니왕국 기본 씬 및 프리팹 세팅이 완료되었습니다!");
        }
    }
}
