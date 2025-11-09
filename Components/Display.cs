using BepInEx.Logging;
using CanvasUtil;
using GlobalEnums;
using InControl;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logger = BepInEx.Logging.Logger;

namespace KeybindChaos.Components;

// Keybind display in the bottom right, text display in the top right
// Keybind display is controlled by this component
// Text display is controlled by whatever wants to display text (i.e. the timer)

public class Display : MonoBehaviour
{
    private GameObject _canvas;

    private Text _text;

    private GameObject? _keybindPanel;
    
    public void UpdateText(string value)
    {
        _text.text = value;
    }

    private void Awake()
    {
        _canvas = CanvasUtil.CanvasUtil.CreateCanvas(RenderMode.ScreenSpaceOverlay, new Vector2(1920, 1080));
        _canvas.GetComponent<CanvasGroup>().blocksRaycasts = false;
        _canvas.GetComponent<CanvasGroup>().interactable = false;
        DontDestroyOnLoad(_canvas);

        GameObject topRightText = CanvasUtil.CanvasUtil.CreateTextPanel(
            _canvas, "abcde", 40, TextAnchor.UpperRight,
            new(
                new(200, 100),
                new(0, 0),
                new(1, 1),
                new(1, 1),
                new(1, 1)
            ),
            Fonts.TrajanBold!);
        _text = topRightText.GetComponent<Text>();
        _text.horizontalOverflow = HorizontalWrapMode.Overflow;

        SetupKeybindPanel();

        KeybindPermuter.OnRandomize += SetupKeybindPanel;
        KeybindPermuter.OnRestore += DestroyKeybindPanel;
    }

    private void SetupKeybindPanel()
    {
        DestroyKeybindPanel();

        if (!KeybindChaosPlugin.Instance.ShowKeybindDisplay.Value)
        {
            return;
        }

        List<(string key, PlayerAction action)> binds = KeybindPermuter.RetrieveBindPairs();

        int rowHeight = 80;
        int spriteColWidth = 80;
        int bindColWidth = 80;
        int nRows = binds.Count;
        int height = rowHeight * nRows;
        int width = bindColWidth + spriteColWidth;

        _keybindPanel = CanvasUtil.CanvasUtil.CreateBasePanel(_canvas, new(new(width, height), new(0, 0), new(1, 0), new(1, 0), new(1, 0)));

        int idx = 0;
        foreach ((string key, PlayerAction action) in binds)
        {
            if (!KeybindChaosPlugin.Instance.BindConfig[key].Value) continue;

            GameObject spriteHolder = CanvasUtil.CanvasUtil.CreateBasePanel(_keybindPanel,
                new(
                    new(spriteColWidth, rowHeight),
                    new(0, -idx * rowHeight),
                    new(0, 1),
                    new(0, 1),
                    new(0, 1)
                    ));

            CanvasUtil.CanvasUtil.CreateImagePanel(spriteHolder, Binds.Sprites[key],
                new(new(spriteColWidth, rowHeight), new(0, 0), new(0.5f, 0.5f), new(0.5f, 0.5f), new(0.5f, 0.5f)));


            ButtonSkin skin = UIManager.instance.uiButtonSkins.GetButtonSkinFor(action);
            
            GameObject skinHolder = CanvasUtil.CanvasUtil.CreateBasePanel(_keybindPanel,
                new(
                    new(bindColWidth, rowHeight),
                    new(spriteColWidth, -idx * rowHeight),
                    new(0, 1),
                    new(0, 1),
                    new(0, 1)
                    ));

            CanvasUtil.CanvasUtil.CreateImagePanel(skinHolder, skin.sprite,
                new(
                    new(bindColWidth, rowHeight),
                    new(0, 0),
                    new(0.5f, 0.5f),
                    new(0.5f, 0.5f),
                    new(0.5f, 0.5f)
                    ));

            if (!string.IsNullOrEmpty(skin.symbol))
            {
                int fontSize = skin.skinType switch
                {
                    ButtonSkinType.SQUARE => skin.symbol.Length <= 2 ? 48 : 36,
                    ButtonSkinType.WIDE => 24,
                    _ => 48
                };

                CanvasUtil.CanvasUtil.CreateTextPanel(skinHolder, skin.symbol, fontSize, TextAnchor.MiddleCenter,
                new(
                    new(bindColWidth, rowHeight),
                    new(0, 0),
                    new(0.5f, 0.5f),
                    new(0.5f, 0.5f),
                    new(0.5f, 0.5f)
                    ),
                Fonts.GetFont("Arial")!);
            }

            idx++;
        }
    }

    private void DestroyKeybindPanel()
    {
        if (_keybindPanel != null)
        {
            Destroy(_keybindPanel);
        }
    }

    private void OnDestroy()
    {
        Destroy(_canvas);

        KeybindPermuter.OnRandomize -= SetupKeybindPanel;
        KeybindPermuter.OnRestore -= DestroyKeybindPanel;

    }
}
