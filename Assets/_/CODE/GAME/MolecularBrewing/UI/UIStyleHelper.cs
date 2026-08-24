//using System;
//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;

//namespace MolecularBrewing.Runtime
//{
//    [Serializable]
//    public class UIThemePalette
//    {
//        public Color m_background;
//        public Color m_headerBg;
//        public Color m_panelBg;
//        public Color m_cardBg;
//        public Color m_wellBg;
//        public Color m_textPrimary;
//        public Color m_textSecondary;
//        public Color m_textMuted;
//        public Color m_accentCyan;
//        public Color m_accentGreen;
//        public Color m_accentAmber;
//        public Color m_btnPrimary;
//        public Color m_btnSecondary;
//        public Color m_btnDanger;
//        public Color m_lensBezel;
//        public Color m_lensGlass;
//        public Color m_lensReticle;
//        public Color m_beakerFill;
//    }

//    public static class UIStyleHelper
//    {
//        #region Publics

//        public static bool IsLightMode = true;

//        public static UIThemePalette LightTheme = new UIThemePalette
//        {
//            m_background = new Color(0.12f, 0.15f, 0.20f, 1f),
//            m_headerBg = new Color(0.08f, 0.11f, 0.15f, 0.98f),
//            m_panelBg = new Color(0.15f, 0.19f, 0.24f, 0.96f),
//            m_cardBg = new Color(0.19f, 0.24f, 0.30f, 0.95f),
//            m_wellBg = new Color(0.10f, 0.13f, 0.17f, 0.96f),
//            m_textPrimary = new Color(0.94f, 0.96f, 0.98f, 1f),
//            m_textSecondary = new Color(0.68f, 0.76f, 0.84f, 1f),
//            m_textMuted = new Color(0.48f, 0.56f, 0.65f, 1f),
//            m_accentCyan = new Color(0.22f, 0.74f, 0.96f, 1f),
//            m_accentGreen = new Color(0.18f, 0.78f, 0.54f, 1f),
//            m_accentAmber = new Color(0.96f, 0.64f, 0.18f, 1f),
//            m_btnPrimary = new Color(0.14f, 0.50f, 0.68f, 1f),
//            m_btnSecondary = new Color(0.25f, 0.31f, 0.38f, 1f),
//            m_btnDanger = new Color(0.78f, 0.24f, 0.28f, 1f),
//            m_lensBezel = new Color(0.25f, 0.55f, 0.72f, 0.50f),
//            m_lensGlass = new Color(0.06f, 0.09f, 0.13f, 0.98f),
//            m_lensReticle = new Color(0.25f, 0.70f, 0.90f, 0.18f),
//            m_beakerFill = new Color(0.20f, 0.68f, 0.88f, 0.22f)
//        };

//        public static UIThemePalette DarkTheme = new UIThemePalette
//        {
//            m_background = new Color(0.05f, 0.07f, 0.09f, 0.98f),
//            m_headerBg = new Color(0.09f, 0.12f, 0.16f, 0.95f),
//            m_panelBg = new Color(0.08f, 0.11f, 0.15f, 0.90f),
//            m_cardBg = new Color(0.06f, 0.08f, 0.11f, 0.95f),
//            m_wellBg = new Color(0.04f, 0.06f, 0.08f, 0.96f),
//            m_textPrimary = new Color(0.95f, 0.98f, 1f, 1f),
//            m_textSecondary = new Color(0.80f, 0.88f, 0.95f, 0.85f),
//            m_textMuted = new Color(0.55f, 0.65f, 0.75f, 0.70f),
//            m_accentCyan = new Color(0.35f, 0.90f, 0.85f, 1f),
//            m_accentGreen = new Color(0.30f, 0.85f, 0.40f, 1f),
//            m_accentAmber = new Color(0.95f, 0.80f, 0.40f, 1f),
//            m_btnPrimary = new Color(0.20f, 0.50f, 0.65f, 1f),
//            m_btnSecondary = new Color(0.25f, 0.30f, 0.38f, 1f),
//            m_btnDanger = new Color(0.35f, 0.18f, 0.20f, 1f),
//            m_lensBezel = new Color(0.12f, 0.35f, 0.48f, 0.45f),
//            m_lensGlass = new Color(0.04f, 0.07f, 0.11f, 0.98f),
//            m_lensReticle = new Color(0.18f, 0.45f, 0.60f, 0.15f),
//            m_beakerFill = new Color(0.20f, 0.60f, 0.80f, 0.15f)
//        };

//        public static UIThemePalette Theme => IsLightMode ? LightTheme : DarkTheme;

