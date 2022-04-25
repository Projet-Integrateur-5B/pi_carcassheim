using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Presets;
public class mask_sprite_preset : AssetPostprocessor
{
    Preset pres = AssetDatabase.LoadAssetAtPath<Preset>("Assets/Affichage_InGame/Tuiles/sprites/mask_preset.preset");
    void OnPreprocessTexture(Texture2D texture)
    {
        if (assetPath.StartsWith("Assets/Affichage_InGame/Tuiles/sprites/mask"))
        {
            pres.ApplyTo(assetImporter);
            // Debug.Log(assetPath);
        }
    }
}