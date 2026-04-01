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

var prop_name: String:
	get(): return name_entry.text

var textures: Array[Texture2D] = []

func _ready() -> void:
	PropRegistry.LoadTextures()
	populate_textues()
	save_button.pressed.connect(save_prop)
	prop_list.item_selected.connect(func(index: int): load_prop(prop_list.get_item_text(index)))
	update_prop_list()

func update_prop_list() -> void:
	prop_list.clear()
	
	for prop: Prop in PropRegistry.GetProps():
		var texture: AtlasTexture = AtlasTexture.new()
		texture.atlas = prop.Texture
		texture.region = prop.Cutout
		
		prop_list.add_item(
			prop.Name,
			texture
		)
	world_picker.update_props()

func load_prop(loading_prop_name: String) -> void:
	var prop: Prop = PropRegistry.GetByName(loading_prop_name)
	name_entry.text = loading_prop_name
	load_texture(prop.Texture)
	prop_texture.texture.region = prop.Cutout
	set_cutout(prop.Cutout)
	sort_x.value = prop.SortingOrigin.x
	sort_y.value = prop.SortingOrigin.y
	
	print("Size: " + str(prop.Texture.get_size()))
	print("Cutout: " + str(prop.Cutout))

func save_prop() -> void:
	if (prop_name == ""):
		printerr("Must assign a name to a saved prop!")
		return
	
	var prop: Prop = Prop.Make(
		prop_name,
		texture_picker.get_item_text(texture_picker.selected),
		prop_texture.texture.atlas,
		Rect2i(prop_texture.texture.region),
		Vector2i(int(sort_x.value), int(sort_y.value))
	)
	PropRegistry.AddOrUpdateProp(prop)
	update_prop_list()
	
	print("Size: " + str(prop.Texture.get_size()))
	print("Cutout: " + str(prop.Cutout))

func populate_textues() -> void:
	texture_picker.clear()
	
	textures = PropRegistry.GetTextures()
	var names: Array[StringName] = PropRegistry.GetNames()
	
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
	load_texture(PropRegistry.GetTextureFromName(texture_name))

func _process(_delta: float) -> void:
	w.max_value = x.max_value - x.value
	h.max_value = y.max_value - y.value
	
	prop_texture.texture.region = get_prop_rect()
	
	prop_texture.pivot_offset = get_prop_rect().size * 0.5
	prop_texture.scale = Vector2.ONE * zoom_slider.value
	
	sort_origin_display.global_position = prop_texture.global_position + Vector2(sort_x.value, sort_y.value) * zoom_slider.value

func get_prop_rect() -> Rect2:
	return Rect2(x.value, y.value, w.value, h.value)
