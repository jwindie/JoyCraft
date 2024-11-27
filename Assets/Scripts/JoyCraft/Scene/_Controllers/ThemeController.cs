using JoyCraft.Scene;
using UnityEditor.Presets;
using UnityEngine;

namespace JoyCraft.Scene.Experimental {
    public class ThemeController : MonoBehaviour {
        [SerializeField] private ThemePreset preset;
        [SerializeField] private bool reload;

        [Header ("Fun Settings")]
        [SerializeField] private string themeFolder;
        [SerializeField] private bool loadRandomThemeOnStart;
        [SerializeField] private ThemePreset[] cycleThemeArray;

        [Header ("Refrences")]
        [SerializeField] private Material backgroundMaterial;
        [SerializeField] private Material boundaryBorderMaterial;
        [SerializeField] private Material cardMaterial;
        [SerializeField] private Material cardOutlineMaterial;

        private ThemePreset lastLoadedPreset;

        private void OnValidate () {
            if (reload) {
                reload = false;
                LoadPreset (preset);
            }
        }

        private void Start () {
            //load a random theme on start out of the themes folder
            if (loadRandomThemeOnStart) {
                var array = Resources.LoadAll<ThemePreset> (themeFolder);
                LoadPreset (array[Random.Range (0, array.Length)]);
            }
            else {
                //load the theme on start to make sure the presets's Load method is called
                //do NOT null check, there should always be a preset
                LoadPreset (preset);
            }
        }

        private void LoadPreset (ThemePreset preset) {
            //unload the last preset
            if (lastLoadedPreset) lastLoadedPreset.Unload (this);

            //set the new preset
            this.preset = lastLoadedPreset = preset;
            preset.Load (this);

            //set the background colors
            SetColorOnMaterial (backgroundMaterial, preset.BackgroundColor);
            SetColorOnMaterial (cardMaterial, preset.CardColor);
            SetColorOnMaterial (cardOutlineMaterial, preset.CardOutlineColor);

            //set boundary colors
            backgroundMaterial.SetColor ("OBColor", preset.BoundaryColor);
            boundaryBorderMaterial.SetColor ("ColorA", preset.BorderColorA);
            boundaryBorderMaterial.SetColor ("ColorB", preset.BorderColorB);

            //grid colors
            backgroundMaterial.SetColor ("GridColor", preset.GridColor);
        }

        private Color GetShadowedColor (Color color, float shadowIntensity) {
            return Vector4.Lerp (color, Vector4.zero, shadowIntensity);
        }

        private void SetColorOnMaterial (Material material, Color color) {
            material.SetColor ("Color", color);
            material.SetColor ("_Color", color);
            material.SetColor ("_BaseColor", color);
        }
    }
}