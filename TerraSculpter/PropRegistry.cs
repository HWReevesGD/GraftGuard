using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

[GlobalClass]
public partial class PropRegistry : RefCounted
{
	public static readonly string Dir = Path.Join(System.IO.Directory.GetCurrentDirectory(), "../GraftGuard/Content/Environment");
	public static Array<Texture2D> Textures = [];
	public static Array<StringName> Names = [];
	public static Array<Prop> Props = [];
	
	public static void LoadTextures()
	{
		// Loop through all files
		foreach (string filepath in Directory.GetFiles(Dir))
		{
			// Skip non-png
			if (Path.GetExtension(filepath) != ".png")
			{
				continue;
			}

			// Create and save a texture from the Image file
			Textures.Add(ImageTexture.CreateFromImage(Image.LoadFromFile(filepath)));
			Names.Add(Path.GetFileName(filepath));
		}
	}

	/// <summary>
	/// Adds or Updates the given prop
	/// </summary>
	/// <param name="prop"></param>
	public static void AddOrUpdateProp(Prop prop)
	{
		// Get index of possible prop
		int? propIndex = null;
		for (int i = 0; i < Props.Count; i++)
		{
			if (Props[i].Name == prop.Name)
			{
				propIndex = i;
				break;
			}
		}
		if (propIndex is int index)
		{
			Props[index] = prop;
		} else
		{
			Props.Add(prop);
		}
	}

	public static Prop GetByName(string name)
	{
		return Props.First((prop) => prop.Name == name);
	}

	public static Texture2D GetTextureFromName(StringName name)
	{
		for (int i = 0; i < Names.Count; i++)
		{
			if (Names[i] == name)
			{
				return Textures[i];
			}
		}
		return null;
	}

	public static Array<Texture2D> GetTextures() => Textures;
	public static Array<StringName> GetNames() => Names;
	public static Array<Prop> GetProps() => Props;
}
