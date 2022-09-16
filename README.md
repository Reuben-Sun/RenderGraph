# RenderGraph

### 描述

基于Unity SRP RenderGraph的延迟渲染，目前还在开发中

#### GBuffer组成

|      | 格式   | 内容                                             |
| ---- | ------ | ------------------------------------------------ |
| MRT0 | RGBA32 | RGB: Albedo, A: shadow                           |
| MRT1 | RGBA32 | RGB: Emission + fog.xyz                          |
| MRT2 | RGBA32 | RGB: EncodeNormal.xyz, A: ShadingMode            |
| MRT3 | RGBA32 | R: Metallic, G: Roughness, B:Occlusion, A: fog.w |

