using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class autoselect_tab : MonoBehaviour
{
    [SerializeField] List<InputField> inputs;

    [SerializeField] List<Button> event_on_validate;

    private int index_field = -1;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Tab) && inputs.Count > 0)
        {
            index_field = (index_field + 1) % inputs.Count;
            inputs[index_field].Select();
            inputs[index_field].ActivateInputField();
        }
        if (Input.GetKeyUp(KeyCode.Return) && index_field > 0)
        {
            foreach (Button bt in event_on_validate)
            {
                bt.onClick?.Invoke();
            }
        }
    }

    void test()
    {
        Debug.Log("debug");
    }


    void OnEnable()
    {
        if (inputs.Count > 0)
        {
            index_field = 0;
            inputs[index_field].Select();
            inputs[index_field].ActivateInputField();
        }
    }

    void OnDisable()
    {
        index_field = -1;
    }

}
