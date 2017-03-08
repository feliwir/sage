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

    public enum CodingMode
    {
        //CODING MODE                   PREDICTION FRAME    MOTIONVECTOR
        INTER_MV           = 0,//  PREVIOUS            FIXED (0,0)
        INTRA              = 1,//  NONE                NONE
        INTER_PLUS_MV      = 2,//  PREVIOUS            NEWLY CALCULATED
        INTER_NEAREST_MV   = 3,//  PREVIOUS            SAME MV AS NEAREST
        INTER_NEAR_MV      = 4,//  PREVIOUS            SAME MV AS NEAR
        USING_GOLDEN       = 5,//  GOLDEN              FIXED (0,0)
        GOLDEN_MV          = 6,//  GOLDEN              NEWLY CALCULATED
        INTER_FOURMV       = 7,//  PREVIOUS            EACH LUMABLOCK HAS ONE MV
        GOLD_NEAREST_MV    = 8,//  GOLDEN              SAME MV AS NEAREST
        GOLD_NEAR_MV       = 9,//  GOLDEN              SAME MV AS NEAR
    }
}
