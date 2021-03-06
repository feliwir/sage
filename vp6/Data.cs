﻿using System;
using System.Collections.Generic;
using System.Text;

namespace sage.vp6
{
    class Data
    {
        internal static readonly byte[] B6To4 = new byte[6] { 0, 0, 1, 1, 2, 3 };

        internal static readonly byte[] B2p = new byte[]{ 0, 0, 0, 0, 1, 2, 3, 3, 3, 3 };

        internal static readonly byte[] NormShift = new byte[256] {
            8,7,6,6,5,5,5,5,4,4,4,4,4,4,4,4,
            3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,
            2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,
            2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,
            1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
            1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
            1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
            1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
        };      

        internal static readonly byte[] CoeffReorderPct = new byte[64] {
            255, 132, 132, 159, 153, 151, 161, 170,
            164, 162, 136, 110, 103, 114, 129, 118,
            124, 125, 132, 136, 114, 110, 142, 135,
            134, 123, 143, 126, 153, 183, 166, 161,
            171, 180, 179, 164, 203, 218, 225, 217,
            215, 206, 203, 217, 229, 241, 248, 243,
            253, 255, 253, 255, 255, 255, 255, 255,
            255, 255, 255, 255, 255, 255, 255, 255,
        };

        internal static readonly byte[,] RunvPct = new byte[2, 14] {
          { 219, 246, 238, 249, 232, 239, 249, 255, 248, 253, 239, 244, 241, 248 },
          { 198, 232, 251, 253, 219, 241, 253, 255, 248, 249, 244, 238, 251, 255 },
        };

        //Arithmetic decoding
        internal static readonly byte[,,,] RactPct = new byte[3, 2, 6, 11] {
          { { { 227, 246, 230, 247, 244, 255, 255, 255, 255, 255, 255 },
              { 255, 255, 209, 231, 231, 249, 249, 253, 255, 255, 255 },
              { 255, 255, 225, 242, 241, 251, 253, 255, 255, 255, 255 },
              { 255, 255, 241, 253, 252, 255, 255, 255, 255, 255, 255 },
              { 255, 255, 248, 255, 255, 255, 255, 255, 255, 255, 255 },
              { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 } },
            { { 240, 255, 248, 255, 255, 255, 255, 255, 255, 255, 255 },
              { 255, 255, 240, 253, 255, 255, 255, 255, 255, 255, 255 },
              { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 },
              { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 },
              { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 },
              { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 } } },
          { { { 206, 203, 227, 239, 247, 255, 253, 255, 255, 255, 255 },
              { 207, 199, 220, 236, 243, 252, 252, 255, 255, 255, 255 },
              { 212, 219, 230, 243, 244, 253, 252, 255, 255, 255, 255 },
              { 236, 237, 247, 252, 253, 255, 255, 255, 255, 255, 255 },
              { 240, 240, 248, 255, 255, 255, 255, 255, 255, 255, 255 },
              { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 } },
            { { 230, 233, 249, 255, 255, 255, 255, 255, 255, 255, 255 },
              { 238, 238, 250, 255, 255, 255, 255, 255, 255, 255, 255 },
              { 248, 251, 255, 255, 255, 255, 255, 255, 255, 255, 255 },
              { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 },
              { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 },
              { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 } } },
          { { { 225, 239, 227, 231, 244, 253, 243, 255, 255, 253, 255 },
              { 232, 234, 224, 228, 242, 249, 242, 252, 251, 251, 255 },
              { 235, 249, 238, 240, 251, 255, 249, 255, 253, 253, 255 },
              { 249, 253, 251, 250, 255, 255, 255, 255, 255, 255, 255 },
              { 251, 250, 249, 255, 255, 255, 255, 255, 255, 255, 255 },
              { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 } },
            { { 243, 244, 250, 250, 255, 255, 255, 255, 255, 255, 255 },
              { 249, 248, 250, 253, 255, 255, 255, 255, 255, 255, 255 },
              { 253, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 },
              { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 },
              { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 },
              { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 } } }
        };

        //Discrete Cosinus decoding
        internal static readonly byte[,] DccvPct = new byte[2, 11] {
            { 146, 255, 181, 207, 232, 243, 238, 251, 244, 250, 249 },
            { 179, 255, 214, 240, 250, 255, 244, 255, 255, 255, 255 },
        };

        //equations for converting Dccv to Dcct
        internal static readonly int[,,] DccvLc = new int[3,5,2] {
          { { 122, 133 }, { 0, 1 }, { 78,  171 }, { 139, 117 }, { 168, 79 } },
          { { 133,  51 }, { 0, 1 }, { 169,  71 }, { 214,  44 }, { 210, 38 } },
          { { 142, -16 }, { 0, 1 }, { 221, -30 }, { 246,  -3 }, { 203, 17 } },
        };

        //candidate predictor positions
        internal static readonly sbyte[,] CandidatePredictorPos = new sbyte[12,2] {
            {  0, -1 },
            { -1,  0 },
            { -1, -1 },
            {  1, -1 },
            {  0, -2 },
            { -2,  0 },
            { -2, -1 },
            { -1, -2 },
            {  1, -2 },
            {  2, -1 },
            { -2, -2 },
            {  2, -2 },
        };

