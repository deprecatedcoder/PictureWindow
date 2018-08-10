/*
 *
 *  PictureWindowUI.cs	(c) Ryan Sullivan 2018
 *  url: http://smirkingcat.software/picturewindow
 *
 *  Manages UI related to the PictureWindow
 *  
 */

using UnityEngine;
using UnityEngine.UI;

namespace SmirkingCat.PictureWindow
{

    public class PictureWindowUI : MonoBehaviour
    {

        private GameObject _instruction, _reset, _subtitle;

        private RectTransform _instructionArrow;

        private Text _instructionText, _subtitleText;

        public RadialProgressBar ResetBar { get; set; }
        private CanvasGroup _resetCG;

        private float _subtitleTimeout;
        private float _subtitleDuration = 2f;


        public enum INSTRUCTION
        {

            None,
            TopLeft,
            BottomLeft,
            BottomRight,

        }


        private void Awake()
        {

            // Get all the instruction stuff and default to the first one
            _instruction = transform.Find("Canvas/Instruction").gameObject;
            _instructionArrow = _instruction.transform.Find("Arrow").GetComponentInChildren<RectTransform>();
            _instructionText = _instruction.transform.Find("Text").GetComponentInChildren<Text>();

            ShowInstruction(INSTRUCTION.TopLeft);

            // Get the reset meter and related components
            _reset = transform.Find("Canvas/Reset").gameObject;

            ResetBar = _reset.GetComponentInChildren<RadialProgressBar>();
            _resetCG = _reset.GetComponent<CanvasGroup>();

            // Get the subtitle object and text component then turn it off for now
            _subtitle = transform.Find("Canvas/Subtitle").gameObject;
            _subtitleText = _subtitle.transform.Find("Text").GetComponentInChildren<Text>();
            _subtitle.SetActive(false);

        }


        private void Update()
        {

            _resetCG.alpha = Mathf.Lerp(0, 1, ResetBar.FillAmount);

            // If on, turn the subtitle off after _subtitleDuration seconds
            if (_subtitle.activeInHierarchy && Time.time > _subtitleTimeout)
                _subtitle.SetActive(false);

        }


        public void ShowSubtitle(string sub)
        {

            _subtitle.SetActive(true);
            _subtitleTimeout = Time.time + _subtitleDuration;
            _subtitleText.text = sub;

        }


        public void ShowInstruction(INSTRUCTION instruction)
        {

            switch (instruction)
            {

                case INSTRUCTION.TopLeft:
                    ShowInstruction(new Vector2(110, -110), new Vector2(0, 1), new Vector2(0, 1), Quaternion.Euler(0, 0, 180), "top left");
                    break;

                case INSTRUCTION.BottomLeft:
                    ShowInstruction(new Vector2(110, 110), new Vector2(0, 0), new Vector2(0, 0), Quaternion.Euler(0, 0, 270), "bottom left");
                    break;

                case INSTRUCTION.BottomRight:
                    ShowInstruction(new Vector2(-110, 110), new Vector2(1, 0), new Vector2(1, 0), Quaternion.Euler(0, 0, 0), "bottom right");
                    break;

                case INSTRUCTION.None:
                default:
                    _instruction.SetActive(false);
                    break;

            }


        }


        private void ShowInstruction(Vector2 anchor, Vector2 anchorMin, Vector2 anchorMax, Quaternion rot, string corner)
        {

            _instruction.SetActive(true);
            _instructionArrow.anchoredPosition = anchor;
            _instructionArrow.anchorMin = anchorMin;
            _instructionArrow.anchorMax = anchorMax;
            _instructionArrow.rotation = rot;
            _instructionText.text = "Touch the tip of" + "\n" +
                "the controller to the" + "\n" +
                corner + "\n" +
                "of the display" + "\n" +
                "and pull the trigger";

        }


    }


}