// Day/night sky: colour gradient, a glowing sun and stars.
// Main.cs feeds the colours, directions and visibilities every frame; the rest is tunable in the Inspector.
shader_type sky;

group_uniforms Sky;
uniform vec3 sky_top_color : source_color = vec3(0.16, 0.45, 0.95);
uniform vec3 sky_horizon_color : source_color = vec3(0.58, 0.8, 1.0);
uniform float sky_curve : hint_range(0.01, 1.0) = 0.15;
uniform vec3 ground_horizon_color : source_color = vec3(0.65, 0.67, 0.7);
uniform vec3 ground_bottom_color : source_color = vec3(0.2, 0.17, 0.13);

group_uniforms Sun;
uniform vec3 sun_direction = vec3(0.0, 1.0, 0.0); // points from the scene towards the sun
uniform vec3 sun_color : source_color = vec3(1.0);
uniform float sun_visibility : hint_range(0.0, 1.0) = 1.0;
uniform float sun_size_degrees : hint_range(0.1, 20.0) = 2.0;
uniform float sun_disk_energy : hint_range(0.0, 16.0) = 4.0;
uniform float sun_halo_size_degrees : hint_range(0.1, 90.0) = 10.0;
uniform float sun_halo_strength : hint_range(0.0, 4.0) = 0.6;

group_uniforms Stars;
uniform float star_visibility : hint_range(0.0, 1.0) = 0.0;
uniform float star_density : hint_range(0.0, 0.05) = 0.006;
uniform float star_grid : hint_range(50.0, 1000.0) = 300.0; // higher = smaller stars
uniform float star_brightness : hint_range(0.0, 4.0) = 1.2;

float hash(vec3 p) {
	p = fract(p * 0.3183099 + 0.1);
	p *= 17.0;
	return fract(p.x * p.y * p.z * (p.x + p.y + p.z));
}

// Fades things out at the horizon so the sun and stars never draw over the ground.
float above_horizon(vec3 dir) {
	return smoothstep(-0.01, 0.02, dir.y);
}

void sky() {
	vec3 dir = EYEDIR;

	// Gradient: horizon to top above, horizon to bottom below.
	vec3 color;
	if (dir.y >= 0.0) {
		float t = 1.0 - pow(1.0 - clamp(dir.y, 0.0, 1.0), 1.0 / sky_curve);
		color = mix(sky_horizon_color, sky_top_color, t);
	} else {
		float t = clamp(-dir.y * 4.0, 0.0, 1.0);
		color = mix(ground_horizon_color, ground_bottom_color, t);
	}

	// Stars: the sky is cut into tiny cells, a few random cells light up. Skipped for the lighting pass.
	if (!AT_CUBEMAP_PASS && star_visibility > 0.0) {
		vec3 cell = floor(dir * star_grid);
		float h = hash(cell);
		if (h > 1.0 - star_density) {
			float star_light = 0.4 + 0.6 * hash(cell + 7.0);
			color += vec3(star_light * star_brightness * star_visibility * above_horizon(dir));
		}
	}

	// Sun: a bright disk with a soft halo around it.
	vec3 sun_dir = normalize(sun_direction);
	float sun_angle = acos(clamp(dot(dir, sun_dir), -1.0, 1.0));
	float sun_radius = radians(sun_size_degrees) * 0.5;
	float disk = 1.0 - smoothstep(sun_radius * 0.85, sun_radius, sun_angle);
	float halo = exp(-sun_angle / radians(sun_halo_size_degrees)) * sun_halo_strength;
	color += sun_color * (disk * sun_disk_energy + halo) * sun_visibility * above_horizon(dir);

	COLOR = color;
}
