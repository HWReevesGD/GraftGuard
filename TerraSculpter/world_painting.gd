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
var enemy_spawns: Array[Vector2i] = []

var held_inside: bool = false
var mouse_inside: bool:
	get(): return painter.world_rect.has_point(get_global_mouse_position())

enum Mode {
	Draw,
	Fill,
	Erase,
	Spawn,
	None,
}

func _draw() -> void:
	for spawn: Vector2i in enemy_spawns:
		draw_circle(spawn, 16.0, Color.RED, false, 2)

func update_picker() -> void:
	world_picker.update_props()
	world_picker.update_tiles()

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
	var right_clicked_inside: bool = Input.is_action_just_pressed("right_click") and mouse_inside
	
	if ((tile != null) or (get_tile_mode() == Mode.Spawn and prop == null)):
		match get_tile_mode():
			Mode.Draw:
				if held_inside:
					if clicked_inside:
						map.set_at_mouse(tile)
					elif Input.is_action_pressed("left_click"):
						map.set_line(last_mouse_tile, mouse_tile, tile)
			Mode.Erase:
				if held_inside:
					if clicked_inside:
						map.set_at_mouse(null)
					elif Input.is_action_pressed("left_click"):
						map.set_line(last_mouse_tile, mouse_tile, null)
			Mode.Fill:
				if clicked_inside:
					map.fill_at_mouse(tile)
				if right_clicked_inside:
					map.fill_at_mouse(null)
			Mode.Spawn:
				if clicked_inside:
					enemy_spawns.append(Vector2i(get_local_mouse_position()))
					queue_redraw()
				if right_clicked_inside:
					var index: int = 0
					while index < enemy_spawns.size():
						if enemy_spawns[index].distance_to(Vector2i(get_local_mouse_position())) < 16.0:
							enemy_spawns.remove_at(index)
							index -= 1
						index += 1
					queue_redraw()
	
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
		"Spawns": return Mode.Spawn
	return Mode.None

func get_prop_mode() -> Mode:
	var current: String = world_picker.prop_mode.get_tab_title(world_picker.prop_mode.current_tab)
	match current:
		"Place": return Mode.Draw
		"Erase": return Mode.Erase
	return Mode.None

func serialize_world() -> Dictionary:
	var props: Dictionary = serialize_placed_props()
	var tiles: Dictionary = serialize_tiles()
	var spawns: Array[Dictionary] = serialize_enemy_spawns()
	
	return {
		"props": props,
		"tiles": tiles,
		"spawns": spawns,
	}

func deserialize_world(serialized_world: Dictionary) -> void:
	# Deserialize Spawns
	var spawns: Array = serialized_world["spawns"]
	for spawn in spawns:
		enemy_spawns.append(Vector2i(Painter.vector_deserialize(spawn)))
	
	# Deserialize Props
	var props: Dictionary = serialized_world["props"]
	var prop_library: Dictionary =  props["library"]
	var placed_prop_ids: Array = props["placed_ids"]
	var placed_prop_positions: Array = props["placed_positions"]
	
	for index: int in range(placed_prop_ids.size()):
		var prop: Prop = Registry.from_name(prop_library[str(int(placed_prop_ids[index]))])
		var prop_position: Vector2 = Painter.vector_deserialize(placed_prop_positions[index])
		var display: PropDisplay = PROP_DISPLAY.instantiate()
		add_child(display)
		displays.append(display)
		display.setup(prop)
		display.position = prop_position
	
	# Deserialize Tiles
	var tiles: Dictionary = serialized_world["tiles"]
	var tile_serialized_library: Array = tiles["source"]
	var serialized_chunks: Array = tiles["chunks"]
	var tile_library: Dictionary[int, Tile] = {}
	
	for tile_definition: Dictionary in tile_serialized_library:
		var tile: Tile = Registry.tile_from_serialized(tile_definition)
		if tile == null:
			print("Unknown tile: " + str(tile_definition))
			continue
		tile_library[int(tile_definition["id"])] = tile
	
	for chunk: Dictionary in serialized_chunks:
		var coordinate: Vector2i = Painter.vector_deserialize(chunk["coordinate"])
		var chunk_tile_coordinate: Vector2i = _save_chunk_to_tile(coordinate)
		var tile_ids: Array = chunk["tiles"]
		var index: int = -1
		for id: int in tile_ids:
			index += 1
			var tile: Tile = tile_library.get(id, null)
			if tile == null:
				continue
			map.tiles[chunk_tile_coordinate + _tile_local_linear_to_local(index)] = tile

