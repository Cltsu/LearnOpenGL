# version 330 core

out vec4 FragColor;
in vec3 localPos;

uniform samplerCube environmentMap;

const float PI = 3.14159265359;



void main()
{
	vec3 normal = normalize(localPos);
	vec3 irradiance = vec3(0.0);
	//
	vec3 up = vec3(0.0, 1.0, 0.0);
	vec3 right =  normalize(cross(up, normal));
	up = normalize(cross(normal, right));
	// 采样出点积分
	float sampleDelta = 0.025;
	float nrSamples = 0.0;
	for(float phi = 0.0; phi < 2 * PI; phi += sampleDelta)
	{
		for(float theta = 0.0; theta < 0.5 * PI; theta += sampleDelta)
		{
			vec3 tangentSpace = vec3(sin(theta) * cos(phi),sin(theta) * sin(phi), cos(theta));
			vec3 sampleVec = right * tangentSpace.x + up * tangentSpace.y + normal * tangentSpace.z;
			irradiance += texture(environmentMap, sampleVec).rgb *  sin(theta) * cos(theta);
			nrSamples++;
		}
	}
	// why product PI?
	irradiance = irradiance  * (1.0 / float(nrSamples)) * PI;
	// div PI would be reasonable
	//irradiance = irradiance  * (1.0 / float(nrSamples)) / PI;
	FragColor = vec4(irradiance, 1.0);
}