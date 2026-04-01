extends PanelContainer
class_name WorldPicker

@onready var prop_picker: ItemList = $Margin/Vertical/PropPicker
@onready var tile_picker: ItemList = $Margin/Vertical/TilePicker

var selected_prop: Prop = null

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
	
	prop_picker.item_selected.connect(
		func(index: int):
			selected_prop = Registry.from_name(prop_picker.get_item_text(index))
	)

func update_tiles() -> void:
	tile_picker.clear()
	for tile: Tile in Registry.tiles:
		tile_picker.add_icon_item(tile.texture)
