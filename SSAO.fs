#version 330 core
out float FragColor;
in vec2 TexCoords;

uniform sampler2D gPositionDepth;
uniform sampler2D gNormal;
uniform sampler2D noiseTexture;

uniform vec3 samples[64];
uniform mat4 projection;

const vec2 noiseScale = vec2(800.0/4.0, 600.0/4.0);

const float radius = 1.0f;
const float kernelSize = 64;
void main()
{             
	vec3 fragPos = texture(gPositionDepth, TexCoords).xyz;
	vec3 normal = texture(gNormal, TexCoords).rgb;
	vec3 randomVec = texture(noiseTexture, TexCoords * noiseScale).xyz;

	vec3 tangent = normalize(randomVec - normal * dot(randomVec, normal));
	vec3 bitangent = cross(normal, tangent);
	mat3 TBN = mat3(tangent, bitangent, normal);

	float occlusion = 0.0;
	for(int i = 0; i < kernelSize; ++i)
	{
		// 获取样本位置
		vec3 sample = TBN * samples[i]; // 切线->观察空间
		sample = fragPos + sample * radius; 
		// 计算sample中有多少被遮蔽，即点的深度大于z-buffer，而zbuffer被存储在gPosition的第四个分量中。
		
		vec4 offset = vec4(sample, 1.0f);
		offset = projection * offset;
		offset.xyz /= offset.w;
		offset.xyz = offset.xyz * 0.5 + 0.5;

		float sampleDepth = -texture(gPositionDepth, offset.xy).w;
		//occlusion += (sampleDepth >= sample.z ? 1.0 : 0.0);
		float rangeCheck = smoothstep(0.0, 1.0, radius / abs(fragPos.z - sampleDepth));
		occlusion += (sampleDepth >= sample.z ? 1.0 : 0.0) * rangeCheck;    
	}

	occlusion = 1.0 - (occlusion / kernelSize);
	FragColor = occlusion;
}