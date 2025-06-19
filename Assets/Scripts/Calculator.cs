using Microsoft.Win32.SafeHandles;
using UnityEngine;

public class Calculator : MonoBehaviour
{
    public float Number1;
    public float Number2;
    public float Screen;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
     
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.KeypadPlus) || Input.GetKeyDown(KeyCode.Equals))
        {
            Screen = Add(Number1, Number2);
            Debug.Log(Screen);
        }
        if (Input.GetKeyDown(KeyCode.KeypadMinus) || Input.GetKeyDown(KeyCode.Minus))
        {
            Screen = Subtract(Number1, Number2);
            Debug.Log(Screen);
        }
        if(Input.GetKeyDown(KeyCode.KeypadDivide))
        {
            Screen = Divide(Number1, Number2);
            Debug.Log(Screen);
        }
        if(Input.GetKeyDown(KeyCode.KeypadMultiply))
        {
            Screen = Multiply(Number1, Number2);
            Debug.Log(Screen);
        }
    }

    private float Add(float number1, float number2)
    {
        float result = number1 + number2;
        return result;
    }

    private float Subtract(float number1, float number2)
    {
        float result = number1 - number2;
        return result;
    }

    private float Divide(float number1, float number2)
    {
        float result = number1 / number2;
        return result;
    }

    private float Multiply(float number1, float number2)
    {
        float result = number1 * number2;
        return result;
    }
}