//        public static Sprite RoundedSprite
//        {
//            get
//            {
//                if (_roundedSprite == null)
//                {
//                    _roundedSprite = CreateRoundedRectSprite(64, 64, 16);
//                }
//                return _roundedSprite;
//            }
//        }

//        public static Sprite CircleSprite
//        {
//            get
//            {
//                if (_circleSprite == null)
//                {
//                    _circleSprite = CreateCircleSprite(64);
//                }
//                return _circleSprite;
//            }
//        }

//        #endregion


//        #region Main API

//        public static GameObject CreatePanel(Transform parent, string name, Color color, Sprite sprite = null)
//        {
//            GameObject obj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
//            obj.transform.SetParent(parent, false);
//            Image img = obj.GetComponent<Image>();
//            img.sprite = sprite != null ? sprite : RoundedSprite;
//            img.type = Image.Type.Sliced;
//            img.color = color;
//            return obj;
//        }

//        public static TextMeshProUGUI CreateText(Transform parent, string name, string text, float fontSize, Color color, TextAlignmentOptions alignment = TextAlignmentOptions.Center, FontStyles fontStyle = FontStyles.Normal)
//        {
//            GameObject obj = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
//            obj.transform.SetParent(parent, false);
//            TextMeshProUGUI tmp = obj.GetComponent<TextMeshProUGUI>();
//            tmp.text = text;
//            tmp.fontSize = fontSize;
//            tmp.color = color;
//            tmp.alignment = alignment;
//            tmp.fontStyle = fontStyle;
//            tmp.raycastTarget = false;
//            return tmp;
//        }

//        public static Button CreateButton(Transform parent, string name, string label, Color bgColor, Color textColor, Action onClick)
//        {
//            GameObject btnObj = CreatePanel(parent, name, bgColor, RoundedSprite);
//            Button btn = btnObj.AddComponent<Button>();
            
//            ColorBlock cb = btn.colors;
//            cb.normalColor = bgColor;
//            cb.highlightedColor = bgColor * 1.12f;
//            cb.pressedColor = bgColor * 0.88f;
//            cb.selectedColor = bgColor;
//            btn.colors = cb;

//            btn.onClick.AddListener(() => AudioManager.PlayMenuNav());

//            if (onClick != null)
//            {
//                btn.onClick.AddListener(() => onClick());
//            }

//            TextMeshProUGUI text = CreateText(btnObj.transform, "Label", label, 15, textColor, TextAlignmentOptions.Center, FontStyles.Bold);
//            RectTransform rt = text.rectTransform;
//            rt.anchorMin = Vector2.zero;
//            rt.anchorMax = Vector2.one;
//            rt.sizeDelta = Vector2.zero;

//            return btn;
//        }

//        #endregion


//        #region Tools and Utilities

//        public static Sprite CreateRoundedRectSprite(int width, int height, int radius)
//        {
//            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
//            Color[] colors = new Color[width * height];

//            for (int y = 0; y < height; y++)
//            {
//                for (int x = 0; x < width; x++)
//                {
//                    int dx = Mathf.Min(x, width - 1 - x);
//                    int dy = Mathf.Min(y, height - 1 - y);

//                    if (dx < radius && dy < radius)
//                    {
//                        float dist = Vector2.Distance(new Vector2(dx, dy), new Vector2(radius, radius));
//                        float alpha = Mathf.Clamp01(radius - dist + 0.5f);
//                        colors[y * width + x] = new Color(1, 1, 1, alpha);
//                    }
//                    else
//                    {
//                        colors[y * width + x] = Color.white;
//                    }
//                }
//            }

//            tex.SetPixels(colors);
//            tex.Apply();
//            return Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100, 0, SpriteMeshType.FullRect, new Vector4(radius, radius, radius, radius));
//        }

//        public static Sprite CreateCircleSprite(int size)
//        {
//            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
//            Color[] colors = new Color[size * size];
//            float center = (size - 1) * 0.5f;
//            float radius = center;

//            for (int y = 0; y < size; y++)
//            {
//                for (int x = 0; x < size; x++)
//                {
//                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
//                    float alpha = Mathf.Clamp01(radius - dist + 0.5f);
//                    colors[y * size + x] = new Color(1, 1, 1, alpha);
//                }
//            }

//            tex.SetPixels(colors);
//            tex.Apply();
//            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
//        }

//        #endregion


//        #region Private and Protected

//        private static Sprite _roundedSprite;
//        private static Sprite _circleSprite;

//        #endregion
//    }
//}
