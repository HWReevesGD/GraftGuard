using Godot;
using System;

[GlobalClass]
public partial class Prop : GodotObject
{
	public string Name;
	public StringName TextureName;
	public Texture2D Texture;
	public Rect2I Cutout;

	public Prop(string name, StringName textureName, Texture2D texture, Rect2I cutout)
	{
		Name = name;
		TextureName = textureName;
		Texture = texture;
		Cutout = cutout;
	}
	
	public static Prop Make(string name, StringName textureName, Texture2D texture, Rect2I cutout)
	{
		return new Prop(name, textureName, texture, cutout);
	}
}
