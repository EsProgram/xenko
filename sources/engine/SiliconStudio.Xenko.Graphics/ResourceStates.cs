// Copyright (c) 2011-2017 Silicon Studio Corp. All rights reserved. (https://www.siliconstudio.co.jp)
// See LICENSE.md for full license information.
using System;

namespace SiliconStudio.Xenko.Graphics
{
    [Flags]
    public enum GraphicsResourceState
    {
        Common = 0,
        Present = 0,
        VertexAndConstantBuffer = 1,
        IndexBuffer = 2,
        RenderTarget = 4,
        UnorderedAccess = 8,
        DepthWrite = 16,
        DepthRead = 32,
        NonPixelShaderResource = 64,
        PixelShaderResource = 128,
        StreamOut = 256,
        IndirectArgument = 512,
        Predication = 512,
        CopyDestination = 1024,
        CopySource = 2048,
        GenericRead = 2755,
        ResolveDestination = 4096,
        ResolveSource = 8192,
    }
}
