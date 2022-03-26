
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FR.CascadeShadows.Rendering;
public class Renderer
{
    // Renderer
    //  - camera
    //  - scene
    //  + Render()
    //      clear ViewPort
    //      setup ViewPort
    //      setup camera properties
    //      render scene

    // scene
    //   - 3D scenegraph (for matrices)

    // pass
    //   - graph of state changes and draw calls
    //   - the overall algorithm
    //   - buffer setup used for rendering
    //   * e.g. SurfacePass -> LightPass -> ForwardPass -> GizmoPass

    public InnerNode ForwardPass = new(TransitionStateStep.Empty);
}
