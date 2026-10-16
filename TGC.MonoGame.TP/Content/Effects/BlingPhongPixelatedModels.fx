#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// Custom Effects - https://docs.monogame.net/articles/content/custom_effects.html
// High-level shader language (HLSL) - https://docs.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl
// Programming guide for HLSL - https://docs.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-pguide
// Reference for HLSL - https://docs.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-reference
// HLSL Semantics - https://docs.microsoft.com/en-us/windows/win32/direct3dhlsl/dx-graphics-hlsl-semantics

float4x4 World;
float4x4 View;
float4x4 Projection;
float4x4 InverseTransposeWorld;

float3 DiffuseColor;

float3 ambientColor; // Light's Ambient Color
float3 diffuseColor; // Light's Diffuse Color
float3 specularColor; // Light's Specular Color
float KAmbient;
float KDiffuse;
float KSpecular;
float shininess;
float3 lightPosition;
float3 eyePosition; // Camera position

float Time = 0;

// Debug: when > 0.5 the pixel shader outputs the normals as color
float DebugNormals = 0;

// Attenuation coefficients (tuned for the world scale). Lower = larger light radius.
float LightLinearAttenuation = 0.0007;
float LightQuadraticAttenuation = 0.0000018;

texture baseTexture;
sampler2D textureSampler = sampler_state
{
    Texture = (baseTexture);
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Wrap;
    AddressV = Wrap;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Normal : NORMAL;
    float2 TextureCoordinates : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION;
    float2 TextureCoordinates : TEXCOORD0;
    float4 WorldPosition : TEXCOORD1;
    float4 Normal : TEXCOORD2;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    // Clear the output
    VertexShaderOutput output = (VertexShaderOutput) 0;
    // Model space to World space
    float4 worldPosition = mul(input.Position, World);
    // View space to Projection space
    float4x4 WorldView = mul(World, View);
    float4x4 WorldViewProjection = mul(WorldView, Projection);
    output.Position = mul(input.Position, WorldViewProjection);
    output.Normal = normalize(mul(input.Normal, InverseTransposeWorld));
    output.WorldPosition = worldPosition;
    output.TextureCoordinates = input.TextureCoordinates;

    return output;
}

// Converts a normal (-1..1) into a visible color (0..1) for debugging
float4 NormalToColor(float3 normal)
{
    return float4(normal * 0.5 + 0.5, 1.0);
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float3 normal = normalize(input.Normal.xyz);

    // Debug mode: visualize the normals directly
    if (DebugNormals > 0.5)
    {
        // Red = broken / missing normal
        /*if (length(input.Normal.xyz) < 0.1)
            return float4(1.0, 0.0, 0.0, 1.0);*/

        // X -> Red, Y -> Green, Z -> Blue
        return NormalToColor(normal);
    }

    // Base vectors
    float3 lightDirection = normalize(lightPosition - input.WorldPosition.xyz);
    float3 viewDirection = normalize(eyePosition - input.WorldPosition.xyz);
    float3 halfVector = normalize(lightDirection + viewDirection);

    //Ambient Light
    float3 ambientLight = KAmbient * ambientColor;
    float pixelation = 2.75;
    float textureSize = 512.0;
    // Round to the nearest multiple of pixelation to create blocks
    float2 newUV;
    newUV.x = round(input.TextureCoordinates.x * textureSize / pixelation) * pixelation;
    newUV.y = round(input.TextureCoordinates.y * textureSize / pixelation) * pixelation;

    newUV.x /= textureSize;
    newUV.y /= textureSize;

    // Get the texture texel
    float4 texelColor = tex2D(textureSampler, newUV);

    // Diffuse Light
    float distance = length(lightPosition - input.WorldPosition.xyz);
    // Attenuation: parametrized linear and quadratic coefficients
    float attenuation = 1.0 / (1.0 + LightLinearAttenuation * distance + LightQuadraticAttenuation * distance * distance);

    float NdotL = dot(normal, lightDirection);
    float intensity = saturate(NdotL);
    float3 diffuseLight = KDiffuse * diffuseColor * intensity * attenuation;

    // Calculate the specular light
    float NdotH = saturate(dot(normal, halfVector));
    float3 specularLight = KSpecular * specularColor * pow(NdotH, shininess) * attenuation;

    float3 finalColor = saturate(ambientLight + diffuseLight) * texelColor.rgb + specularLight;
    return float4(finalColor, texelColor.a);
}

technique BasicColorDrawing
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};
