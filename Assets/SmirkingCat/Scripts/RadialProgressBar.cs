/*
 *
 *  RadialProgressBar.cs	(c) Ryan Sullivan 2018
 *  url: http://smirkingcat.software/
 *
 *  A radial progress bar that fills smoothly then lerps back to empty.
 *  
 */

using UnityEngine;
using UnityEngine.UI;

public class RadialProgressBar : MonoBehaviour {

    private Image _fillImage;
    public float Value { get; set; }
    public float FillAmount { get { return _fillImage.fillAmount; } }

    private float _lerpTime = 5;
    private float _lerpTimer;

    private static float _threshold = 0.01f;

    private void Awake()
    {

        _fillImage = transform.Find("Fill").GetComponent<Image>();

    }


	// Update is called once per frame
	void Update ()
    {

        // When the fill value is set to zero lerp the fillAmount down to zero
        // otherwise set it directly
        if (Value == 0 && _fillImage.fillAmount != 0)
        {

            _lerpTimer += Time.deltaTime;

            if (_lerpTimer > _lerpTime)
            {
                _lerpTimer = _lerpTime;
            }

            // Set to zero if below threshold, otherwise lerp our way there
            _fillImage.fillAmount = (_fillImage.fillAmount < _threshold ? 0 : Mathf.Lerp(_fillImage.fillAmount, 0, _lerpTimer / _lerpTime ));

        }
        else
        {

            if (_lerpTimer != 0) _lerpTimer = 0;

            // Set to one if above threshold, otherwise use the exact value
            _fillImage.fillAmount = (Value > (1 - _threshold) ? 1 : Value);

        }


    }


}