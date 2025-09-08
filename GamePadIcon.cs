using UnityEngine;
using TMPro;
using System;
using System.Linq;
using Framework.Input;
using I2.Loc;
using UnityEngine.InputSystem;

namespace InputHelpers
{
    [RequireComponent(typeof(TMP_Text))]
    public class GamePadIcon : MonoBehaviour
    {
        [Header("Input Action Asset")]
        [SerializeField] private InputActionAsset inputActionAsset;
        
        [Header("Action Selection")]
        [SerializeField] private string selectedActionName = "Back";
        
        private TMP_Text textMeshPro;
        private LocalizationParamsManager localizationParamsManager;
        
        private void Awake()
        {
            textMeshPro = GetComponent<TMP_Text>();
            localizationParamsManager = GetComponent<LocalizationParamsManager>();

            if (localizationParamsManager == null)
                localizationParamsManager = gameObject.AddComponent<LocalizationParamsManager>();
        }
        
        private void Start()
        {
            // Subscribe to sprite asset changes
            GamePadSpriteRepository.Instance.OnSpriteAssetChanged += OnSpriteAssetChanged;
            
            // Subscribe to control scheme changes
            ControlSchemeSwapper.OnChanged += OnControlSchemeChanged;
           
            OnControlSchemeChanged(ControlSchemeSwapper.currentControlScheme);
        }
        
        private void OnDestroy()
        {
            if (GamePadSpriteRepository.Instance != null)
            {
                GamePadSpriteRepository.Instance.OnSpriteAssetChanged -= OnSpriteAssetChanged;
            }
            
            ControlSchemeSwapper.OnChanged -= OnControlSchemeChanged;
        }
        
        private void OnControlSchemeChanged(ControlScheme controlScheme)
        {
            if (controlScheme is ControlScheme.Gamepad or ControlScheme.SteamDeck)
            {
                UpdateIcon(GamePadSpriteRepository.Instance.GetCurrentSpriteAsset());
            }
            else
            {
                // Hide gamepad icon by restoring original text
                if (textMeshPro != null)
                {
                    localizationParamsManager.SetParameterValue(selectedActionName, String.Empty);
                }
            }
        }
        
        private void OnSpriteAssetChanged(TMP_SpriteAsset newSpriteAsset)
        {
            UpdateIcon(newSpriteAsset);
        }
        
        private void UpdateIcon(TMP_SpriteAsset gamepadSpriteAsset)
        {
            if (textMeshPro == null)
                throw new InvalidOperationException("TextMeshPro component is null");
                
            if (inputActionAsset == null)
                throw new InvalidOperationException("Input Action Asset is null");

            if (gamepadSpriteAsset == null)
                throw new InvalidOperationException("Gamepad Sprite Asset is null");

            textMeshPro.spriteAsset = gamepadSpriteAsset;
            
            string spriteName = GetSpriteNameForAction(selectedActionName)+"_0";
            //doing things this way because TMP is weird, it uses one hash code when generating the lookup table,
            //and another when looking up a sprite name
            //fixed in 6000.2: https://issuetracker.unity3d.com/issues/sprite-index-cannot-be-found-when-using-the-getspriteindexfromname-method
            int hashCode = TMP_TextUtilities.GetHashCode(spriteName);
            int spriteIndex = gamepadSpriteAsset.GetSpriteIndexFromHashcode(hashCode);
            
            if (spriteIndex != -1)
            {
                string spriteMarkup = $"<sprite index={spriteIndex}>";
                localizationParamsManager.SetParameterValue(selectedActionName, spriteMarkup);
            }
            else
            {
                throw new InvalidOperationException($"Sprite '{spriteName}' not found in sprite asset");
            }
        }
        
        private string GetSpriteNameForAction(string actionName)
        {
            // Try to find the action in the InputActionAsset
            if (inputActionAsset != null)
            {
                InputAction action = inputActionAsset.FindAction(actionName);
                if (action != null)
                {
                    foreach (InputBinding binding in action.bindings)
                    {
                        string path = binding.path;
                    
                        string spriteName = GetSpriteNameForActionPath(path);

                        if (!string.IsNullOrEmpty(spriteName))
                            return spriteName;
                    }
                }
            }
            
            Debug.LogError("No sprite found for action: " + actionName);
            return "btn_a";
        }

        private string GetSpriteNameForActionPath(string path)
        {
            if (path.EndsWith("/buttonSouth") || path.EndsWith("/buttonA"))
                return "btn_a";
            if (path.EndsWith("/buttonNorth") || path.EndsWith("/buttonY"))
                return "btn_y";
            if (path.EndsWith("/buttonWest") || path.EndsWith("/buttonX"))
                return "btn_x";
            if (path.EndsWith("/buttonEast") || path.EndsWith("/buttonB"))
                return "btn_b";
            if (path.EndsWith("/dpad/up"))
                return "dpad_up";
            if (path.EndsWith("/dpad/down"))
                return "dpad_down";
            if (path.EndsWith("/dpad/left"))
                return "dpad_left";
            if (path.EndsWith("/dpad/right"))
                return "dpad_right";
            if (path.EndsWith("/leftStick"))
                return "lstick";
            if (path.EndsWith("/rightStick"))
                return "rstick";
            if (path.EndsWith("/start"))
                return "start";
            if (path.EndsWith("/select") || path.EndsWith("/back"))
                return "select";

            return string.Empty;
        }
    }
} 