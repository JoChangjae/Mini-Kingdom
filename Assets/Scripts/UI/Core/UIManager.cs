using System;
using System.Collections.Generic;
using UnityEngine;
using MiniKingdom.Core;
using MiniKingdom.Utils;

namespace MiniKingdom.UI
{
    public enum ScreenType
    {
        Kingdom,
        DungeonSelect,
        InDungeon_HUD,
        LevelUp,
        RunResult,
        Shop,
        Building,
        Inventory,
        DiscoveryBook,
        RoyalDecree,
        Settings,
        Pause,
        KingdomDefense
    }

    /// <summary>
    /// Singleton managing all UI screens.
    /// Manages screen stack and transitions.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [SerializeField] private List<ScreenBase> screenPrefabs;
        [SerializeField] private Transform screenParent;
        
        private Dictionary<ScreenType, ScreenBase> _instantiatedScreens = new Dictionary<ScreenType, ScreenBase>();
        private Stack<ScreenType> _screenStack = new Stack<ScreenType>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeScreens();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeScreens()
        {
            foreach (var prefab in screenPrefabs)
            {
                var instance = Instantiate(prefab, screenParent);
                instance.gameObject.SetActive(false);
                _instantiatedScreens.Add(instance.GetScreenType(), instance);
            }
        }

        /// <summary>
        /// Shows a screen and pushes it to the stack.
        /// </summary>
        public void Show(ScreenType screenType, bool pushToStack = true)
        {
            if (!_instantiatedScreens.TryGetValue(screenType, out var screen))
            {
                foreach (var inScene in FindObjectsOfType<ScreenBase>(true))
                {
                    if (inScene.GetScreenType() == screenType)
                    {
                        _instantiatedScreens[screenType] = inScene;
                        screen = inScene;
                        break;
                    }
                }
            }

            if (screen != null)
            {
                if (pushToStack)
                {
                    if (_screenStack.Count > 0 && _instantiatedScreens.TryGetValue(_screenStack.Peek(), out var prev))
                    {
                        prev.Hide();
                    }
                    _screenStack.Push(screenType);
                }
                
                screen.gameObject.SetActive(true);
                screen.Show();
            }
            else
            {
                Debug.LogWarning($"Screen {screenType} not found.");
            }
        }

        /// <summary>
        /// Hides a screen.
        /// </summary>
        public void Hide(ScreenType screenType)
        {
            if (_instantiatedScreens.TryGetValue(screenType, out var screen))
            {
                screen.Hide();
                screen.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Toggles a screen's visibility.
        /// </summary>
        public void Toggle(ScreenType screenType)
        {
            if (_instantiatedScreens.TryGetValue(screenType, out var screen))
            {
                if (screen.gameObject.activeSelf)
                {
                    Pop();
                }
                else
                {
                    Show(screenType);
                }
            }
        }

        /// <summary>
        /// Pops the top screen from the stack and shows the previous one.
        /// </summary>
        public void Pop()
        {
            if (_screenStack.Count > 0)
            {
                var topScreen = _screenStack.Pop();
                Hide(topScreen);

                if (_screenStack.Count > 0)
                {
                    var previousScreen = _screenStack.Peek();
                    Show(previousScreen, false);
                }
            }
        }
    }
}
