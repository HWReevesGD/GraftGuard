extends Node
class_name Prop

var prop_name: String
var texture_name: String
var texture: Texture2D
var cutout: Rect2i
var sorting_origin: Vector2i
var use_collision: bool
var collision: Rect2i

func _init(_name: String, _tex_name: String, _tex: Texture2D, _cutout: Rect2i, _sort_origin: Vector2i, _use_collision: bool, _collision: Rect2i) -> void:
	prop_name = _name
	texture_name = _tex_name
	texture = _tex
	cutout = _cutout
	sorting_origin = _sort_origin
	use_collision = _use_collision
	collision = _collision
