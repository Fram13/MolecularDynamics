#version 440 core

in vec3 normal;
in vec3 color;

out vec4 outColor;

void main()
{
    float intensity = min(max(-normal.z + 0.15f, 0.0f), 1.0f);
    outColor = vec4(intensity * color.x, intensity * color.y, intensity * color.z, 1.0f);
}