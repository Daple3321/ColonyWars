using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class StencilWritePass : ScriptableRenderPass
{
    /*private LayerMask layerMask;
    private int stencilReference;
    private FilteringSettings filteringSettings;
    private List<ShaderTagId> shaderTagIdList = new List<ShaderTagId>();
    private RenderStateBlock stencilWriteStateBlock;

    public StencilWritePass(RenderPassEvent renderPassEvent, LayerMask layerMask, int stencilRef)
    {
        this.renderPassEvent = renderPassEvent;
        this.layerMask = layerMask;
        this.stencilReference = stencilRef;

        filteringSettings = new FilteringSettings(RenderQueueRange.opaque, layerMask);

        // Стандартные ShaderTagId для URP (Lit, SimpleLit, Unlit)
        shaderTagIdList.Add(new ShaderTagId("SRPDefaultUnlit"));
        shaderTagIdList.Add(new ShaderTagId("UniversalForward"));
        shaderTagIdList.Add(new ShaderTagId("UniversalForwardOnly"));
        // Добавьте другие теги, если ваши объекты используют кастомные шейдеры/теги

        stencilWriteStateBlock = new RenderStateBlock(RenderStateMask.Stencil);
        stencilWriteStateBlock.stencilReference = stencilReference;
        stencilWriteStateBlock.stencilState = new StencilState(
            enabled: true,
            readMask: 0, // Мы не читаем, только пишем
            writeMask: 255, // Пишем во все биты
            compareFunction: CompareFunction.Always, // Всегда проходить тест, чтобы записать
            passOperation: StencilOp.Replace,      // Заменить значение в stencil на stencilReference
            failOperation: StencilOp.Keep,         // Не важно
            zFailOperation: StencilOp.Keep         // Не важно, если объект не прошел Z-тест
        );
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get("StencilWritePass");
        
        // Настройки отрисовки (используем стандартные шейдеры объектов)
        var drawingSettings = CreateDrawingSettings(shaderTagIdList, ref renderingData, renderingData.cameraData.defaultOpaqueSortFlags);

        context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings, ref stencilWriteStateBlock);
        
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }*/
}
