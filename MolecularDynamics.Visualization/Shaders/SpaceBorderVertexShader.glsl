#version 440 core

layout (location = 0) in vec3 vertex;

uniform mat4 viewModel;

void main()
{
	gl_Position = viewModel * vec4(vertex, 1.0f);
}