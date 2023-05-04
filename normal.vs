#version 330 core
layout (location = 0) in vec3 position;
layout (location = 1) in vec3 normal;
layout (location = 2) in vec2 texCoords;
layout (location = 3) in vec3 tangent;
layout (location = 4) in vec3 bitangent;

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;

uniform vec3 lightPos;
uniform vec3 viewPos;

out VS_OUT{
	vec3 TangentViewPos;
	vec3 TangentLightPos;
	vec3 TangentFragPos;
	vec2 TexCoords;
}vs_out;

void main()
{
	gl_Position = projection * view * model * vec4(position, 1.0f);
	vs_out.TexCoords = texCoords;

	mat3 normalMat = transpose(inverse(mat3(model)));
	vec3 T = normalize(normalMat * tangent);
	vec3 B = normalize(normalMat * bitangent);
	vec3 N = normalize(normalMat * normal);

	mat3 TBN = transpose(mat3(T, B, N));
	vs_out.TangentViewPos = TBN * viewPos;
	vs_out.TangentLightPos = TBN * lightPos;
	vs_out.TangentFragPos = TBN * vec3(model * vec4(position, 1.0f));
}