using UnityEngine;
using TMPro;
using System;

namespace InputHelpers
{
    [RequireComponent(typeof(TextMeshPro))]
    public class GamePadIcon : MonoBehaviour
    {
        [Header("Action Selection")]
        [SerializeField] private GlobalControlsAction selectedAction = GlobalControlsAction.Back;
        
        [Header("Sprite Asset")]
        [SerializeField] private TMP_SpriteAsset gamepadSpriteAsset;
        
        private TextMeshPro textMeshPro;
        
        private void Awake()
        {
            textMeshPro = GetComponent<TextMeshPro>();
        }
        
        private void Start()
        {
            UpdateIcon();
        }
        
        public void UpdateIcon()
        {
            if (textMeshPro == null)
                throw new InvalidOperationException("TextMeshPro component is null");
                
            if (gamepadSpriteAsset == null)
                throw new InvalidOperationException("Gamepad Sprite Asset is null");
                
            string spriteName = GetSpriteNameForAction(selectedAction);
            int spriteIndex = gamepadSpriteAsset.GetSpriteIndexFromName(spriteName);
            
            if (spriteIndex != -1)
            {
                textMeshPro.text = $"<sprite index={spriteIndex}>";
            }
            else
            {
                throw new InvalidOperationException($"Sprite '{spriteName}' not found in sprite asset");
            }
        }
        
        private string GetSpriteNameForAction(GlobalControlsAction action)
        {
            return action switch
            {
                GlobalControlsAction.Back => "btn_b",
                GlobalControlsAction.Confirm => "btn_a",
                GlobalControlsAction.Undo => "btn_y",
                GlobalControlsAction.ToggleEditMode => "btn_x",
                GlobalControlsAction.CursorMove => "lstick",
                GlobalControlsAction.NudgeUp => "dpad_up",
                GlobalControlsAction.NudgeDown => "dpad_down",
                GlobalControlsAction.NudgeLeft => "dpad_left",
                GlobalControlsAction.NudgeRight => "dpad_right",
                GlobalControlsAction.Hint => "btn_start",
                GlobalControlsAction.Tap => "btn_a",
                GlobalControlsAction.TapAndHold => "btn_a",
                _ => "btn_a"
            };
        }
        
        public void SetAction(GlobalControlsAction action)
        {
            selectedAction = action;
            UpdateIcon();
        }
        
        public enum GlobalControlsAction
        {
            Back,
            Confirm,
            Undo,
            ToggleEditMode,
            CursorMove,
            NudgeUp,
            NudgeDown,
            NudgeLeft,
            NudgeRight,
            Hint,
            Tap,
            TapAndHold
        }
    }
} 