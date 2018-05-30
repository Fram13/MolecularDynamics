#version 440 core

layout (location = 0) in vec3 vertex;
layout (location = 1) in vec3 position;
layout (location = 2) in vec3 inColor;

uniform mat4 viewModel;
uniform mat3 transposeInverseViewModel;

out vec3 normal;
out vec3 color;

void main()
{
    gl_Position = viewModel * vec4(vertex + position, 1.0f);
    normal = transposeInverseViewModel * vertex;
    color = inColor;
}