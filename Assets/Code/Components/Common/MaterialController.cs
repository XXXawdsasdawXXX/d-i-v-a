using UnityEngine;

namespace Code.Components.Objects
{
    public class MaterialController : MonoBehaviour
    {

        public Material targetMaterial;
        public string propertyName = "_ShineRotate";
        public float newValue = 3.14f; // Новое значение для угла в радианах
        [SerializeField] private bool _isOn;

        public string featureName = "SHINE_ON"; // Имя директивы


        public void SetShineAngle()
        {
            if (targetMaterial != null && targetMaterial.HasProperty(propertyName))
            {
                // Устанавливаем новое значение параметра в шейдере
                targetMaterial.SetFloat(propertyName, newValue);
            }
            else
            {
                Debug.LogError("Материал не содержит свойство с именем " + propertyName);
            }
        }


        public void SetShineAngle(float value)
        {
            targetMaterial.SetFloat(propertyName, value);
        }

        public void SetActiveShine()
        {
            // Проверяем, что у материала есть нужная директива
            if (targetMaterial != null && targetMaterial.shader.isSupported)
            {
              
                if (_isOn && !targetMaterial.IsKeywordEnabled(featureName))
                {
                 targetMaterial.EnableKeyword(featureName);
                    
                }
                else 
                {
                  targetMaterial.DisableKeyword(featureName);
                    
                }
            }
            else
            {
                Debug.LogError("Материал не поддерживает директиву " + featureName);
            }
        }
    }
}