using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeDisplay : MonoBehaviour
{
    [SerializeField] private Slider slider;
    private TMP_Text text;

    private int hour;
    private float minute;

    private void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    public void DisplayTime()
    {
        hour = Mathf.FloorToInt(slider.value / 60);
        minute = slider.value % 60;

        if (hour > 0)
        {
            text.text = hour.ToString() + " hours " + minute + " minutes";
        }
        else
        {
            text.text = minute.ToString() + " minutes";
        }
    }
}
