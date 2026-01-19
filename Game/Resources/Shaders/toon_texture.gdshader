shader_type spatial;
render_mode blend_mix, depth_draw_opaque, cull_disabled, diffuse_toon, specular_toon;

// replace with a hint_albedo so Godot knows this is your color map
uniform sampler2D albedo_tex : source_color;
uniform sampler2D normal_tex : hint_normal;

uniform sampler2D color_gradient;

uniform vec3  rim_color    : source_color = vec3(0.0); // white by default
uniform float rim_power    = 4.0;   // higher = tighter rim
uniform float rim_strength = 0.8;   // overall intensity

// optional: if you still want a flat tint multiplier
uniform float tint : hint_range(0.0, 2.0) = 1.0;

void fragment() {
    // sample your texture
    vec3 albedoColor = texture(albedo_tex, UV).rgb * tint;

    // apply your fresnel‐tint on the textured color
    vec3 lightAlbedo  = albedoColor * 3.0;
    vec3 base_color   = albedoColor
                        * lightAlbedo;
	float rim  = pow(1.0 - clamp(dot(normalize(NORMAL), normalize(VIEW)), 0.0, 1.0),
                     rim_power);          // 0…1
    vec3 rim_col = rim * rim_strength * rim_color;

    ALBEDO = clamp(base_color + rim_col, 0.0, 1.0);
	NORMAL_MAP = texture(normal_tex, UV).rgb;
}

void light() {
    // Calculate base Lambertian term
    float lambertian = max(dot(NORMAL, LIGHT), 0.0);

    // Add an ambient term to ensure a minimum brightness
    float ambient_strength = 0.2; // Adjust this value (0.0 to 1.0) to control ambient light
    float lighting = lambertian + ambient_strength;

    // Quantize the lighting for cel-shading effect
    float levels = 2.0;
    lighting = floor(lighting * levels) / levels;

    // Adjust the sample range to avoid overly dark areas and account for ambient
    float sample = clamp(lighting * ATTENUATION, 0.2, 1.0); // Raised minimum from 0.05 to 0.2
    vec4 shaded = texture(color_gradient, vec2(sample, 0.0));
    DIFFUSE_LIGHT += clamp((shaded.rgb * LIGHT_COLOR), 0.0, 1.0);
}