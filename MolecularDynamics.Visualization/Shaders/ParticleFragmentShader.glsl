#version 440 core

in vec3 normal;
out vec4 color;

void main()
{
    float intensity = min(max(-normal.z + 0.15f, 0.0f), 1.0f);
    color = vec4(intensity, intensity, intensity, 1.0f);
}