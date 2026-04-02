extends Control
class_name Painter

@onready var name_entry: LineEdit = $Panel/MainMargin/MainVertical/Name/NameEntry

@onready var x: SpinBox = $Panel/MainMargin/MainVertical/Rect/Vertical/Horizontal/X
@onready var y: SpinBox = $Panel/MainMargin/MainVertical/Rect/Vertical/Horizontal/Y
@onready var w: SpinBox = $Panel/MainMargin/MainVertical/Rect/Vertical/Horizontal/W
@onready var h: SpinBox = $Panel/MainMargin/MainVertical/Rect/Vertical/Horizontal/H

@onready var prop_texture: TextureRect = $Panel/MainMargin/MainVertical/PropBackground/PropTexture
@onready var prop_background: ColorRect = $Panel/MainMargin/MainVertical/PropBackground
@onready var prop_texture_back: TextureRect = $Panel/MainMargin/MainVertical/PropBackground/PropTextureBack

@onready var texture_picker: OptionButton = $Panel/MainMargin/MainVertical/Texture/TexturePicker

@onready var zoom_slider: HSlider = $Panel/MainMargin/MainVertical/Zoom/MarginContainer/SliderBox/ZoomSlider

@onready var save_button: Button = $Panel/MainMargin/MainVertical/SaveButton

@onready var prop_list: ItemList = $Panel/MainMargin/MainVertical/PropList

@onready var sort_x: SpinBox = $Panel/MainMargin/MainVertical/SortingOrigin/Vertical/Horizontal/X
@onready var sort_y: SpinBox = $Panel/MainMargin/MainVertical/SortingOrigin/Vertical/Horizontal/Y

@onready var sort_origin_display: Marker2D = $Panel/MainMargin/MainVertical/PropBackground/PropTexture/SortOrigin

@onready var world_picker: WorldPicker = $Picker

@onready var collision_check: CheckBox = $Panel/MainMargin/MainVertical/Collision/CollisionCheck
@onready var collision_data: VBoxContainer = $Panel/MainMargin/MainVertical/Collision/CollisionData
@onready var collision_x: SpinBox = $Panel/MainMargin/MainVertical/Collision/CollisionData/Horizontal/X
@onready var collision_y: SpinBox = $Panel/MainMargin/MainVertical/Collision/CollisionData/Horizontal/Y
@onready var collision_w: SpinBox = $Panel/MainMargin/MainVertical/Collision/CollisionData/Horizontal/W
@onready var collision_h: SpinBox = $Panel/MainMargin/MainVertical/Collision/CollisionData/Horizontal/H
@onready var inspector_panel: Panel = $Panel

var prop_name: String:
	get(): return name_entry.text
var world_rect: Rect2:
	get(): return Rect2(
		inspector_panel.size.x,
		0,
		get_viewport_rect().size.x - inspector_panel.size.x - world_picker.size.x,
		get_viewport_rect().size.y)

var textures: Array[Texture2D] = []
@onready var world: WorldPainting = $World

var is_collison_dragging: bool = false
var collision_drag_start: Vector2
var collision_drag_end: Vector2

var is_size_dragging: bool = false
var size_drag_start: Vector2
var size_drag_end: Vector2

func update_all_props() -> void:
	update_prop_list()
	world.world_picker.update_props()
	world.update_prop_data()

func _ready() -> void:
	Registry.load_textures()
	Registry.load_tiles()
	
	populate_textues()
	save_button.pressed.connect(save_prop)
	prop_list.item_selected.connect(func(index: int): load_prop(prop_list.get_item_text(index)))
	
	update_prop_list()
	world_picker.update_tiles()

func update_prop_list() -> void:
	prop_list.clear()
	
	for prop: Prop in Registry.props:
		var texture: AtlasTexture = AtlasTexture.new()
		texture.atlas = prop.texture
		texture.region = prop.cutout
		
		prop_list.add_item(
			prop.prop_name,
			texture
		)

