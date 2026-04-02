extends Node2D
class_name PropDisplay

@onready var sprite: Sprite2D = $Sprite

var prop: Prop

func setup(source_prop: Prop) -> void:
	prop = source_prop
	
	sprite.centered = false
	sprite.texture = AtlasTexture.new()
	sprite.texture.atlas = prop.texture
	sprite.texture.region = prop.cutout
	sprite.offset = -prop.sorting_origin

func is_mouse_over() -> bool:
	return Rect2(global_position + sprite.offset, sprite.texture.get_size()).has_point(get_global_mouse_position())
