using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Herramienta de conveniencia para armar la animación del gatito Cat1b con los
/// 96 frames de Assets/Sprites/scene*.png. Se ejecuta DESDE Unity (menu Assets > Animaciones).
/// </summary>
public static class BuildCat1bGatito
{
    const int EXPECTED_FRAMES = 96;

    [MenuItem("Assets/Animaciones/Cat1b: armar animación (96 frames)", false, 0)]
    static void BuildCat1bClip()
    {
        var spritesDir = "Assets/Sprites";
        var files = Directory.GetFiles(spritesDir, "scene*.png")
            .Select(Path.GetFileName)
            .OrderBy(f => int.Parse(Regex.Replace(f, @"\D", "")))
            .ToArray();

        if (files.Length == 0)
        {
            Debug.LogError("Cat1b: no hay frames 'scene*.png' en Assets/Sprites/.");
            return;
        }

        if (files.Length != EXPECTED_FRAMES)
        {
            Debug.LogWarning($"Cat1b: encontrados {files.Length} frames (se esperaba {EXPECTED_FRAMES}). Se continua igual.");
        }

        var sprites = files.Select(f => AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/Sprites/{f}"))
            .Where(s => s != null).ToArray();

        if (sprites.Length == 0)
        {
            Debug.LogError("Cat1b: no se cargaron sprites. Asegurate de estar en el editor de Unity.");
            return;
        }

        // Crear el AnimationClip y asignar los 96 frames (Unity resuelve los fileIDs).
        var clip = new AnimationClip { name = "Cat1b", wrapMode = WrapMode.Loop, sampleRate = 60 };
        var frameList = sprites.Select(s => new SpriteAnimation { sprite = s, duration = 1f / 60f }).ToArray();

        try
        {
            AnimationUtility.SetSpriteAnimation(clip, typeof(SpriteRenderer), frameList);
        }
        catch (Exception e)
        {
            Debug.LogError(
                $"Cat1b: {e.GetType().Name}: \"{e.Message}\". " +
                "Tu version de Unity es antigua y no soporta SetSpriteAnimation. " +
                "Abre Assets/Animations/Cat1b.anim en el Inspector y arrastra los 96 sprites en orden (Unity los ordena y rellena el clip)."
            );
            return;
        }

        var clipPath = "Assets/Animations/Cat1b.anim";
        AssetDatabase.CreateAsset(clip, clipPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Dejar el prefabs mostrando el frame 0 para verlo en el editor.
        var prefabsPath = "Assets/Prefabs/move (18)11_0.prefab";
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabsPath);
        if (prefab != null)
        {
            var sr = prefab.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = sprites[0];
                EditorUtility.SetDirty(sr);
                Debug.Log($"Cat1b: prefabs 'move (18)11_0' actualizado, sprite inicial = '{sprites[0].name}'.");
            }
        }
        else
        {
            Debug.LogWarning("Cat1b: prefabs no encontrado (se armó el clip de todas formas).");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log(
            $"Cat1b: todo listo. Arrastra 'Assets/Prefabs/move (18)11_0' a la escena y dale Play."
        );
    }

    [MenuItem("Assets/Animaciones/Cat1b: verificar setup", false, 1)]
    static void VerifySetup()
    {
        var prefabsPath = "Assets/Prefabs/move (18)11_0.prefab";
        var controllerPath = "Assets/Animations/move (18)11_0.controller";
        var clipPath = "Assets/Animations/Cat1b.anim";

        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabsPath);
        var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);
        var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);

        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Verificacion Cat1b:");

        if (prefab != null)
        {
            var anim = prefab.GetComponent<Animator>();
            var controllerName = anim?.runtimeAnimatorController?.name ?? "(no hay Animator)";
            sb.AppendLine($"  Prefabs: OK -> Animator controller = '{controllerName}'");
        }
        else
        {
            sb.AppendLine("  Prefabs: NO encontrado");
        }

        if (controller != null)
        {
            var cat1State = controller.layers[0].stateMachine.states
                .Select(s => s.state).FirstOrDefault(s => s.name == "Cat1");
            var motionName = cat1State?.motion?.name ?? "(sin motion)";
            sb.AppendLine($"  Controller: estado Cat1 -> motion = '{motionName}'");
        }
        else
        {
            sb.AppendLine("  Controller: NO encontrado");
        }

        if (clip != null)
        {
            var frameCount = clip.frameCount;
            sb.AppendLine($"  Clip Cat1b: {frameCount} frames, duracion {clip.length:F2}s");
        }
        else
        {
            sb.AppendLine("  Clip Cat1b: NO creado (ejecuta primero 'Cat1b: armar animacion (96 frames)')");
        }

        EditorUtility.DisplayDialog("Verificacion Cat1b", sb.ToString(), "Ok");
    }
}
