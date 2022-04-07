using SharpDX;
using SharpDX.Direct3D11;

namespace FR.CascadeShadows.Rendering;

public interface IRenderingPipeline
{
    void Clear(DeviceContext1 context, Viewport viewport);
    void Render(DeviceContext1 context, Viewport viewport, Color background);
    //void SetRenderTarget(DeviceContext1 context, Texture2D target);
}
