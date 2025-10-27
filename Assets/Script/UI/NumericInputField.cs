using System.Linq;
using TMPro;
using UnityEngine;

public class NumericInputField : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;

    private void Awake()
    {
        inputField.onValueChanged.AddListener(ValidateInput);
        inputField.text = "0G";
    }

    private void ValidateInput(string value)
    {
        string numericOnly = string.Concat(value.Where(char.IsDigit));
        //if (value != numericOnly)
            inputField.text = numericOnly + "G";
    }

    public int GetNumericValue()
    {
        string value = inputField.text;
        string numericOnly = string.Concat(value.Where(char.IsDigit));

        // 예외 방지를 위해 TryParse 사용
        if (int.TryParse(numericOnly, out int result))
        {
            return result;
        }
        else
        {
            return 0; // 숫자가 없거나 변환 실패 시 기본값
        }
    }
}
