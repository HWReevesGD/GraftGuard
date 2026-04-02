extends Node2D
class_name Map

const DIRECTIONS: Array[Vector2i] = [ Vector2i.UP, Vector2i.DOWN, Vector2i.LEFT, Vector2i.RIGHT ]
var tiles: Dictionary[Vector2i, Tile] = {}

func clear() -> void:
	tiles.clear()
	queue_redraw()

func set_at_mouse(tile: Tile) -> void:
	if tile != null:
		tiles[global_to_tile(get_global_mouse_position())] = tile
	else:
		tiles.erase(global_to_tile(get_global_mouse_position()))
	queue_redraw()

func set_line(start: Vector2i, end: Vector2i, tile: Tile):
	
	var x_diff: float = abs(start.x - end.x)
	var y_diff: float = abs(start.y - end.y)
	
	if x_diff >= y_diff:
		_set_low_slope_line(start, end, tile)
	else:
		_set_high_slope_line(start, end, tile)
	
	queue_redraw()

func _set_low_slope_line(start: Vector2i, end: Vector2i, tile: Tile):
	var left: Vector2i = start if start.x < end.x else end
	var right: Vector2i = start if start.x >= end.x else end
	
	var delta_x = right.x - left.x
	var delta_y = right.y - left.y
	
	var slope_sign: int = sign(delta_y)
	
	var slope = 2 * delta_y
	var slope_error: int = slope - delta_x
	var y = left.y
	for x in range(left.x, right.x + 1):
		if tile != null:
			tiles[Vector2i(x, y)] = tile
		else:
			tiles.erase(Vector2i(x, y))
		
		slope_error += slope
		
		if sign(slope_error) != -slope_sign:
			y += slope_sign
			slope_error -= 2 * delta_x * slope_sign

func _set_high_slope_line(start: Vector2i, end: Vector2i, tile: Tile):
	var low: Vector2i = start if start.y < end.y else end
	var high: Vector2i = start if start.y >= end.y else end
	
	var delta_x = high.x - low.x
	var delta_y = high.y - low.y
	
	var slope_sign: int = sign(delta_x)
	
	var slope = 2 * delta_x
	var slope_error: int = slope - delta_y
	var x = low.x
	for y in range(low.y, high.y + 1):
		if tile != null:
			tiles[Vector2i(x, y)] = tile
		else:
			tiles.erase(Vector2i(x, y))
		
		slope_error += slope
		
		if sign(slope_error) != -slope_sign:
			x += slope_sign
			slope_error -= 2 * delta_y * slope_sign

func fill_at_mouse(tile: Tile) -> void:
	var mouse: Vector2i = global_to_tile(get_global_mouse_position())
	var to_fill: Tile = tiles.get(mouse)
	var interations: int = 0
	
	if to_fill != null && to_fill.id == tile.id:
		return
	if to_fill == tile:
		return
	
	var is_inside: Callable
	
	if to_fill == null:
		is_inside = func(coordinate: Vector2i): return tiles.get(coordinate) == null
	else:
		is_inside = func(coordinate: Vector2i):
			var check_tile: Tile = tiles.get(coordinate)
			return check_tile != null and check_tile.id == to_fill.id
	
	var frontier: Array[Vector2i] = [ mouse ]
	
	while not frontier.is_empty() and interations < 10000:
		interations += 1
		var current: Vector2i = frontier.pop_front()
		if is_inside.call(current):
			if tile != null:
				tiles[current] = tile
			else:
				tiles.erase(current)
			for direction: Vector2i in DIRECTIONS:
				frontier.append(current + direction)
	
	queue_redraw()

func global_to_tile(global_pos: Vector2) -> Vector2i:
	return Vector2i(to_local(global_pos) / Tile.SIZE)

func _draw() -> void:
	for coordinate in tiles.keys():
		var tile: Tile = tiles[coordinate]
		draw_texture(tile.texture, coordinate * Tile.SIZE)
