using System.Collections.Generic;
using UnityEngine;

public class ThemedObject : MonoBehaviour
{
    public Component targetComponent;
    public string propertyName;
    public ColorManager.ThemeColor color;

    private void Start() {
        ColorManager.i.objects.Add(this);
    }

    private void Update() {
        Color targetColor = ColorManager.i.GetColor(color);
        System.Type type = targetComponent.GetType();
        var property = type.GetProperty(propertyName, 
            System.Reflection.BindingFlags.FlattenHierarchy 
            | System.Reflection.BindingFlags.Instance 
            | System.Reflection.BindingFlags.NonPublic 
            | System.Reflection.BindingFlags.Public
        );

        if (property != null) {
            property.SetValue(targetComponent, targetColor);
        }
        else {
            var field = type.GetField(propertyName, 
                System.Reflection.BindingFlags.FlattenHierarchy 
                | System.Reflection.BindingFlags.Instance 
                | System.Reflection.BindingFlags.NonPublic 
                | System.Reflection.BindingFlags.Public
            );

            if (field != null) {
                field.SetValue(targetComponent, targetColor);
            }
            else {
                Debug.LogError($"Didn't found property \"{propertyName}\"");
            }
        }
    }

    private void OnDestroy() {
        ColorManager.i.objects.Remove(this);
    }

    public void SetColor() {

    }
}
