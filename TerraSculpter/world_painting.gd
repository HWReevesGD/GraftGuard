extends Node2D
class_name WorldPainting

const PROP_DISPLAY = preload("uid://cxun613vh6l53")

@onready var painter_panel: Panel = $"../Panel"
@onready var painter: Painter = $".."
@onready var world_picker: WorldPicker = $"../Picker"

var displays: Array[PropDisplay] = []

func _unhandled_input(event: InputEvent) -> void:
	if event.is_action_pressed("left_click") and world_picker.selected_prop != null:
		print(get_global_mouse_position())
		var display: PropDisplay = PROP_DISPLAY.instantiate()
		add_child(display)
		display.global_position = get_global_mouse_position()
		display.setup(world_picker.selected_prop)
