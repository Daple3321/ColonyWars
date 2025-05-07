using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OutlineFeature
{
    /*[System.Serializable]
    public enum FilterMode
    {
        LayerMask,
        Tag,
        MarkerComponent
    }

    [System.Serializable]
    public class OutlineSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
        public FilterMode filterMode = FilterMode.MarkerComponent;
        public LayerMask outlineLayer; // Используется, если filterMode = LayerMask
        public string outlineTag;      // Используется, если filterMode = Tag
        public Material outlineMaterial; // Ваш материал контура (inverted hull)
        [Range(1, 255)] // 0 обычно для "ничего"
        public int stencilReference = 1;
    }

    public OutlineSettings settings = new OutlineSettings();

    private StencilWritePass stencilWritePass;
    private OutlineDrawPass outlineDrawPass;
    private List<Renderer> renderersToOutline = new List<Renderer>(); // Список рендереров для обводки

    public override void Create()
    {
        if (settings.outlineMaterial == null)
        {
            Debug.LogWarningFormat("{0}: Outline Material не назначен в OutlineStencilFeature.", GetType().Name);
            return;
        }

        // Проходы будут использовать общий список renderersToOutline
        stencilWritePass = new StencilWritePass(settings.renderPassEvent, settings.stencilReference, renderersToOutline);
        outlineDrawPass = new OutlineDrawPass(settings.renderPassEvent + 1, settings.outlineMaterial, settings.stencilReference, renderersToOutline);
    }

    // Этот метод вызывается для каждой камеры один раз за кадр перед EnqueuePasses.
    public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    {
        if (settings.outlineMaterial == null) return;

        // Очищаем и заполняем список рендереров перед тем, как передать его в проходы
        renderersToOutline.Clear();
        CollectRenderers(renderingData.cameraData.camera); // Собираем рендереры для текущей камеры
    }


    private void CollectRenderers(Camera camera)
    {
        // Эта логика может быть более сложной в зависимости от потребностей
        // (например, учитывать frustum culling камеры и т.д.)
        // Сейчас для простоты ищем по всей сцене.

        if (settings.filterMode == FilterMode.MarkerComponent)
        {
            OutlineTarget[] targets = FindObjectsOfType<OutlineTarget>(); // MonoBehaviour.FindObjectsOfType - может быть медленным при частом вызове. Рассмотрите кеширование или альтернативные методы сбора.
            foreach (var target in targets)
            {
                Renderer r = target.GetComponent<Renderer>();
                if (r != null && r.enabled && r.isVisible && target.gameObject.activeInHierarchy) // Добавим базовые проверки
                {
                    // Дополнительная проверка на попадание в область видимости камеры (опционально, если объекты могут быть далеко)
                    // GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(camera), r.bounds)
                    renderersToOutline.Add(r);
                }
            }
        }
        else if (settings.filterMode == FilterMode.Tag)
        {
            if (string.IsNullOrEmpty(settings.outlineTag)) return;
            GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(settings.outlineTag);
            foreach (var obj in taggedObjects)
            {
                Renderer r = obj.GetComponent<Renderer>();
                 if (r != null && r.enabled && r.isVisible && obj.activeInHierarchy)
                {
                    renderersToOutline.Add(r);
                }
            }
        }
        else // FilterMode.LayerMask
        {
            // Для LayerMask, мы бы обычно использовали CullingResults в ScriptableRenderPass,
            // но для единообразия с другими методами и для прямого контроля,
            // мы также можем найти объекты по слою.
            // Это менее эффективно, чем работа с CullingResults.
            Renderer[] allRenderers = FindObjectsOfType<Renderer>();
            foreach (var r in allRenderers)
            {
                if (r != null && r.enabled && r.isVisible && r.gameObject.activeInHierarchy &&
                    ((1 << r.gameObject.layer) & settings.outlineLayer.value) != 0)
                {
                    renderersToOutline.Add(r);
                }
            }
        }
    }


    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.outlineMaterial == null || renderersToOutline.Count == 0)
        {
            return;
        }
        
        // Убедимся, что камера будет использовать Stencil Buffer
        // Это важно, если URP не делает это автоматически.
        // Настройка ConfigureInput(ScriptableRenderPassInput.Stencil) в проходах также может помочь.
        renderingData.cameraData.requiresStencil = true; // Сообщаем URP, что нам нужен Stencil.

        renderer.EnqueuePass(stencilWritePass);
        renderer.EnqueuePass(outlineDrawPass);
    }*/
}
