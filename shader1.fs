# version 330 core

out vec4 FragColor;

in VS_OUT{
	vec3 FragPos;
	vec3 Normal;
	vec2 TexCoords;
} fs_in;

uniform sampler2D diffuseTexture;
uniform samplerCube shadowMap;

uniform vec3 lightPos;
uniform vec3 viewPos;

uniform float far_plane;

float ShadowCalculation(vec3 fragPos){
    vec3 dir = normalize(fragPos - lightPos);
    float mapDepth = texture(shadowMap, dir).r;
    mapDepth *= far_plane;
    float curDepth = length(fragPos - lightPos);
    float bias = 0.05f;
    float shadow = mapDepth > curDepth - bias ? 0.0f : 1.0f;
    return shadow;
}

void main(){
	vec3 color = texture(diffuseTexture, fs_in.TexCoords).rgb;
	vec3 normal = normalize(fs_in.Normal);
	vec3 lightColor = vec3(0.7);

	// ambient
    vec3 ambient = 0.3 * lightColor;
    // diffuse
    vec3 lightDir = normalize(lightPos - fs_in.FragPos);
    float diff = max(dot(lightDir, normal), 0.0);
    vec3 diffuse = diff * lightColor;
    // specular
    vec3 viewDir = normalize(viewPos - fs_in.FragPos);
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = 0.0;
    vec3 halfwayDir = normalize(lightDir + viewDir);  
    spec = pow(max(dot(normal, halfwayDir), 0.0), 64.0);
    vec3 specular = spec * lightColor;    
    // calculate shadow
    float shadow = ShadowCalculation(fs_in.FragPos);       
    vec3 lighting = (ambient + (1.0 - shadow) * (diffuse + specular)) * color;    
    
    FragColor = vec4(lighting, 1.0);
    //vec3 dir = normalize(fs_in.FragPos - lightPos);
    //float mapDepth = texture(shadowMap, dir).r;
    //FragColor = vec4(vec3(mapDepth), 1.0);
}