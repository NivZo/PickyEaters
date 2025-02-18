//shader_type canvas_item;
//
//uniform int amount = 40;
//
//void fragment()
//{
	//vec2 grid_uv = round(UV * float(amount)) / float(amount);
	//
	//vec4 text = texture(TEXTURE, grid_uv);
	//
	//COLOR = text;
//}

shader_type canvas_item;
render_mode unshaded;

uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, repeat_disable, filter_nearest;
uniform float PIXELATION_STRENGTH : hint_range(0.05, 1.0, 0.05) = 0.1;
uniform float RESOLUTION_WIDTH : hint_range(144.0, 1920.0, 48.0) = 480.0;

void fragment()
{
	float pixel_size = 1.0/(SCREEN_PIXEL_SIZE.x * RESOLUTION_WIDTH * PIXELATION_STRENGTH);
	float x = FRAGCOORD.x - mod(FRAGCOORD.x, pixel_size);
	float y = FRAGCOORD.y - mod(FRAGCOORD.y, pixel_size);
	float shift = floor(pixel_size / 2.0);
	COLOR = texture(SCREEN_TEXTURE, vec2(x + shift, y + shift) * SCREEN_PIXEL_SIZE);
}