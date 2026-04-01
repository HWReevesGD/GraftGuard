@tool
extends Marker2D

@export var radius: float = 8.0
@export var color: Color = Color.WHITE

func _process(_delta: float) -> void:
	queue_redraw()

func _draw() -> void:
	draw_circle(Vector2.ZERO, radius, color, false, 2.0)
	draw_circle(Vector2.ZERO, radius + 1, Color.BLACK, false, 2.0)
