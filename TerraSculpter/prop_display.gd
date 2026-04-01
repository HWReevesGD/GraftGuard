extends Node2D
class_name PropDisplay

@onready var sprite: Sprite2D = $Sprite

var prop: Prop

func setup(source_prop: Prop) -> void:
	prop = source_prop
	
	sprite.centered = false
	sprite.texture = AtlasTexture.new()
	sprite.texture.atlas = prop.Texture
	sprite.texture.region = prop.Cutout
	sprite.offset = -prop.SortingOrigin
