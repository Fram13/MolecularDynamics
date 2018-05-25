#version 440 core

layout (location = 0) in vec3 vertex;
layout (location = 1) in vec3 position;

uniform mat4 modelView;
uniform mat3 transposeInverseModelView;

out vec3 normal;

void main()
{
    gl_Position = modelView * vec4(vertex + position, 1.0f);
    normal = transposeInverseModelView * vertex;
}