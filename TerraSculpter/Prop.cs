using Godot;
using System;

[GlobalClass]
public partial class Prop : GodotObject
{
	public string Name;
	public StringName TextureName;
	public Texture2D Texture;
	public Rect2I Cutout;
	public Vector2I SortingOrigin;
	public bool EnableCollision;
	public Rect2I Collision;

	public Prop(string name, StringName textureName, Texture2D texture, Rect2I cutout, Vector2I sortingOrigin, bool enableCollision, Rect2I collision)
	{
		Name = name;
		TextureName = textureName;
		Texture = texture;
		Cutout = cutout;
		SortingOrigin = sortingOrigin;
		EnableCollision = enableCollision;
		Collision = collision;
	}
	
	public static Prop Make(string name, StringName textureName, Texture2D texture, Rect2I cutout, Vector2I sortingOrigin, bool enableCollision, Rect2I collision)
	{
		return new Prop(name, textureName, texture, cutout, sortingOrigin, enableCollision, collision);
	}
}
