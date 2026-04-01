extends PanelContainer
class_name WorldPicker

@onready var prop_picker: ItemList = $Margin/Vertical/PropPicker
var selected_prop: Prop = null

func update_props() -> void:
	prop_picker.clear()
	
	for prop: Prop in PropRegistry.GetProps():
		var texture: AtlasTexture = AtlasTexture.new()
		texture.atlas = prop.Texture
		texture.region = prop.Cutout
		
		prop_picker.add_item(
			prop.Name,
			texture
		)
	
	prop_picker.item_selected.connect(
		func(index: int):
			selected_prop = PropRegistry.GetByName(prop_picker.get_item_text(index))
	)
