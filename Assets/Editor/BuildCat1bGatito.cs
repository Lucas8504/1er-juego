using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

/// <summary>
/// Arma el AnimationClip Cat1b con los 96 frames de Assets/Sprites/scene*.png.
/// Se ejecuta desde Unity: menu Assets > Animaciones > Cat1b.
/// Usa la API estable (ObjectReferenceKeyframe), compatible con Unity 6.
/// </summary>
public static class BuildCat1bGatito
{
    const int ExpectedFrames = 96;
    const float FrameRate = 60f;
    const string SpritesDir = "Assets/Sprites";
    const string ClipPath = "Assets/Animations/Cat1b.anim";
    const string ControllerPath = "Assets/Animations/move (18)11_0.controller";
    const string PrefabPath = "Assets/Prefabs/move (18)11_0.prefab";
    const string StateName = "Cat1";

    [MenuItem("Assets/Animaciones/Cat1b: armar clip (96 frames)", false, 0)]
    static void BuildClip()
    {
        var absoluteSpritesDir = Path.Combine(Application.dataPath, "Sprites");
        var files = Directory.GetFiles(absoluteSpritesDir, "scene*.png")
            .Select(Path.GetFileName)
            .OrderBy(f => int.Parse(Regex.Replace(f, @"\D", "")))
            .ToArray();

        if (files.Length == 0)
        {
            Debug.LogError($"Cat1b: no hay frames 'scene*.png' en {SpritesDir}/.");
            return;
        }

        if (files.Length != ExpectedFrames)
            Debug.LogWarning($"Cat1b: encontrados {files.Length} frames (se esperaba {ExpectedFrames}). Se continua igual.");

        var sprites = files.Select(f => LoadSprite($"{SpritesDir}/{f}")).ToArray();

        var missing = files.Where((f, i) => sprites[i] == null).ToArray();
        if (missing.Length > 0)
        {
            Debug.LogError("Cat1b: no se pudo cargar el sprite de: " + string.Join(", ", missing));
            return;
        }

        // IMPORTANTE: nunca borrar y recrear el asset. DeleteAsset elimina el .meta
        // y Unity asigna un GUID nuevo, rompiendo la referencia del AnimatorState.
        // Se modifica el clip existente en su lugar para conservar el GUID.
        var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(ClipPath);
        bool isNewClip = clip == null;
        if (isNewClip)
            clip = new AnimationClip();

        clip.name = "Cat1b";
        clip.frameRate = FrameRate;
        clip.wrapMode = WrapMode.Loop;

        var keyframes = new ObjectReferenceKeyframe[sprites.Length];
        for (int i = 0; i < sprites.Length; i++)
            keyframes[i] = new ObjectReferenceKeyframe { time = i / FrameRate, value = sprites[i] };

        var binding = EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite");
        AnimationUtility.SetObjectReferenceCurve(clip, binding, keyframes);

        var settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = true;
        AnimationUtility.SetAnimationClipSettings(clip, settings);

        if (isNewClip)
            AssetDatabase.CreateAsset(clip, ClipPath);
        else
            EditorUtility.SetDirty(clip);

        // El controller tenia m_Motion apuntando a un GUID inexistente
        // (Cat1b.anim nunca se creo), asi que se repara la referencia aca.
        int repairedStates = PointControllerToClip(clip);

        // Sprite inicial del prefab (para verlo quieto en el editor)
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
        if (prefab != null)
        {
            var sr = prefab.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = sprites[0];
                EditorUtility.SetDirty(sr);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log(
            $"Cat1b: clip creado con {sprites.Length} frames, duracion {(sprites.Length / FrameRate):F2}s, " +
            $"loop, {FrameRate}fps. Guardado en {ClipPath}. " +
            $"Estados '{StateName}' reparados en el controller: {repairedStates}. Dale Play a SampleScene."
        );
    }

    /// <summary>
    /// Apunta el motion de los estados con nombre StateName al clip dado.
    /// Devuelve cuantos estados se repararon.
    /// </summary>
    static int PointControllerToClip(AnimationClip clip)
    {
        var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);
        if (controller == null)
        {
            Debug.LogWarning($"Cat1b: no se encontro el controller en {ControllerPath}.");
            return 0;
        }

        int repaired = 0;
        foreach (var layer in controller.layers)
        {
            if (layer.stateMachine == null) continue;
            foreach (var child in layer.stateMachine.states)
            {
                if (child.state.name != StateName) continue;
                child.state.motion = clip;
                repaired++;
            }
        }

        if (repaired > 0)
            EditorUtility.SetDirty(controller);

        return repaired;
    }

    [MenuItem("Assets/Animaciones/Cat1b: verificar setup", false, 1)]
    static void VerifySetup()
    {
        var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(ClipPath);
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);

        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Verificacion Cat1b:");

        if (clip != null)
        {
            var curves = AnimationUtility.GetObjectReferenceCurveBindings(clip);
            var count = 0;
            foreach (var b in curves)
                count += AnimationUtility.GetObjectReferenceCurve(clip, b)?.Length ?? 0;

            sb.AppendLine($"  Clip Cat1b: OK - {count} keyframes, {clip.length:F2}s, loop={clip.wrapMode == WrapMode.Loop}");
        }
        else
        {
            sb.AppendLine("  Clip Cat1b: NO existe - ejecuta 'Cat1b: armar clip (96 frames)'");
        }

        if (prefab != null)
        {
            var anim = prefab.GetComponent<Animator>();
            var sr = prefab.GetComponent<SpriteRenderer>();
            var controllerName = anim != null && anim.runtimeAnimatorController != null
                ? anim.runtimeAnimatorController.name : "(sin controller)";
            sb.AppendLine($"  Prefabs: Animator controller = '{controllerName}'");
            sb.AppendLine($"  Prefabs: sprite inicial = '{(sr != null && sr.sprite != null ? sr.sprite.name : "(sin sprite)")}'");
        }
        else
        {
            sb.AppendLine($"  Prefabs: NO encontrado en {PrefabPath}");
        }

        var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);
        if (controller != null)
        {
            foreach (var layer in controller.layers)
            {
                if (layer.stateMachine == null) continue;
                foreach (var child in layer.stateMachine.states)
                {
                    if (child.state.name != StateName) continue;
                    var motion = child.state.motion;
                    var status = motion == null ? "ROTA (null)"
                        : motion == clip ? "OK -> Cat1b"
                        : $"OK -> {motion.name}";
                    sb.AppendLine($"  Controller: estado '{StateName}' motion = {status}");
                }
            }
        }
        else
        {
            sb.AppendLine($"  Controller: NO encontrado en {ControllerPath}");
        }

        EditorUtility.DisplayDialog("Verificacion Cat1b", sb.ToString(), "Ok");
    }

    /// <summary>
    /// Carga el Sprite de un PNG. Los frames estan importados como
    /// "Sprite Mode: Multiple", asi que el Sprite es un sub-asset y no el asset principal.
    /// </summary>
    static Sprite LoadSprite(string assetPath)
    {
        var direct = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
        if (direct != null) return direct;

        var subAssets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
        if (subAssets == null) return null;

        return subAssets.OfType<Sprite>().OrderBy(s => s.name).FirstOrDefault();
    }
}
