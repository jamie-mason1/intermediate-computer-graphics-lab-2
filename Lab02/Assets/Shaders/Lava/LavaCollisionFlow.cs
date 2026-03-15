using UnityEngine;

[ExecuteAlways]  // Runs in Play Mode AND Edit Mode
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(MeshRenderer))]
public class LavaCollisionFlow : MonoBehaviour
{
    [Header("Collision Flow Settings")]
    public float collisionRadius = 1.5f;      // How far lava is affected
    public float collisionStrength = 0.5f;    // How strongly lava diverts

    private Collider col;
    private Material lavaMaterial;

    // Shader property IDs
    private static readonly int ColliderPosID = Shader.PropertyToID("_ColliderPos");
    private static readonly int ColliderRadiusID = Shader.PropertyToID("_ColliderRadius");
    private static readonly int CollisionStrengthID = Shader.PropertyToID("_CollisionStrength");

    void Awake()
    {
        col = GetComponent<Collider>();

        // Grab the material from the MeshRenderer
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            // Use material instance if you want per-object behavior
            lavaMaterial = renderer.material;
        }
    }
    void OnCollisionStay(Collision collision)
{
    if (lavaMaterial == null) return;

    // Only react to objects tagged "Floor"
    if (collision.gameObject.CompareTag("Floor"))
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            Vector3 hitPoint = contact.point;  // Exact point of collision
            // Send this point to the shader
        }
    }
}

    void Update()
    {
        if (lavaMaterial == null || col == null) return;

        Vector3 colliderWorldPos = col.bounds.center;

        // Set the shader properties
        //lavaMaterial.SetVector(ColliderPosID, colliderWorldPos);
        lavaMaterial.SetFloat(ColliderRadiusID, collisionRadius);
        lavaMaterial.SetFloat(CollisionStrengthID, collisionStrength);
    }
}