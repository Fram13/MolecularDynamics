#version 440 core

layout (location = 0) in vec3 vertex;

uniform mat4 modelView;

void main()
{
	gl_Position = modelView * vec4(vertex, 1.0f);
}