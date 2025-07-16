using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace InputHelpers
{
    public class GamePadSpriteRepository : MonoBehaviour
    {
        public static GamePadSpriteRepository Instance
        {
            get;
            private set;
        }

        [Header("Sprite Assets")]
        [SerializeField] private TMP_SpriteAsset xboxSpriteAsset;
        [SerializeField] private TMP_SpriteAsset playstationSpriteAsset;
        [SerializeField] private TMP_SpriteAsset nintendoSpriteAsset;
        [SerializeField] private TMP_SpriteAsset steamSpriteAsset;
        [SerializeField] private TMP_SpriteAsset genericSpriteAsset;

        [Header("Auto Detection")]
        [SerializeField] private bool autoDetectGamepad = true;
        [SerializeField] private float detectionInterval = 1f;

        // Events
        public event Action<TMP_SpriteAsset> OnSpriteAssetChanged;
        public event Action<GamepadType> OnGamepadTypeChanged;

        // Current state
        private TMP_SpriteAsset _currentSpriteAsset;
        private GamepadType _currentGamepadType = GamepadType.Generic;
        private float _lastDetectionTime;

        public enum GamepadType
        {
            Xbox,
            PlayStation,
            Nintendo,
            Steam,
            Generic
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
            else if (Instance != this)
            {
                Debug.LogError("Duplicate GamePadSpriteRepository");
                Destroy(gameObject);
            }
        }

        private void Initialize()
        {
            // Set initial sprite asset
            _currentSpriteAsset = GetSpriteAssetForGamepadType(_currentGamepadType);
            
            if (autoDetectGamepad)
            {
                DetectCurrentGamepad();
            }
        }

        private void Update()
        {
            if (autoDetectGamepad && Time.time - _lastDetectionTime > detectionInterval)
            {
                DetectCurrentGamepad();
                _lastDetectionTime = Time.time;
            }
        }

        private void DetectCurrentGamepad()
        {
            var gamepads = Gamepad.all;
            if (gamepads.Count > 0)
            {
                var gamepad = gamepads[0];
                var newGamepadType = DetermineGamepadType(gamepad);
                
                if (newGamepadType != _currentGamepadType)
                {
                    _currentGamepadType = newGamepadType;
                    var newSpriteAsset = GetSpriteAssetForGamepadType(_currentGamepadType);
                    
                    if (newSpriteAsset != _currentSpriteAsset)
                    {
                        _currentSpriteAsset = newSpriteAsset;
                        OnGamepadTypeChanged?.Invoke(_currentGamepadType);
                        OnSpriteAssetChanged?.Invoke(_currentSpriteAsset);
                    }
                }
            }
        }

        private GamepadType DetermineGamepadType(Gamepad gamepad)
        {
            string name = gamepad.name.ToLower();
            
            if (name.Contains("xbox") || name.Contains("xinput"))
                return GamepadType.Xbox;
            else if (name.Contains("playstation") || name.Contains("ps4") || name.Contains("ps5") || name.Contains("dualshock") || name.Contains("dualsense"))
                return GamepadType.PlayStation;
            else if (name.Contains("nintendo") || name.Contains("switch") || name.Contains("joycon") || name.Contains("pro controller"))
                return GamepadType.Nintendo;
            else if (name.Contains("steam") || name.Contains("valve"))
                return GamepadType.Steam;
            else
                return GamepadType.Generic;
        }

        private TMP_SpriteAsset GetSpriteAssetForGamepadType(GamepadType gamepadType)
        {
            return gamepadType switch
            {
                GamepadType.Xbox => xboxSpriteAsset,
                GamepadType.PlayStation => playstationSpriteAsset,
                GamepadType.Nintendo => nintendoSpriteAsset,
                GamepadType.Steam => steamSpriteAsset,
                GamepadType.Generic => genericSpriteAsset,
                _ => genericSpriteAsset
            };
        }

        // Public methods
        public TMP_SpriteAsset GetCurrentSpriteAsset()
        {
            return _currentSpriteAsset;
        }

        public GamepadType GetCurrentGamepadType()
        {
            return _currentGamepadType;
        }
    }
} 