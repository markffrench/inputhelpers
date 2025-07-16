using UnityEngine;
using TMPro;
using System;
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
        private string originalText;
        
        private void Awake()
        {
            textMeshPro = GetComponent<TMP_Text>();
        }
        
        private void Start()
        {
            // Store the original text
            originalText = textMeshPro.text;
            
            // Subscribe to sprite asset changes
            GamePadSpriteRepository.Instance.OnSpriteAssetChanged += OnSpriteAssetChanged;
            UpdateIcon(GamePadSpriteRepository.Instance.GetCurrentSpriteAsset());
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            if (GamePadSpriteRepository.Instance != null)
            {
                GamePadSpriteRepository.Instance.OnSpriteAssetChanged -= OnSpriteAssetChanged;
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
                if(originalText.Contains("{"+selectedActionName+"}"))
                    textMeshPro.text = originalText.Replace("{"+selectedActionName+"}", $"<sprite index={spriteIndex}>");
                else
                {
                    throw new InvalidOperationException(
                        $"Button text does not contain matching prompt: {originalText} vs {selectedActionName}");
                }
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
                    // Get the binding path to determine the control type
                    var binding = action.bindings[0]; // Use first binding as default
                    string path = binding.path;
                    
                    // Map common input paths to sprite names
                    if (path.Contains("/buttonSouth") || path.Contains("/buttonA"))
                        return "btn_a";
                    if (path.Contains("/buttonNorth") || path.Contains("/buttonY"))
                        return "btn_y";
                    if (path.Contains("/buttonWest") || path.Contains("/buttonX"))
                        return "btn_x";
                    if (path.Contains("/buttonEast") || path.Contains("/buttonB"))
                        return "btn_b";
                    if (path.Contains("/dpad/up"))
                        return "dpad_up";
                    if (path.Contains("/dpad/down"))
                        return "dpad_down";
                    if (path.Contains("/dpad/left"))
                        return "dpad_left";
                    if (path.Contains("/dpad/right"))
                        return "dpad_right";
                    if (path.Contains("/leftStick"))
                        return "lstick";
                    if (path.Contains("/rightStick"))
                        return "rstick";
                    if (path.Contains("/start"))
                        return "btn_start";
                    if (path.Contains("/select") || path.Contains("/back"))
                        return "btn_back";
                }
            }
            
            // Fallback mapping based on action name
            return actionName.ToLower() switch
            {
                "back" or "cancel" => "btn_b",
                "confirm" or "submit" or "interact" => "btn_a",
                "undo" or "secondary" => "btn_y",
                "toggleeditmode" or "tertiary" => "btn_x",
                "cursormove" or "move" => "lstick",
                "nudgeup" or "up" => "dpad_up",
                "nudgedown" or "down" => "dpad_down",
                "nudgeleft" or "left" => "dpad_left",
                "nudgeright" or "right" => "dpad_right",
                "hint" or "help" => "btn_start",
                "tap" or "click" => "btn_a",
                "tapandhold" or "hold" => "btn_a",
                _ => "btn_a"
            };
        }
    }
} 