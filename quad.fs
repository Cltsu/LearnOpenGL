#version 330 core
out vec4 FragColor;
in vec2 TexCoords;

uniform sampler2D gPosition;


void main()
{             
   FragColor = vec4(vec3(texture(gPosition, TexCoords).r), 1.0f);
}