func load_prop(loading_prop_name: String) -> void:
	var prop: Prop = Registry.from_name(loading_prop_name)
	name_entry.text = loading_prop_name
	load_texture(prop.texture)
	prop_texture.texture.region = prop.cutout
	set_cutout(prop.cutout)
	sort_x.value = prop.sorting_origin.x
	sort_y.value = prop.sorting_origin.y
	
	collision_check.button_pressed = prop.use_collision
	
	if prop.use_collision:
		collision_x.value = prop.collision.position.x
		collision_y.value = prop.collision.position.y
		collision_w.value = prop.collision.size.x
		collision_h.value = prop.collision.size.y
	else:
		collision_x.value = 0.0
		collision_y.value = 0.0
		collision_w.value = 32.0
		collision_h.value = 32.0
	
	print("Size: " + str(prop.texture.get_size()))
	print("Cutout: " + str(prop.cutout))

func save_prop() -> void:
	if (prop_name == ""):
		printerr("Must assign a name to a saved prop!")
		return
	
	var collision_rect: Rect2i = Rect2i(0.0, 0.0, 32.0, 32.0)
	
	if collision_check.button_pressed:
		collision_rect = Rect2i(
			int(collision_x.value),
			int(collision_y.value),
			int(collision_w.value),
			int(collision_h.value)
		)
	
	var prop: Prop = Prop.new(
		prop_name,
		texture_picker.get_item_text(texture_picker.selected),
		prop_texture.texture.atlas,
		Rect2i(prop_texture.texture.region),
		Vector2i(int(sort_x.value), int(sort_y.value)),
		collision_check.button_pressed,
		collision_rect
	)
	
	Registry.add_or_update(prop)
	update_all_props()
	
	print("Size: " + str(prop.texture.get_size()))
	print("Cutout: " + str(prop.cutout))

func erase_prop() -> void:
	if prop_name == "":
		return
	var removed: bool = Registry.remove(prop_name)
	update_all_props()
	print("Removed " + prop_name + "?: " + str(removed))

func populate_textues() -> void:
	texture_picker.clear()
	
	textures = Registry.textures
	var names: Array[String] = Registry.names
	
	for index: int in range(names.size()):
		texture_picker.add_item(names[index])
	
	load_texture_from_name(names[0])

func load_texture(texture: Texture2D):
	prop_texture.texture = AtlasTexture.new()
	prop_texture.texture.atlas = texture
	
	x.max_value = texture.get_width()
	y.max_value = texture.get_height()
	
	x.value = 0
	y.value = 0
	
	w.max_value = prop_texture.texture.get_width() - x.value
	h.max_value = prop_texture.texture.get_height() - y.value
	
	w.value = w.max_value
	h.value = h.max_value

func set_cutout(cutout: Rect2i) -> void:
	x.value = cutout.position.x
	y.value = cutout.position.y
	w.value = cutout.size.x
	h.value = cutout.size.y

func load_texture_from_name(texture_name: StringName) -> void:
	load_texture(Registry.texture_from_name(texture_name))

func _process(_delta: float) -> void:
	w.max_value = x.max_value - x.value
	h.max_value = y.max_value - y.value
	
	prop_texture.texture.region = get_prop_rect()
	
	prop_texture.pivot_offset = get_prop_rect().size * 0.5
	prop_texture.scale = Vector2.ONE * zoom_slider.value
	
	sort_origin_display.global_position = prop_texture.global_position + Vector2(sort_x.value, sort_y.value) * zoom_slider.value
	
	collision_data.visible = collision_check.button_pressed
	
	update_collision_dragging()
	update_origin_setting()
	#update_cutout_dragging()
	update_background()
	
	queue_redraw()

func update_background() -> void:
	prop_texture_back.texture = prop_texture.texture.atlas
	prop_texture_back.global_position = prop_texture.global_position - Vector2(x.value, y.value) * zoom_slider.value
	prop_texture_back.scale = prop_texture.scale
	prop_texture_back.pivot_offset = prop_texture.pivot_offset

func set_collision_rect(collision_rect: Rect2i) -> void:
	collision_rect = collision_rect.abs()
	collision_x.value = collision_rect.position.x
	collision_y.value = collision_rect.position.y
	collision_w.value = collision_rect.size.x
	collision_h.value = collision_rect.size.y

func update_collision_dragging() -> void:
	var mouse_inside: bool = (prop_texture.get_global_rect().has_point(get_global_mouse_position()) and 
		prop_background.get_global_rect().has_point(get_global_mouse_position()))
	var mouse_clicked: bool = mouse_inside and Input.is_action_just_pressed("left_click")# and not Input.is_action_pressed("ctrl")
	var mouse_down: bool = Input.is_action_pressed("left_click")
	
	if mouse_clicked:
		is_collison_dragging = true
		collision_drag_start = prop_texture.get_local_mouse_position()
	if not mouse_down:
		is_collison_dragging = false
	
	if is_collison_dragging:
		collision_drag_end = prop_texture.get_local_mouse_position()
		set_collision_rect(Rect2i(
			collision_drag_start,
			collision_drag_end - collision_drag_start
		))

