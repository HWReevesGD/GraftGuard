class_name Tile

const SIZE: int = 16

var texture: AtlasTexture
var texture_name: String
var texture_cutout: Rect2i
var solid: bool

func _init(atlas: Texture2D, atlas_name: String, atlas_cutout: Rect2i, is_solid: bool) -> void:
	texture = AtlasTexture.new()
	texture.atlas = atlas
	texture.region = atlas_cutout
	texture_name = atlas_name
	solid = is_solid
