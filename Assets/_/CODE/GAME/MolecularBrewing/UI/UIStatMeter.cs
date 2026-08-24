//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;

//namespace MolecularBrewing.Runtime
//{
//    public class UIStatMeter : MonoBehaviour
//    {
//        #region Publics

//        public float m_targetValue;
//        public float m_currentValue;

//        #endregion


//        #region Unity API

//        private void Update()
//        {
//            if (Mathf.Abs(m_currentValue - m_targetValue) > 0.1f)
//            {
//                m_currentValue = Mathf.Lerp(m_currentValue, m_targetValue, Time.deltaTime * 12f);
//                UpdateDisplay();
//            }
//        }

//        #endregion


//        #region Main API

//        public void Initialize(string statName, Color barColor)
//        {
//            if (!HasSerializedBindings())
//            {
//                BuildProceduralUI(statName, barColor);
//            }
//            else
//            {
//                ApplyConfiguration(statName, barColor);
//            }
//        }

//        public void SetValue(float value, bool instant = false)
//        {
//            m_targetValue = Mathf.Clamp(value, 0f, 100f);
//            if (instant)
//            {
//                m_currentValue = m_targetValue;
//                UpdateDisplay();
//            }
//        }

//        public void SetBarColor(Color color)
//        {
//            if (_fillImage != null) _fillImage.color = color;
//            if (_valueText != null) _valueText.color = color;
//        }

//        #endregion


//        #region Tools and Utilities

//        private bool HasSerializedBindings()
//        {
//            return _fillImage != null && _label != null && _valueText != null;
//        }

//        private void ApplyConfiguration(string statName, Color barColor)
//        {
//            if (_label != null) _label.text = statName;
//            if (_fillImage != null)
//            {
//                _fillImage.color = barColor;
//                _fillImage.fillAmount = 0f;
//            }
//            if (_valueText != null)
//            {
//                _valueText.color = barColor;
//                _valueText.text = "0";
//            }
//        }

//        private void BuildProceduralUI(string statName, Color barColor)
//        {
//            RectTransform rt = GetComponent<RectTransform>();
//            if (rt == null) rt = gameObject.AddComponent<RectTransform>();
//            rt.sizeDelta = new Vector2(240, 28);

//            // Header Container
//            GameObject headerObj = new GameObject("Header", typeof(RectTransform));
//            headerObj.transform.SetParent(transform, false);
//            RectTransform hRt = headerObj.GetComponent<RectTransform>();
//            hRt.anchorMin = new Vector2(0f, 0.5f);
//            hRt.anchorMax = new Vector2(1f, 1f);
//            hRt.offsetMin = Vector2.zero;
//            hRt.offsetMax = Vector2.zero;

//            var t = UIStyleHelper.Theme;

//            _label = UIStyleHelper.CreateText(headerObj.transform, "Label", statName, 12, t.m_textPrimary, TextAlignmentOptions.Left, FontStyles.Bold);
//            RectTransform lRt = _label.rectTransform;
//            lRt.anchorMin = new Vector2(0f, 0f);
//            lRt.anchorMax = new Vector2(0.7f, 1f);
//            lRt.offsetMin = Vector2.zero;
//            lRt.offsetMax = Vector2.zero;

//            _valueText = UIStyleHelper.CreateText(headerObj.transform, "Value", "0", 12, barColor, TextAlignmentOptions.Right, FontStyles.Bold);
//            RectTransform vRt = _valueText.rectTransform;
//            vRt.anchorMin = new Vector2(0.7f, 0f);
//            vRt.anchorMax = new Vector2(1f, 1f);
//            vRt.offsetMin = Vector2.zero;
//            vRt.offsetMax = Vector2.zero;

//            // Bar Background
//            GameObject bgObj = UIStyleHelper.CreatePanel(transform, "BarBg", t.m_wellBg, UIStyleHelper.RoundedSprite);
//            _barBgImage = bgObj.GetComponent<Image>();
//            RectTransform bgRt = bgObj.GetComponent<RectTransform>();
//            bgRt.anchorMin = new Vector2(0f, 0f);
//            bgRt.anchorMax = new Vector2(1f, 0.45f);
//            bgRt.offsetMin = Vector2.zero;
//            bgRt.offsetMax = Vector2.zero;

//            // Fill Bar
//            GameObject fillObj = UIStyleHelper.CreatePanel(bgObj.transform, "Fill", barColor, UIStyleHelper.RoundedSprite);
//            _fillImage = fillObj.GetComponent<Image>();
//            _fillImage.type = Image.Type.Filled;
//            _fillImage.fillMethod = Image.FillMethod.Horizontal;
//            _fillImage.fillOrigin = 0;
//            _fillImage.fillAmount = 0f;
//            RectTransform fRt = _fillImage.rectTransform;
//            fRt.anchorMin = Vector2.zero;
//            fRt.anchorMax = Vector2.one;
//            fRt.offsetMin = new Vector2(2, 2);
//            fRt.offsetMax = new Vector2(-2, -2);
//        }

//        private void UpdateDisplay()
//        {
//            if (_fillImage != null)
//            {
//                _fillImage.fillAmount = m_currentValue / 100f;
//            }
//            if (_valueText != null)
//            {
//                _valueText.text = Mathf.RoundToInt(m_currentValue).ToString();
//            }
//        }

//        #endregion


//        #region Private and Protected

//        [Header("GameObject Serialized Bindings")]
//        [SerializeField] private TextMeshProUGUI _label;
//        [SerializeField] private TextMeshProUGUI _valueText;
//        [SerializeField] private Image _fillImage;
//        [SerializeField] private Image _barBgImage;

//        #endregion
//    }
//}
