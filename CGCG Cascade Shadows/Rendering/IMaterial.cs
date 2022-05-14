using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FR.CascadeShadows.Rendering;

public interface IMaterial
{
    TransitionStep ProgramStep { get; }
    TransitionStep MaterialStep { get; }
}

public interface IDepthMaterial
{
    TransitionStep DepthStep { get; }
}
