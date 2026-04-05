extends PanelContainer
class_name WorldPicker

@onready var prop_picker: ItemList = $Margin/Vertical/TabBar/Props/PropPicker
@onready var tile_picker: ItemList = $Margin/Vertical/TabBar/Tiles/TilePicker
@onready var data_picker: ItemList = $Margin/Vertical/TabBar/Data/DataPicker

var selected = null

func deselect_all() -> void:
	selected = null
	
	tile_picker.deselect_all()
	prop_picker.deselect_all()
	data_picker.deselect_all()

func _ready() -> void:
	prop_picker.item_selected.connect(
		func(index: int):
			deselect_all()
			selected = Registry.from_name(prop_picker.get_item_text(index))
	)
	tile_picker.item_selected.connect(
		func(index: int):
			deselect_all()
			selected = Registry.tiles[index]
	)
	data_picker.item_selected.connect(
		func(index: int):
			deselect_all()
			selected = data_picker.get_item_text(index)
	)

func update_props() -> void:
	prop_picker.clear()
	
	for prop: Prop in Registry.props:
		var texture: AtlasTexture = AtlasTexture.new()
		texture.atlas = prop.texture
		texture.region = prop.cutout
		
		prop_picker.add_item(
			prop.prop_name,
			texture
		)

func update_tiles() -> void:
	tile_picker.clear()
	for tile: Tile in Registry.tiles:
		tile_picker.add_icon_item(tile.texture)
