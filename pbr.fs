#version 330 core

out vec4 FragColor;

in vec2 TexCoords;
in vec3 WorldPos;
in vec3 Normal;

uniform vec3 camPos;

uniform vec3  albedo;
uniform float metallic;
uniform float roughness;
uniform float ao;

// lights
uniform vec3 lightPositions[4];
uniform vec3 lightColors[4];

const float PI = 3.14159265359;

vec3 fresnelSchlick(float cosTheta, vec3 F0)
{
	return F0 + (1 - F0) * pow((1 - cosTheta), 5);
}

float DistributionGGX(vec3 N, vec3 H, float roughness)
{
	float cos = max(0.0, dot(N, H));
	float cos2 = cos * cos;
	cos2 = cos2 * cos2;
	float a2 = roughness * roughness;
	float frac = cos2 * (a2 - 1) + 1;
	float frac2 = frac * frac;

	return a2 / (PI * frac2);
}

float GeometrySchlickGGX(float NdotV, float roughness)
{
	float k = pow((roughness + 1), 2) / 8.0;

	float nom = NdotV;
	float denom = NdotV * (1 - k) + k;
	return nom / denom;
}

float GeometrySmith(vec3 N, vec3 V, vec3 L, float roughness)
{
	float NdotV = max(dot(N, V), 0.0);
	float NdotL = max(dot(N, L), 0.0);

	return GeometrySchlickGGX(NdotV, roughness) * GeometrySchlickGGX(NdotL, roughness);
}

void main()
{    
	vec3 N = normalize(Normal);
	vec3 V = normalize(camPos - WorldPos);
	vec3 F0 = vec3(0.04);
	F0 = mix(F0, albedo, metallic);
	vec3 Lo = vec3(0.0f);
	for(int i = 0; i < 4; i++)
	{
		vec3 L = normalize(lightPositions[i] - WorldPos);
		vec3 H = normalize(L + V);
		float distance    = length(lightPositions[i] - WorldPos);
        float attenuation = 1.0 / (distance * distance);
        vec3 radiance     = lightColors[i] * attenuation;   

		//F
		vec3 F = fresnelSchlick(max(dot(H, V), 0.0), F0);
		//D and G
		float NDF = DistributionGGX(N, H, roughness);
		float G = GeometrySmith(N, V, L, roughness);
		// cook-torrance
		vec3 nom = NDF * G * F;
		float denom = 4 * max(dot(V, N), 0.0) * max(dot(L, N), 0.0) + 0.001;
		vec3 specular = nom / denom;
		// ks and kd
		vec3 kS = F;
		vec3 kD = vec3(1.0) - kS;
		kD *= 1.0 - metallic;
		//
		float NdotL = max(dot(N, L), 0.0);
		// no ks, because ks is F
		Lo += (kD * albedo / PI + specular) * radiance * NdotL;
	}

	vec3 ambient = vec3(0.03) * albedo * ao;
	vec3 color = ambient + Lo;
	//gramma
	color = color / (color + vec3(1.0));
	color = pow(color, vec3(1.0/2.2)); 

	FragColor = vec4(color, 1.0);
}