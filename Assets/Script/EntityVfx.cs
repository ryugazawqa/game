using System.Collections;
using UnityEngine;
using System.Collections.Generic; // Dictionary kullanmak için gerekli

public class EntityVfx : MonoBehaviour
{

    [SerializeField] private SpriteRenderer SrBody; 
    [SerializeField] private SpriteRenderer SrHead;
    [SerializeField] private SpriteRenderer SrWing;
    [SerializeField] private SpriteRenderer SrLeftLeg;
    [SerializeField] private SpriteRenderer SrRightLeg;

    private List<SpriteRenderer> allRenderers = new List<SpriteRenderer>();
    

    private Dictionary<SpriteRenderer, Material> originalMaterials = new Dictionary<SpriteRenderer, Material>();

    [SerializeField] private Material onDamageVfxMat;
    [SerializeField] private float onDamageVfxDuration = .15f;
    private Coroutine onDamageVfxCo;

    private void Awake()
    {
        
        allRenderers.Add(SrBody); 
        allRenderers.Add(SrHead);
        allRenderers.Add(SrWing);
        allRenderers.Add(SrLeftLeg);
        allRenderers.Add(SrRightLeg);

        foreach (var sr in allRenderers)
        {
          
            if (sr != null)
            {

                originalMaterials.Add(sr, sr.material); 
            }
        }
    }

    public void PlayOnDamageVfx()
    {
        if(onDamageVfxCo != null)
        {
            StopCoroutine(onDamageVfxCo);
        }
        onDamageVfxCo = StartCoroutine(OnDamageVfxCo());
    }

    private IEnumerator OnDamageVfxCo()
    {

        foreach (var sr in allRenderers)
        {
            if (sr != null)
            {
                sr.material = onDamageVfxMat;
            }
        }
        
        yield return new WaitForSeconds(onDamageVfxDuration);

       
        foreach (var sr in allRenderers)
        {
         
            if (sr != null && originalMaterials.ContainsKey(sr))
            {
                sr.material = originalMaterials[sr];
            }
        }

        onDamageVfxCo = null;
    }
}