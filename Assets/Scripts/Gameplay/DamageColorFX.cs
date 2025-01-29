using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class DamageColorFX : MonoBehaviour
    {
        [SerializeField] Color color;
        [SerializeField] float duration;

        Color originalColor;
        SpriteRenderer renderer;
        readonly List<Coroutine> coroutines = new List<Coroutine>(); 
        
        Shader originalShader;
        Shader hitShader;
    
        void Awake()
        {
            renderer = GetComponent<SpriteRenderer>();
            originalColor = renderer.color;

            originalShader = renderer.material.shader;
            hitShader = Shader.Find("GUI/Text Shader");
        }
    
        public void Flash()
        {
            coroutines.Add(StartCoroutine(FlashRoutine()));
        }
    
        private IEnumerator FlashRoutine()
        {
            renderer.color = color;
            renderer.material.shader = hitShader;
        
            yield return new WaitForSeconds(duration);
        
            renderer.color = originalColor;
            renderer.material.shader = originalShader;
        
            coroutines.RemoveAt(coroutines.Count-1);
        }

        private void OnDisable()
        {
            foreach(var routine in coroutines)
            {
                StopCoroutine(routine);
            }
            coroutines.Clear();
        }
    }
}