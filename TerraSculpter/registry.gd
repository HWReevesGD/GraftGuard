class_name Registry

static var props: Array[Prop] = []
static var textures: Array[Texture2D] = []
static var names: Array[String] = []
static var tiles: Array[Tile] = []

static func get_content_directory() -> String:
	var directory: String
	
	if not OS.has_feature("editor"):
		directory = OS.get_executable_path().get_base_dir().path_join("../GraftGuard/Content/")
	else:
		directory = ProjectSettings.globalize_path("res://").path_join("../GraftGuard/Content/")
	if directory == null:
		directory = OS.get_executable_path().get_base_dir().path_join("../../../../GraftGuard/Content/")
	return directory

static func load_textures() -> void:
	var directory: String = get_content_directory() + "Environment/Props/"
	
	print("Texture Directory: " + directory)
	var dir: DirAccess = DirAccess.open(directory)
	
	for file_path: String in dir.get_files():
		if (file_path.get_extension() != "png"):
			continue
		print("   Loading Texture: " + file_path)
		
		var texture: Texture2D = ImageTexture.create_from_image(Image.load_from_file(directory.path_join(file_path)))
		assert(texture != null)
		textures.append(texture)
		names.append(file_path.get_file())

static func load_tiles() -> void:
	var directory: String = get_content_directory() + "Environment/Tilesets/"
	
	print("Tileset Directory: " + directory)
	var dir: DirAccess = DirAccess.open(directory)
	
	for file_path: String in dir.get_files():
		if (file_path.get_extension() != "png"):
			continue
		print("   Loading Tileset: " + file_path)
		
		var is_tileset_solid: bool = file_path.get_file().get_basename().rfind("solid")
		var atlas_image: Image = Image.load_from_file(directory.path_join(file_path))
		var atlas: Texture2D = ImageTexture.create_from_image(atlas_image)
		assert(atlas != null)
		
		var tile_count: Vector2i = Vector2i(atlas.get_size() / Tile.SIZE)
		
		for x in range(tile_count.x):
			for y in range(tile_count.y):
				var cutout: Rect2i = Rect2i(x * Tile.SIZE, y * Tile.SIZE, Tile.SIZE, Tile.SIZE)
				if is_region_empty(atlas_image, cutout):
					continue
				
				tiles.append(
					Tile.new(
						atlas,
						file_path.get_file(),
						cutout,
						is_tileset_solid
					)
				)

static func is_region_empty(image: Image, region: Rect2i) -> bool:
	var part: Image = image.get_region(region)
	var size: Vector2i = part.get_size()
	for x in size.x:
		for y in size.y:
			var pixel: Color = part.get_pixel(x, y)
			if pixel.a != 0.0:
				return false
	return true

static func add_or_update(prop: Prop) -> void:
	var prop_index: int = -1
	
	for index: int in range(props.size()):
		if (props[index].prop_name == prop.prop_name):
			prop_index = index
			break
	
	if prop_index != -1:
		props[prop_index] = prop
	else:
		props.append(prop)

static func remove(prop_name: String) -> bool:
	var index: int = props.find_custom(func(prop: Prop): return prop.prop_name == prop_name)
	if index == -1:
		return false
	props.remove_at(index)
	return true

static func from_name(name: String) -> Prop:
	return props.get(props.find_custom(func(prop: Prop): return prop.prop_name == name))

static func texture_from_name(name: String) -> Texture2D:
	return textures.get(names.find(name))

#"id": tile.id,
			#"texture": tile.texture_name,
			#"cutout": Painter.rect_serialize(tile.texture_cutout),
			#"is_solid": tile.solid,

static func tile_from_serialized(data: Dictionary) -> Tile:
	var texture_name: String = data["texture"] + ".png"
	var cutout: Rect2i = Rect2i(Painter.rect_deserialize(data["cutout"]))
	var solid: bool = data["is_solid"]
	
	var index: int = tiles.find_custom(
		func(tile: Tile):
			return (
				tile.texture_name == texture_name and
				tile.texture_cutout == cutout and
				tile.solid == solid
			))
	
	if index == -1:
		return null
	return tiles[index]
