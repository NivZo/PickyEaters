shader_type canvas_item;

uniform vec4 color1 : source_color = vec4(0.941, 0.71, 0.255, 1);
uniform vec4 color2 : source_color = vec4(1, 1, 1, 1);
uniform vec4 color3 : source_color = vec4(0.5, 0.5, 0.5, 1);
uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_linear_mipmap;
// etc.

float get_gray(vec4 col){
	return max(col.r, max(col.g, col.b));
}

void fragment(){
	vec4 color = texture(SCREEN_TEXTURE, SCREEN_UV);
	vec4 new_color = vec4(.0);


	if (abs(get_gray(color1) - get_gray(color)) < abs(get_gray(new_color) - get_gray(color))) {
		new_color = color1;
	}
	if (abs(get_gray(color2) - get_gray(color)) < abs(get_gray(new_color) - get_gray(color))) {
		new_color = color2;
	}
	if (abs(get_gray(color3) - get_gray(color)) < abs(get_gray(new_color) - get_gray(color))) {
		new_color = color3;
	}
	// etc.


	COLOR = new_color;
}