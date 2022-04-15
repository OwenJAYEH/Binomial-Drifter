using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ExampleListener : MonoBehaviour
{

    public ButtonHandler primaryAxisClickHandlerRight = null;
    public ButtonHandler primaryAxisClickHandlerLeft = null;
    public AxisHandler2D primaryAxisHandler = null;
    public AxisHandler triggerHandler = null;

    public void OnEnable()
    {
        primaryAxisClickHandlerRight.OnButtonDown += PrintPrimaryButtonDown;
        primaryAxisClickHandlerLeft.OnButtonDown += PrintPrimaryButtonDown;
        primaryAxisHandler.OnValueChange += PrintPrimaryAxis;
        triggerHandler.OnValueChange += PrintTrigger;
    }

    public void OnDisable()
    {
        primaryAxisClickHandlerRight.OnButtonDown -= PrintPrimaryButtonDown;
        primaryAxisClickHandlerLeft.OnButtonDown -= PrintPrimaryButtonDown;
        primaryAxisHandler.OnValueChange -= PrintPrimaryAxis;
        triggerHandler.OnValueChange -= PrintTrigger;
    }

    private void PrintPrimaryButtonDown(XRController controller)
    {
        print("Primary button down");
    }

    private void PrintPrimaryButtonUp(XRController controller)
    {

    }

    private void PrintPrimaryAxis(XRController controller, Vector2 value)
    {
       // print("Primary axis: " + value);
    }

    private void PrintTrigger(XRController controller, float value)
    {
       // print("Trigger: " + value);
    }
}
