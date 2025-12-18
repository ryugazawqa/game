using UnityEngine;
using static UnityEngine.ParticleSystem;
public class leafParticle : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem leaffParticle;
    private void Awake()
    {
        leaffParticle.Play();
    }
}
