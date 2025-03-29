using UnityEngine;

public class GlobalReferences : MonoBehaviour
{
    public static GlobalReferences Instance { get; set; }

    public GameObject bulletImpactStoneEffect; // Stone
    public GameObject bulletImpactMetalEffect; // Metal
    public GameObject bulletImpactWoodEffect;  // Wood
    public GameObject bulletImpactSandEffect;  // Sand
    public GameObject bulletImpactFleshEffect; // Flesh

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
}