func serialize_enemy_spawns() -> Array[Dictionary]:
	var spawns: Array[Dictionary] = []
	for spawn in enemy_spawns:
		spawns.append(Painter.vector_serialize(spawn))
	return spawns

func serialize_placed_props() -> Dictionary:
	var next_id: int = 0
	var prop_library: Dictionary[int, String] = {}
	var inverted_prop_library: Dictionary[String, int] = {}
	for prop in Registry.props:
		prop_library[next_id] = prop.prop_name
		inverted_prop_library[prop.prop_name] = next_id
		next_id += 1
	
	var placed_prop_positions: Array[Dictionary] = []
	var placed_prop_ids: Array[int] = []
	
	for display in displays:
		placed_prop_positions.append(Painter.vector_serialize(display.position))
		placed_prop_ids.append(inverted_prop_library[display.prop.prop_name])
	
	return {
		"library": prop_library,
		"placed_ids": placed_prop_ids,
		"placed_positions": placed_prop_positions,
	}

const SAVE_CHUNK_BITS: int = 4
const SAVE_CHUNK_SIZE: int = 1 << SAVE_CHUNK_BITS
const SAVE_CHUNK_MASK: int = SAVE_CHUNK_SIZE - 1
const SAVE_CHUNK_AREA: int = SAVE_CHUNK_SIZE << SAVE_CHUNK_BITS

func serialize_tiles() -> Dictionary:
	var serialized_tiles: Array[Dictionary] = []
	for tile in Registry.tiles:
		serialized_tiles.append({
			"id": tile.id,
			"texture": tile.texture_name.get_basename(),
			"cutout": Painter.rect_serialize(tile.texture_cutout),
			"is_solid": tile.solid,
		})
	
	var placed_tiles: Dictionary[Vector2i, Array] = {}
	for coordinate: Vector2i in map.tiles:
		var tile: Tile = map.tiles[coordinate]
		var chunk_coordinate: Vector2i = _tile_to_save_chunk(coordinate)
		var array: Array = placed_tiles.get_or_add(chunk_coordinate, [])
		if array.is_empty():
			array.resize(SAVE_CHUNK_AREA)
			array.fill(0)
		array[_tile_to_local_linear_chunk(coordinate)] = tile.id
		placed_tiles[chunk_coordinate] = array
	
	var chunks: Array[Dictionary] = []
	for coordinate: Vector2i in placed_tiles:
		var array: Array = placed_tiles[coordinate]
		chunks.append({
			"coordinate": Painter.vector_serialize(coordinate),
			"tiles": array
		})
	
	return {
		"source": serialized_tiles,
		"chunks": chunks,
	}

func _tile_to_save_chunk(coordinate: Vector2i) -> Vector2i:
	return Vector2i(coordinate.x >> SAVE_CHUNK_BITS, coordinate.y >> SAVE_CHUNK_BITS)

func _save_chunk_to_tile(coordinate: Vector2i) -> Vector2i:
	return Vector2i(coordinate.x << SAVE_CHUNK_BITS, coordinate.y << SAVE_CHUNK_BITS)

func _tile_to_local_linear_chunk(coordinate: Vector2i) -> int:
	return (coordinate.x & SAVE_CHUNK_MASK) + ((coordinate.y & SAVE_CHUNK_MASK) << SAVE_CHUNK_BITS)

func _tile_local_linear_to_local(local_linear: int) -> Vector2i:
	return Vector2i(local_linear & SAVE_CHUNK_MASK, local_linear >> SAVE_CHUNK_BITS)


func tile_tab_changed(tab: int) -> void:
	if world_picker.draw_mode.get_tab_title(tab) == "Spawns":
		world_picker.deselect_all()