func update_origin_setting() -> void:
	var mouse_inside: bool = prop_background.get_global_rect().has_point(get_global_mouse_position())
	var mouse_clicked: bool = mouse_inside and Input.is_action_just_pressed("right_click")
	if mouse_clicked:
		sort_x.value = prop_texture.get_local_mouse_position().x
		sort_y.value = prop_texture.get_local_mouse_position().y

func set_size_rect(size_rect: Rect2) -> void:
	size_rect = size_rect.abs()
	x.value = size_rect.position.x
	y.value = size_rect.position.y
	w.value = size_rect.size.x
	h.value = size_rect.size.y

func get_size_rect() -> Rect2i:
	@warning_ignore("narrowing_conversion")
	return Rect2i(
		x.value,
		y.value,
		w.value,
		h.value
	)

func rect_closest_point(rect: Rect2, point: Vector2) -> Vector2:
	var clamped_point = point.clamp(rect.position, rect.end)
	if rect.has_point(point):
		return point
	return clamped_point

func update_cutout_dragging() -> void:
	var mouse_inside: bool = prop_background.get_global_rect().has_point(get_global_mouse_position())
	var mouse_clicked: bool = mouse_inside and Input.is_action_just_pressed("left_click") and Input.is_action_pressed("ctrl") 
	var mouse_down: bool = Input.is_action_pressed("left_click")
	var mouse_local: Vector2 = prop_texture.get_local_mouse_position().clamp(
		-Vector2(x.value, y.value),
		prop_texture_back.size - Vector2(x.value, y.value)
	)
	
	if mouse_clicked:
		is_size_dragging = true
		size_drag_start = mouse_local
	if not mouse_down:
		if is_size_dragging:
			is_size_dragging = false
			set_size_rect(
				Rect2i(
					get_size_rect().position + Vector2i(size_drag_start),
					size_drag_end - size_drag_start
				)
			)
	
	if is_size_dragging:
		size_drag_end = mouse_local

func get_prop_rect() -> Rect2:
	return Rect2(x.value, y.value, w.value, h.value)

func _draw() -> void:
	var tex_pos: Vector2 = prop_texture.global_position - global_position
	if collision_check.button_pressed:
		draw_rect(
		Rect2(
			(Vector2(collision_x.value, collision_y.value) * zoom_slider.value + tex_pos),
			Vector2(collision_w.value, collision_h.value) * zoom_slider.value
			),
		Color.RED,
		false,
		2.0
		)
	if is_size_dragging:
		draw_rect(
		Rect2(
			(size_drag_start * zoom_slider.value + tex_pos),
			(size_drag_end - size_drag_start) * zoom_slider.value
			).abs(),
		Color.GREEN,
		false,
		2.0
		)

func export_all():
	export_prop_definitions()

func export_prop_definitions() -> void:
	var exported_props: Array[Dictionary] = []
	
	for prop: Prop in Registry.props:
		var exported: Dictionary = {
			"name": prop.prop_name,
			"texture_file": prop.texture_name.get_file().get_basename(),
			"cutout_rectangle": rect_serialize(prop.cutout),
			"sorting_origin": vector_serialize(prop.sorting_origin),
			"uses_collision": prop.use_collision,
			"collision_rectangle": rect_serialize(prop.collision),
		}
		exported_props.append(exported)
	
	var save_directory: String = Registry.get_content_directory() + "Environment/"
	var json: String = JSON.stringify(exported_props, "\t", false)
	
	var file: FileAccess = FileAccess.open(save_directory + "prop_definitions.json", FileAccess.WRITE)
	file.store_string(json)
	file.close()
	
	print("Exported to: " + save_directory + "prop_definitions.json")

func rect_serialize(rect: Rect2) -> Dictionary:
	return {
		"position": vector_serialize(rect.position),
		"size": vector_serialize(rect.size),
	}

func vector_serialize(vector: Vector2) -> Dictionary:
	return {
		"X": vector.x,
		"Y": vector.y,
	}
