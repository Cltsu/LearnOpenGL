#version 330 core

out vec4 FragColor;

in VS_OUT{
	vec3 TangentViewPos;
	vec3 TangentLightPos;
	vec3 TangentFragPos;
	vec2 TexCoords;
}fs_in;

uniform sampler2D diffuseMap;
uniform sampler2D normalMap;


void main()
{
	vec3 normal = texture(normalMap, fs_in.TexCoords).rgb;
	vec3 color = texture(diffuseMap, fs_in.TexCoords).rgb;

    vec3 viewPos = fs_in.TangentViewPos;
    vec3 FragPos = fs_in.TangentFragPos;
    vec3 lightPos = fs_in.TangentLightPos;

	// ambient
    vec3 ambient = 0.3 * color;
    // diffuse
    vec3 lightDir = normalize(lightPos - FragPos);
    float diff = max(dot(lightDir, normal), 0.0);
    vec3 diffuse = diff * color;
    // specular
    vec3 viewDir = normalize(viewPos - FragPos);
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = 0.0;
    vec3 halfwayDir = normalize(lightDir + viewDir);  
    spec = pow(max(dot(normal, halfwayDir), 0.0), 64.0);
    vec3 specular = 0.1 * spec * color;    
    // calculate shadow
    vec3 lighting = ambient + diffuse + specular;    
    
    FragColor = vec4(lighting, 1.0);
}