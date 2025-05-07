using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OutlineDrawPass : ScriptableRenderPass
{
    /*private LayerMask layerMask;
    private Material outlineMaterial;
    private int stencilReference;
    private FilteringSettings filteringSettings;
    private List<ShaderTagId> shaderTagIdList = new List<ShaderTagId>(); // Для DrawingSettings
    private RenderStateBlock stencilTestStateBlock;

    public OutlineDrawPass(RenderPassEvent renderPassEvent, LayerMask layerMask, Material outlineMat, int stencilRef)
    {
        this.renderPassEvent = renderPassEvent;
        this.layerMask = layerMask;
        this.outlineMaterial = outlineMat;
        this.stencilReference = stencilRef;

        filteringSettings = new FilteringSettings(RenderQueueRange.opaque, layerMask);
        
        // Нам все еще нужны ShaderTagId, чтобы DrawingSettings были валидными,
        // даже если мы используем overrideMaterial.
        shaderTagIdList.Add(new ShaderTagId("SRPDefaultUnlit"));
        shaderTagIdList.Add(new ShaderTagId("UniversalForward"));
        shaderTagIdList.Add(new ShaderTagId("UniversalForwardOnly"));

        stencilTestStateBlock = new RenderStateBlock(RenderStateMask.Stencil);
        stencilTestStateBlock.stencilReference = stencilReference;
        stencilTestStateBlock.stencilState = new StencilState(
            enabled: true,
            readMask: 255,
            writeMask: 0, // Обычно не пишем в stencil во время отрисовки контура,
                           // или используем StencilOp.Zero для passOperation, чтобы очистить
            compareFunction: CompareFunction.Equal, // Рисовать только если stencil == stencilReference
            passOperation: StencilOp.Keep,        // Не изменять stencil
            failOperation: StencilOp.Keep,
            zFailOperation: StencilOp.Keep
        );
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (outlineMaterial == null) return;

        CommandBuffer cmd = CommandBufferPool.Get("OutlineDrawPass");

        var drawingSettings = CreateDrawingSettings(shaderTagIdList, ref renderingData, renderingData.cameraData.defaultOpaqueSortFlags);
        drawingSettings.overrideMaterial = outlineMaterial; // Используем материал контура

        context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings, ref stencilTestStateBlock);
        
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }*/
}
