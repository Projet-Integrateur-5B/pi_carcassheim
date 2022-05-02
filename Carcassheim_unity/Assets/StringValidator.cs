using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
public class StringValidator : MonoBehaviour
{

    TMP_InputField text_field;
    string last_valid;
    int value;

    public UnityEvent<int> OnValueFinal;

    // Start is called before the first frame update
    void Start()
    {
        text_field = gameObject.GetComponent<TMP_InputField>();
        if (text_field != null)
        {
            last_valid = text_field.text;
            if (!int.TryParse(last_valid, out value))
            {
                value = 0;
                text_field.SetTextWithoutNotify("0");
            }
            text_field.onValueChanged.AddListener(onModif);
            text_field.onEndEdit.AddListener(onEnd);
        }
        else
        {
            Debug.LogWarning("Validator should be connected to an inputstr");
        }
    }

    void onModif(string to_validate)
    {
        if (to_validate.Length == 0)
            return;
        int res;
        if (!int.TryParse(to_validate, out res))
        {
            text_field.SetTextWithoutNotify(last_valid);
        }
        else
        {
            value = res;
            last_valid = to_validate;
        }
    }

    void onEnd(string to_validate)
    {
        int res;
        if (!int.TryParse(to_validate, out res))
        {
            text_field.SetTextWithoutNotify(last_valid);
        }
        else
        {
            value = res;
            last_valid = to_validate;
        }
        OnValueFinal?.Invoke(value);
    }
}
