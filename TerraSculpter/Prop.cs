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

	public Prop(string name, StringName textureName, Texture2D texture, Rect2I cutout, Vector2I sortingOrigin)
	{
		Name = name;
		TextureName = textureName;
		Texture = texture;
		Cutout = cutout;
		SortingOrigin = sortingOrigin;
	}
	
	public static Prop Make(string name, StringName textureName, Texture2D texture, Rect2I cutout, Vector2I sortingOrigin)
	{
		return new Prop(name, textureName, texture, cutout, sortingOrigin);
	}
}
