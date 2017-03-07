using System;
using System.Collections.Generic;
using System.Text;

namespace sage.vp6
{
    public enum Format
    {
        VP60 = 6,
        VP61 = 7,
        VP62 = 8,
    }

    public enum Profile
    {
        SIMPLE      = 0,
        ADVANCED    = 3
    }

    public enum ScalingMode
    {
        MAINTAIN_ASPECT_RATIO   = 0,
        SCALE_TO_FIT            = 1,
        CENTER                  = 2,
        OTHER                   = 4
    }
}
