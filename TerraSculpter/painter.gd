extends Control
class_name Painter

@onready var name_entry: LineEdit = $Panel/MainMargin/MainVertical/Name/NameEntry

@onready var x: SpinBox = $Panel/MainMargin/MainVertical/Rect/Vertical/Horizontal/X
@onready var y: SpinBox = $Panel/MainMargin/MainVertical/Rect/Vertical/Horizontal/Y
@onready var w: SpinBox = $Panel/MainMargin/MainVertical/Rect/Vertical/Horizontal/W
@onready var h: SpinBox = $Panel/MainMargin/MainVertical/Rect/Vertical/Horizontal/H

@onready var prop_texture: TextureRect = $Panel/MainMargin/MainVertical/PropBackground/PropTexture

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

var prop_name: String:
	get(): return name_entry.text

var textures: Array[Texture2D] = []

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
	
	world_picker.update_props()

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
	update_prop_list()
	
	print("Size: " + str(prop.texture.get_size()))
	print("Cutout: " + str(prop.cutout))

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
	
	queue_redraw()

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
