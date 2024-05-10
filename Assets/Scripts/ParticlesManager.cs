using System.Collections.Generic;
using UnityEngine;

public class ParticlesManager : MonoBehaviour
{
    public static ParticlesManager i;

    public GameObject circleParticlePrefab;
    
    private void Awake()
    {
        i = this;
    }
    
    public void CircleParticles(Vector2 worldPosition, float scale)
    {
        MeshRenderer s = GameObject.Instantiate(circleParticlePrefab).GetComponent<MeshRenderer>();
        Material m = s.material;

        s.transform.position = worldPosition;
        s.transform.localScale = Vector3.one * scale;

        float FLOW_AMOUNT = 0.2f;
        float DURATION = 0.4f;
        float ALPHA = 0.7f;

        LeanTween.value(s.gameObject, 0, 1 - FLOW_AMOUNT, DURATION).setOnUpdate((val) => {
            m.SetFloat("_InnerSize", val);
        }).setEaseOutQuad();

        LeanTween.value(s.gameObject, 0, 1 - FLOW_AMOUNT, DURATION).setOnUpdate((val) => {
            m.SetFloat("_OuterSize", val);
        }).setEaseOutExpo();
        
        LeanTween.value(s.gameObject, 1, 0, DURATION).setOnUpdate((val) => {
            m.SetColor("_Color", new Color(1.0f, 1.0f, 1.0f, ALPHA * val));
        });

        LeanTween.value(s.gameObject, 0, FLOW_AMOUNT, DURATION).setOnUpdate((val) => {
            m.SetFloat("_FlowAmount", val);
        }).setEaseInQuart().setOnComplete(() => Destroy(s.gameObject));
    }     
}
