extends Node2D
class_name WorldPainting

const pan_speed: float = 512.0
const PROP_DISPLAY = preload("uid://cxun613vh6l53")

@onready var painter_panel: Panel = $"../Panel"
@onready var painter: Painter = $".."
@onready var world_picker: WorldPicker = $"../Picker"
@onready var map: Map = $Map
@onready var zoom_slider: HSlider = $"../Picker/Margin/Vertical/Zoom/MarginContainer/SliderBox/ZoomSlider"

var pan_position: Vector2
var displays: Array[PropDisplay] = []
var last_mouse_tile: Vector2i

var held_inside: bool = false
var mouse_inside: bool:
	get(): return painter.world_rect.has_point(get_global_mouse_position())

enum Mode {
	Draw,
	Fill,
	Erase,
	None,
}

func _unhandled_input(event: InputEvent) -> void:
	if event.is_action("left_click"):
		held_inside = true

func update_prop_data() -> void:
	var to_remove: Array[PropDisplay] = []
	
	for display in displays:
		var index: int = Registry.props.find_custom(func(p: Prop): return p.prop_name == display.prop.prop_name)
		if index == -1:
			to_remove.append(display)
			continue
		var prop: Prop = Registry.props[index]
		display.setup(prop)
	
	for display in to_remove:
		displays.erase(display)
		display.queue_free()

func _input(event: InputEvent) -> void:
	if event.is_action_pressed("zoom_in"):
		zoom_slider.value += 0.2
	if event.is_action_pressed("zoom_out"):
		zoom_slider.value -= 0.2

func _process(delta: float) -> void:
	if not Input.is_action_pressed("left_click"):
		held_inside = false
	queue_redraw()
	var input: Vector2 = Input.get_vector("pan_left", "pan_right", "pan_up", "pan_down")
	pan_position -= input * delta * pan_speed
	
	scale = Vector2.ONE * zoom_slider.value
	global_position = pan_position * zoom_slider.value + get_viewport_rect().size * 0.5
	
	var mouse_tile: Vector2i = map.global_to_tile(get_global_mouse_position())
	
	var tile: Tile = world_picker.selected_tile
	var prop: Prop = world_picker.selected_prop
	var clicked_inside: bool = Input.is_action_just_pressed("left_click") and mouse_inside
	
	if tile != null and held_inside:
		match get_tile_mode():
			Mode.Draw:
				if clicked_inside:
					map.set_at_mouse(tile)
				elif Input.is_action_pressed("left_click"):
					map.set_line(last_mouse_tile, mouse_tile, tile)
			Mode.Erase:
				if clicked_inside:
					map.set_at_mouse(null)
				elif Input.is_action_pressed("left_click"):
					map.set_line(last_mouse_tile, mouse_tile, null)
			Mode.Fill:
				if clicked_inside:
					map.fill_at_mouse(tile)
	
	if prop != null and held_inside:
		if clicked_inside and Input.is_action_just_pressed("left_click") and mouse_inside:
			match get_prop_mode():
				Mode.Draw:
					var display: PropDisplay = PROP_DISPLAY.instantiate()
					add_child(display)
					display.global_position = get_global_mouse_position()
					display.setup(world_picker.selected_prop)
					displays.append(display)
				Mode.Erase:
					var found: PropDisplay = null
					for display: PropDisplay in displays:
						if display.is_mouse_over():
							found = display
							break
					if found != null:
						displays.erase(found)
						found.queue_free()
							

	last_mouse_tile = mouse_tile

func get_tile_mode() -> Mode:
	var current: String = world_picker.draw_mode.get_tab_title(world_picker.draw_mode.current_tab)
	match current:
		"Draw": return Mode.Draw
		"Fill": return Mode.Fill
		"Erase": return Mode.Erase
	return Mode.None

func get_prop_mode() -> Mode:
	var current: String = world_picker.prop_mode.get_tab_title(world_picker.prop_mode.current_tab)
	match current:
		"Place": return Mode.Draw
		"Erase": return Mode.Erase
	return Mode.None
