using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GamingApp.EditorTools
{
    /// <summary>Writes UiArt images to PNG sprites (once per editor session) and loads them.</summary>
    static class UiSprites
    {
        const string Dir = "Assets/Art/UI/Generated";
        static readonly HashSet<string> written = new();

        public static Sprite Get(string name)
        {
            string path = $"{Dir}/{name}.png";
            if (!written.Contains(name) || !File.Exists(path))
            {
                Write(UiArt.Make(name), path);
                written.Add(name);
            }
            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        /// <summary>Tileable world texture (floors): default texture type, repeat wrap, mipmaps.</summary>
        public static Texture2D GetTexture(string name)
        {
            string path = $"{Dir}/{name}.png";
            if (!written.Contains(name) || !File.Exists(path))
            {
                Write(UiArt.Make(name), path);
                var imp = (TextureImporter)AssetImporter.GetAtPath(path);
                imp.textureType = TextureImporterType.Default;
                imp.wrapMode = TextureWrapMode.Repeat;
                imp.mipmapEnabled = true;
                imp.alphaIsTransparency = false;
                imp.textureCompression = TextureImporterCompression.CompressedHQ;
                imp.SaveAndReimport();
                written.Add(name);
            }
            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }

        static void Write(Art art, string path)
        {
            BuilderUtil.EnsureDir(Dir);
            var pixels = new Color[art.W * art.H];
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = new Color(art.Px[i * 4], art.Px[i * 4 + 1], art.Px[i * 4 + 2], art.Px[i * 4 + 3]);
            var tex = new Texture2D(art.W, art.H, TextureFormat.RGBA32, false);
            tex.SetPixels(pixels);
            File.WriteAllBytes(path, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);

            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            var imp = (TextureImporter)AssetImporter.GetAtPath(path);
            imp.textureType = TextureImporterType.Sprite;
            imp.spriteImportMode = SpriteImportMode.Single;
            imp.alphaIsTransparency = true;
            // World-space sprites are minified by the 3D camera and need mips to avoid shimmering.
            imp.mipmapEnabled = art.Name.StartsWith("enemy_") || art.Name.StartsWith("ship_") || art.Name.StartsWith("bolt_") || art.Name.StartsWith("gem_");
            imp.wrapMode = TextureWrapMode.Clamp;
            imp.textureCompression = TextureImporterCompression.Uncompressed;
            imp.spriteBorder = new Vector4(art.Border[0], art.Border[1], art.Border[2], art.Border[3]);
            imp.SaveAndReimport();
        }
    }
}