        internal static readonly int[] ReferenceFrame = new int[10] {
            FrameSelect.PREVIOUS,  /* VP56_MB_INTER_NOVEC_PF */
            FrameSelect.CURRENT,   /* VP56_MB_INTRA */
            FrameSelect.PREVIOUS,  /* VP56_MB_INTER_DELTA_PF */
            FrameSelect.PREVIOUS,  /* VP56_MB_INTER_V1_PF */
            FrameSelect.PREVIOUS,  /* VP56_MB_INTER_V2_PF */
            FrameSelect.GOLDEN,    /* VP56_MB_INTER_NOVEC_GF */
            FrameSelect.GOLDEN,    /* VP56_MB_INTER_DELTA_GF */
            FrameSelect.PREVIOUS,  /* VP56_MB_INTER_4V */
            FrameSelect.GOLDEN,    /* VP56_MB_INTER_V1_GF */
            FrameSelect.GOLDEN,    /* VP56_MB_INTER_V2_GF */
        };

        internal static readonly Tree[] PvaTree = new Tree[]{
            new Tree( 8, 0),
            new Tree( 4, 1),
            new Tree( 2, 2), new Tree(-0), new Tree(-1),
            new Tree( 2, 3), new Tree(-2), new Tree(-3),
            new Tree( 4, 4),
            new Tree( 2, 5), new Tree(-4), new Tree(-5),
            new Tree( 2, 6), new Tree(-6), new Tree(-7),
        };

        internal static readonly Tree[] PcTree = new Tree[]{
            new Tree( 4, 6),
            new Tree( 2, 7), new Tree(-0), new Tree(-1),
            new Tree( 4, 8),
            new Tree( 2, 9), new Tree(-2), new Tree(-3),
            new Tree( 2,10), new Tree(-4), new Tree(-5),
        };

        internal static readonly Tree[] PcrTree =  new Tree[] {
            new Tree( 8, 0),
            new Tree( 4, 1),
            new Tree( 2, 2), new Tree(-1), new Tree(-2),
            new Tree( 2, 3), new Tree(-3), new Tree(-4),
            new Tree( 8, 4),
            new Tree( 4, 5),
            new Tree( 2, 6), new Tree(-5), new Tree(-6),
            new Tree( 2, 7), new Tree(-7), new Tree(-8),
                     new Tree(-0),
        };

        internal static readonly byte[] CoeffBias = new byte[] {
            0, 1, 2, 3, 4, 5, 7, 11, 19, 35, 67 };

        internal static readonly byte[] CoeffBitLength = new byte[] { 0, 1, 2, 3, 4, 10 };

        internal static readonly byte[,] CoeffParseTable = new byte[6,11] {
            { 159,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0 },
            { 145, 165,   0,   0,   0,   0,   0,   0,   0,   0,   0 },
            { 140, 148, 173,   0,   0,   0,   0,   0,   0,   0,   0 },
            { 135, 140, 155, 176,   0,   0,   0,   0,   0,   0,   0 },
            { 130, 134, 141, 157, 180,   0,   0,   0,   0,   0,   0 },
            { 129, 130, 133, 140, 153, 177, 196, 230, 243, 254, 254 },
        };

        internal static readonly byte[] ZigZag = new byte[64] {
            0,   1,  8, 16,  9,  2,  3, 10,
            17, 24, 32, 25, 18, 11,  4,  5,
            12, 19, 26, 33, 40, 48, 41, 34,
            27, 20, 13,  6,  7, 14, 21, 28,
            35, 42, 49, 56, 57, 50, 43, 36,
            29, 22, 15, 23, 30, 37, 44, 51,
            58, 59, 52, 45, 38, 31, 39, 46,
            53, 60, 61, 54, 47, 55, 62, 63
        };

        internal static readonly byte[] Scantable = Util.Transpose(ZigZag);

        internal static readonly byte[] CoeffGroups = new byte[64] {
            0, 0, 1, 1, 1, 2, 2, 2,
            2, 2, 2, 3, 3, 3, 3, 3,
            3, 3, 3, 3, 3, 3, 4, 4,
            4, 4, 4, 4, 4, 4, 4, 4,
            4, 4, 4, 4, 4, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5,
            5, 5, 5, 5, 5, 5, 5, 5,
        };

        internal static readonly Tree[] PmbtTree = new Tree[] {
            new Tree( 8, 1),
            new Tree( 4, 2),
            new Tree( 2, 4), new Tree(CodingMode.INTER_MV,true), new Tree(CodingMode.INTER_PLUS_MV,true),
            new Tree( 2, 5), new Tree(CodingMode.INTER_NEAREST_MV,true), new Tree(CodingMode.INTER_NEAR_MV,true),
            new Tree( 4, 3),
            new Tree( 2, 6), new Tree(CodingMode.INTRA,true),new Tree(CodingMode.INTER_FOURMV,true),
            new Tree( 4, 7),
            new Tree( 2, 8), new Tree(CodingMode.USING_GOLDEN,true), new Tree(CodingMode.GOLDEN_MV,true),
            new Tree( 2, 9), new Tree(CodingMode.GOLD_NEAREST_MV,true), new Tree(CodingMode.GOLD_NEAR_MV,true),
        };

        internal static readonly byte[] HuffCoeffMap = new byte[] {
            13, 14, 11, 0, 1, 15, 16, 18, 2, 17, 3, 4, 19, 20, 5, 6, 21, 22, 7, 8, 9, 10
        };

        internal static readonly byte[] HuffRunMap = new byte[] {
            10, 13, 11, 12, 0, 1, 2, 3, 14, 8, 15, 16, 4, 5, 6, 7
        };

    }
}
